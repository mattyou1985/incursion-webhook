#pragma warning disable CS8602 // Dereference of a possibly null reference.
using IncursionWebhook.Attributes;
using IncursionWebhook.Services.Discord;
using IncursionWebhook.Services.Redis;
using IncursionWebhook.Services.SpawnMonitor.Models;
using Microsoft.AspNetCore.Mvc;

namespace IncursionWebhook.Services.SpawnMonitor.Controllers
{
    /// <summary>Manage webhooks for incursion spawns</summary>
    [ApiController, DevelopmentOnly, Route("api/spawn-webhook")]
    public class SpawnWebhookController : ControllerBase
    {
        private const string redisNamespace = "spawnWebhooks";

        private readonly IDiscordService _discord;
        private readonly ILogger<SpawnWebhookController> _logger;
        private readonly IRedis _redis;
        
        /// <inheritdoc cref="SpawnWebhookController"/>
        public SpawnWebhookController(IDiscordService discord, ILogger<SpawnWebhookController> logger, IRedis redis)
        {
            _discord = discord;
            _logger = logger;
            _redis = redis;
        }

        /// <summary>Return a list of Spawn Webhooks</summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<SpawnWebhook>))]
        public async Task<IActionResult> Index()
        {
            List<SpawnWebhook> webhooks = await _redis.Get<List<SpawnWebhook>>("spawnWebhooks");
            return Ok(webhooks);
        }

        /// <summary>Register a new Spawn Webhook</summary>
        /// <param name="webhookUrl">The URL of a Discord Webhook</param>
        /// <param name="Highsec">Monitor Highsec spawns</param>
        /// <param name="Lowsec">Monitor Lowsec spawns</param>
        /// <param name="Nullsec">Monitor Nullsec spawns</param>
        /// <param name="pingGroup"><em>optional:</em> Send pings to this group when a new spawn is detected. Leave null to disable this feature</param>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(SpawnWebhook))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        public async Task<IActionResult> Create([FromForm] Uri webhookUrl,
            [FromForm] bool Highsec = true, [FromForm] bool Lowsec = true, [FromForm] bool Nullsec = true, [FromForm] ulong? pingGroup = null)
        {
            if(!_discord.TryCreate(webhookUrl, out DiscordWebhook? webhook, out string? error))
            {
                return BadRequest(error);
            }

#pragma warning disable CS8604 // Possible null reference argument.
            SpawnWebhook newWebhook = new(webhook)
            {
                WebhookUrl = webhookUrl.ToString(),
                Highsec = Highsec,
                Lowsec = Lowsec,
                Nullsec = Nullsec,
                PingGroup = pingGroup
            };


            // Get existing webhooks, check for duplicates and if none are found add our new 
            // webhook to the collection, the database, and return a 201 Created result
            List<SpawnWebhook> webhooks = await _redis.Get<List<SpawnWebhook>>(redisNamespace) ?? new();
            if(webhooks.Any(x => x.Id == newWebhook.Id))
            {
                return BadRequest($"Webhook {newWebhook?.Id} already in DB");
            }
            
            webhooks.Add(newWebhook);
            await _redis.Set(redisNamespace, webhooks);
            return Created(Url.Action("Index"), newWebhook);
        }
#pragma warning restore CS8604

        /// <summary>Change a Spawn Webhook's settings</summary>
        /// <param name="id">The ID of the webhook that you wish to update</param>
        /// <param name="Highsec">Monitor Highsec spawns</param>
        /// <param name="Lowsec">Monitor Lowsec spawns</param>
        /// <param name="Nullsec">Monitor Nullsec spawns</param>
        /// <param name="pingGroup"><em>optional:</em> Send pings to this group when a new spawn is detected. Leave null to disable this feature</param>
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public async Task<IActionResult> Update(string id,
            [FromForm] bool Highsec = true, [FromForm] bool Lowsec = true, [FromForm] bool Nullsec = true, [FromForm] ulong? pingGroup = null)
        {
            List<SpawnWebhook> webhooks = await _redis.Get<List<SpawnWebhook>>(redisNamespace);
            SpawnWebhook? webhook = webhooks?.FirstOrDefault(x => x.Id == id);
            if (webhook is null) return NotFound($"Webhook {id} not found");

            // Update values
            webhook.Highsec = Highsec;
            webhook.Lowsec = Lowsec;
            webhook.Nullsec = Nullsec;
            webhook.PingGroup = pingGroup;

            // Update webhooks in database, and return 200
            await _redis.Set(redisNamespace, webhooks);
            return Ok();
        }

        /// <summary>Refresh a webhook's metadata using the URL provided during creation</summary>
        /// <param name="id">The ID of the webhook to be refreshed</param>
        [HttpPatch("{id}/refresh")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RefreshDiscordInfo(string id) 
        {
            List<SpawnWebhook> webhooks = await _redis.Get<List<SpawnWebhook>>(redisNamespace);
            SpawnWebhook? webhook = webhooks?.FirstOrDefault(x => x.Id == id);
            if (webhook is null) return NotFound($"Webhook {id} not found");

            if (_discord.TryCreate(new Uri(webhook.WebhookUrl), out DiscordWebhook? updatedWebhook, out string? error))
            {
                return BadRequest(error);
            }


            webhook.Id          = updatedWebhook.Id;
            webhook.Avatar      = updatedWebhook.Avatar;
            webhook.Name        = updatedWebhook.Name;
            webhook.ChannelId   = updatedWebhook.ChannelId;
            webhook.GuildId     = updatedWebhook.GuildId;

            // Update webhooks in database, and return 200
            await _redis.Set(redisNamespace, webhooks);
            return Ok();
        }

        /// <summary>Delete a SpawnWebhook &amp; stop sending it messages</summary>
        /// <param name="id">The ID of the webhook that is to be deleted</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public async Task<IActionResult> Delete(string id)
        {
            // Get all webhooks from the database
            List<SpawnWebhook> webhooks = await _redis.Get<List<SpawnWebhook>>(redisNamespace);

            // Try to find the webhook we want to delete
            SpawnWebhook? webhook = webhooks?.FirstOrDefault(wb => wb.Id == id);
            if (webhook is null) return NotFound($"Webhook {id} not found");

            // Delete the webhook & update DB
            webhooks.Remove(webhook);
            await _redis.Set(redisNamespace, webhooks);
            return Ok();
        }
    }
}
#pragma warning restore CS8602 // Dereference of a possibly null reference.