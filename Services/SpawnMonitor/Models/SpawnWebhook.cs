using IncursionWebhook.Services.Discord;

namespace IncursionWebhook.Services.SpawnMonitor.Models
{
    public class SpawnWebhook : DiscordWebhook
    {
        public SpawnWebhook() { }
        public SpawnWebhook(DiscordWebhook webhook)
        {
            Id = webhook.Id;
            Avatar = webhook.Avatar;
            Name = webhook.Name;
            ChannelId = webhook.ChannelId;
            GuildId = webhook.GuildId;
        }

        /// <summary>Monitor for Highsec Spawns</summary>
        public bool Highsec { get; set; }

        /// <summary>Monitor for Lowsec Spawns</summary>
        public bool Lowsec { get; set; }

        /// <summary>Monitor for Nullsec Spawns</summary>
        public bool Nullsec { get; set; }

        /// <summary>The RoleId that should be @mentioned for <u>NEW</u> spawns</summary>
        /// <remarks>Leave NULL for none</remarks>
        public ulong? PingGroup { get; set; }
    }
}
