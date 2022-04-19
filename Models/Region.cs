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
