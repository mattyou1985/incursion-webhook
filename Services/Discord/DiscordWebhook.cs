﻿using Discord;
using Discord.Webhook;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace IncursionWebhook.Services.Discord
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public class DiscordWebhook
    {
        private const string BASE_URL = "https://discord.com/channels";
        private DiscordWebhookClient _client;

        /// <summary>Discord Webhook ID</summary>
        [Key]
        public ulong Id { get; set; }

        #region Webhook User Information
        public string? Avatar { get; set; }
        public string? Name { get; set; }
        #endregion

        #region Webhook Channel
        /// <summary>The Channel ID that payloads are sent to</summary>
        [JsonProperty("channel_id")]
        public string ChannelId { get; set; }

        /// <summary>The Server ID that the webhook owns the webhook</summary>
        [JsonProperty("guild_id")]
        public string GuildId { get; set; }

        /// <summary>The Webhook Channel URL</summary>
        /// <example>https://discord.com/channels/{guild_id}/{channel_id}</example>
        /// <remarks>To jump to a specific message, append "/{MessageId}" to this URL</remarks>
        public string Url
        {
            get => $"{BASE_URL}/{GuildId}/{ChannelId}";
        }
        #endregion
#pragma warning restore CS8618

        /// <summary>Sends a new message to the Discord Webhook</summary>
        /// <returns>The messageId</returns>
        public async Task<ulong> SendMessage(string? text, bool isTTS = false, IEnumerable<Embed> embeds = null)
        {
            _client = new(Url);
            return await _client.SendMessageAsync(text, isTTS, embeds);
        }

        /// <inheritdoc cref="DiscordWebhookClient.DeleteMessageAsync(ulong, Discord.RequestOptions)"/>
        public async void DeleteMessage(ulong messageId)
        {
            _client = new(Url);
            await _client.DeleteMessageAsync(messageId);
        }
    }
}
