namespace IncursionWebhook.Services.Discord
{
    public static class DiscordExtensions
    {
        /// <summary>Adds a DiscordWebhook Service to the <see cref="IServiceCollection"/></summary>
        public static void AddDiscord(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IDiscordService, DiscordServices>();
        }

        /// <summary>Returns a Discord timestamp string</summary>
        /// <remarks><em>Format:</em> Monday, 13:22 (in 2 days)</remarks>
        public static string DiscordTimestamps(this DateTime timestamp, bool withDiffForHumans = true)
        {
            long unixTimestamp = (long)timestamp.Subtract(DateTime.UnixEpoch).TotalSeconds;

            if (!withDiffForHumans) return timestamp.ToString("dddd, HH:mm");

            return string.Format("{0} ({1})",
                timestamp.ToString("dddd, HH:mm"),
                $"<t:{unixTimestamp}:R>"
            );
        }

        /// <summary>Replace spaces in URLs with an underscore</summary>
        public static string DotlanSafe(this string s)
        {
            return s.Replace(" ", "_");
        }
    }
}
