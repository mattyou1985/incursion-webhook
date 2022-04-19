#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Newtonsoft.Json;

namespace IncursionWebhook.Services.Redis
{
    public class SolarSystem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        [JsonProperty("SecurityStatus")]
        public double TrueSecurityStatus { get; set; }

        public SiteType SiteType { get; set; } = SiteType.None;
        
        [JsonIgnore]
        public float SecurityStatus
        {
            get
            {
                if (TrueSecurityStatus > 0.0 && TrueSecurityStatus < 0.05)
                {
                    float tmp = (float)Math.Ceiling(TrueSecurityStatus * 100);
                    return (float)tmp / 100;
                }

                return (float)Math.Round(TrueSecurityStatus, 1);
            }
        }

        [JsonIgnore]
        public Security Security
        {
            get
            {
                if (SecurityStatus >= 0.5)
                {
                    return Security.Highsec;
                }
                else if (SecurityStatus <= 0.4 && SecurityStatus >= 0.01)
                {
                    return Security.Lowsec;
                }
                else if (SecurityStatus <= 0)
                {
                    return Security.Nullsec;
                }

                return Security.Unknown;
            }
        }

        public string DotlanUrl(string regionName)
        {
            return $"https://evemaps.dotlan.net/map/{regionName}/{Name}".Replace(" ", "_");
        }
    }

    public enum Security
    {
        Highsec,
        Lowsec,
        Nullsec,
        Unknown
    }

    public enum SiteType
    {
        Headquarters,
        Assaults,
        Vanguards,
        Staging,
        None
    }
}
#pragma warning restore CS8618