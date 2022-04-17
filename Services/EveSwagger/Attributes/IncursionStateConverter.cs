using IncursionWebhook.Services.EveSwagger.Models;
using Newtonsoft.Json;

namespace IncursionWebhook.Services.EveSwagger.Attributes
{
    public class IncursionStateConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            string state = (string)reader.Value;

            return state switch
            {
                "established" => State.Established,
                "mobilizing" => State.Mobilizing,
                "withdrawing" => State.Withdrawing,
                _ => State.Unknown,
            };
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value?.ToString()?.ToLower());
        }
    }
}
