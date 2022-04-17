using IncursionWebhook.Services.EveSwagger.Attributes;
using Newtonsoft.Json;

namespace IncursionWebhook.Services.EveSwagger.Models
{
    public class EsiIncursion
    {
        [JsonProperty("constellation_id")]
        public int ConstellationId { get; set; }

        [JsonProperty("faction_id")]
        public int FactionId { get; set; }

        [JsonProperty("has_boss")]
        public bool HasBoss { get; set; }

        [JsonProperty("influence")]
        public float Influence { get; set; }

        [JsonProperty("state")]
        [JsonConverter(typeof(IncursionStateConverter))]
        public State State { get; set; }

        [JsonProperty("infested_solar_systems")]
        public int[] InfestedSolarSystems { get; set; }

        [JsonProperty("staging_solar_system_id")]
        public int StagingSystemId { get; set; }
    }
}
