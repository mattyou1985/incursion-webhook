#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Newtonsoft.Json;

namespace IncursionWebhook.Models
{
    public class Region
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [JsonIgnore]
        public string DotlanUrl
        {
            get => $"https://evemaps.dotlan.net/map/{Name}".Replace(" ", "_");
        }
    }
}
