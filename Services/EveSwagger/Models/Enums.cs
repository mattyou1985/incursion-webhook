namespace IncursionWebhook.Services.EveSwagger.Models
{
    public enum RouteFlag
    {
        Secure = 0,
        Shortest = 1,
        Insecure = 2
    }

    /// <summary>The incursion phase (established, mobo, etc.)</summary>
    public enum State
    {
        Unknown = 0,
        /// <summary>
        /// Initial phase of the incursion. Sansha's Nation is establishing control of the constellation.
        /// </summary>
        Established = 1,
        /// <summary>
        /// The incursion has reached its apex. Sansha's Nation has deployed its mothership to protect its headquarters.
        /// </summary>
        Mobilizing = 2,
        /// <summary>
        /// The incursion is ending. Sansha's Nation has either overrun the constellation, or is preparing to retreat.
        /// </summary>
        Withdrawing = 3
    }
}
