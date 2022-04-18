using IncursionWebhook.Models;
using Newtonsoft.Json;

namespace IncursionWebhook.Services.Redis
{
    public static class DbSeeder
    {
        public static async Task InitializeAsync(IRedis redis)
        {
            List<Region> regions = JsonConvert.DeserializeObject<List<Region>>(File.ReadAllText("Data/Regions.json")) ?? new();
            regions.ForEach(region =>
            {
                redis.Set($"region:{region.Id}", region);
            });

            List<Constellation> constellations = JsonConvert.DeserializeObject<List<Constellation>>(File.ReadAllText("Data/Constellations.json")) ?? new();
            constellations.ForEach(constellation =>
            {
                redis.Set($"constellation:{constellation.Id}", constellation);
            });

            List<SolarSystem> systems = JsonConvert.DeserializeObject<List<SolarSystem>>(File.ReadAllText("Data/Systems.json")) ?? new();
            systems.ForEach(system =>
            {
                redis.Set($"system:{system.Id}", system);
            });
        }
    }
}
