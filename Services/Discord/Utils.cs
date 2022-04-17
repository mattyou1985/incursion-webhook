namespace IncursionWebhook.Services.Discord
{
    /// <summary>Helper class for the Discord Webhook Service</summary>
    public static class Utils
    {
        /// <summary>Create a Discord Markdown URL</summary>
        /// <param name="href">The <u>href</u> attribute of the URL</param>
        /// <param name="text">The link text</param>
        public static string MarkdownUrl(string href, string text) => $"[{text}]({href})";
    }
}
