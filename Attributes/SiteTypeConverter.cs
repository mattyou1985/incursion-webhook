#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
using IncursionWebhook.Models;
using Newtonsoft.Json;

namespace IncursionWebhook.Attributes
{
    public class SiteTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            string? state = (string?)reader.Value;

            return state switch
            {
                "Headquarters"  => SiteType.Headquarters,
                "Assaults"      => SiteType.Assaults,
                "Vanguards"     => SiteType.Vanguards,
                "Staging"       => SiteType.Staging,
                _               => SiteType.None,
            };
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value?.ToString());
        }
    }
}
#pragma warning restore CS8765