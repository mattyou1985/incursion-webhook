#pragma warning disable CS8604 // Possible null reference argument.
using IncursionWebhook.Services.Redis;
using Microsoft.AspNetCore.Mvc;

namespace IncursionWebhook.Services.Discord
{
    [ApiController, Route("api/discord")]
    public class DiscordWebhookController : ControllerBase
    {
        private readonly IWebhookClient _webhookClient;
        private readonly IRedis _redis;

        public DiscordWebhookController(IWebhookClient webhookClient, IRedis redis)
        {
            _webhookClient = webhookClient;
            _redis = redis;
        }

        /// <summary>List avaliable webhooks</summary>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<DiscordWebhook> webhooks = await _redis.Get<List<DiscordWebhook>>("discord-webhooks");
            return Ok(webhooks);
        }

        /// <summary>Create a new Discord Webhook</summary>
        [HttpPost]
        public async Task<IActionResult> Index(Uri webhookUrl, DiscordWebhook? webhook)
        {
            if (!_webhookClient.TryCreate(webhookUrl, out DiscordWebhook? newWebhook, out string? error))
            {
                return BadRequest(error);
            }

            List<DiscordWebhook> webhooks = await _redis.Get<List<DiscordWebhook>>("discord-webhooks") ?? new();

            if (webhooks.Any(x => x.Id == newWebhook?.Id))
            {
                return BadRequest($"Webhook {newWebhook?.Id} already in DB");
            }
            
            // We need to set the URL here so it is avaliable
            // when we want to make HTTP calls via the Discord API
            newWebhook.WebhookUrl = webhookUrl.ToString();

            // Add the new webhook to REDIS
            webhooks.Add(newWebhook);
            await _redis.Set("discord-webhooks", webhooks);

            // Return result
            return Created(Url.Action("Index"), newWebhook);
        }

        /// <summary>Delete a specific Discord Webhook</summary>
        /// <param name="id">\\\todo</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            List<DiscordWebhook> webhooks = await _redis.Get<List<DiscordWebhook>>("discord-webhooks") ?? new();
            DiscordWebhook? webhook = webhooks.FirstOrDefault(x => x.Id == id);
            if (webhook == null) return NotFound();
            await _redis.Delete("discord-webhooks");
            return Ok();
        }
    }
}
#pragma warning restore CS8604