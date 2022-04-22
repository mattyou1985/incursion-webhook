namespace IncursionWebhook
{
    public static class Extensions
    {
        public static string? AsNullIfEmpty(this string str)
        {
            return !string.IsNullOrEmpty(str) ? str : null;
        }
    }
}
