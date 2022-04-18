using Discord;

namespace IncursionWebhook.Services.Discord
{
    /// <summary>Helper class for the Discord Webhook Service</summary>
    public static class Utils
    {
        /// <summary>Create a Discord Markdown URL</summary>
        /// <param name="href">The <u>href</u> attribute of the URL</param>
        /// <param name="text">The link text</param>
        public static string MarkdownUrl(string href, string text) => $"[{text}]({href})";

        /// <summary>Creates a Dotlan Universe URL</summary>
        /// <remarks>Constellation has priority over system. If both are provided the system will be omitted</remarks>
        public static string DotlanUniverseUrl(string region, string? constellation = null, string? system = null)
        {
            string url = $"https://evemaps.dotlan.net/map/{region}";
            if (!string.IsNullOrWhiteSpace(constellation)) return $"{url}/{constellation}".DotlanSafe();
            if (!string.IsNullOrWhiteSpace(system)) return $"{url}/{system}".DotlanSafe();
            return url.DotlanSafe();
        }

        /// <summary>Get the color of a given system security status</summary>
        /// <remarks>see: https://web.archive.org/web/20120219150840/http://blog.evepanel.net/eve-online/igb/colors-of-the-security-status.html</remarks>
        public static Color SecStatusColor (this double secStatus)
        {
            secStatus = Double.Parse(secStatus.ToString("0.0"));

            return secStatus switch
            {
                1.0 => new Color(47, 239, 239),
                0.9 => new Color(72, 240, 192),
                0.8 => new Color(0, 239, 71),
                0.7 => new Color(0, 240, 0),
                0.6 => new Color(143, 239, 47),
                0.5 => new Color(239, 239, 0),
                0.4 => new Color(215, 119, 0),
                0.3 => new Color(240, 96, 0),
                0.2 => new Color(240, 70, 2),
                0.1 => new Color(215, 48, 0),
                _ => new Color(240, 0, 0),
            };
        }
    }
}
