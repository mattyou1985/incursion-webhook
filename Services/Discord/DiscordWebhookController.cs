using Microsoft.AspNetCore.Mvc;

namespace IncursionWebhook.Services.Discord
{
    [ApiController, Route("api/discord")]
    public class DiscordWebhookController : ControllerBase
    {
        private readonly IWebhookClient _webhookClient;
        private readonly List<DiscordWebhook> _webhooks = new();

        public DiscordWebhookController(IWebhookClient webhookClient)
        {
            _webhookClient = webhookClient;
        }

        /// <summary>List avaliable webhooks</summary>
        [HttpGet]
        public IActionResult Index()
        {
            return Ok(_webhooks);
        }

        /// <summary>Create a new Discord Webhook</summary>
        [HttpPost]
        public async Task<IActionResult> Index(Uri webhookUrl)
        {
            if (!_webhookClient.TryCreate(webhookUrl, out DiscordWebhook? webhook, out string? error))
            {
                return BadRequest(error);
            }

            if (_webhooks.Any(x => x.Id == webhook.Id))
            {
                return BadRequest($"Webhook {webhook.Id} already in DB");
            }

            _webhooks.Add(webhook);
            return Ok(webhook);
        }

        /// <summary>Delete a specific Discord Webhook</summary>
        /// <param name="id">\\\todo</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(ulong id)
        {
            DiscordWebhook? webhook = _webhooks.FirstOrDefault(x => x.Id == id);
            if (webhook == null) return NotFound();

            _webhooks.Remove(webhook);
            return Ok();
        }
    }
}
