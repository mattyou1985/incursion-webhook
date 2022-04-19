#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace IncursionWebhook.Models
{
    public class Constellation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int RegionId { get; set; }

        public string DotlanUrl(string regionName)
        {
            return $"https://evemaps.dotlan.net/map/{regionName}/{Name}".Replace(" ", "_");
        }
    }
}
#pragma warning restore CS8618