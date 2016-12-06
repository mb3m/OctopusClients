using System;
using System.Linq;
using Newtonsoft.Json;

namespace Octopus.Client.Serialization
{
    public class TotalHoursTimeSpanConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var ts = (TimeSpan)value;
            var tsString = $"{Math.Floor(ts.TotalHours).ToString("00")}:{ts.Minutes.ToString("00")}:{0.ToString("00")}";
            serializer.Serialize(writer, tsString);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            var value = serializer.Deserialize<string>(reader);
            if (value.Contains("."))
            {
                // Then we've received a .NET TimeSpan > 23 hours, we can parse this as a timezone.
                return TimeSpan.Parse(value);
            }
            else
            {
                // We've received a JS timespan in the expected HH:mm:ss format.
                var tsParts = value.Split(':');
                return tsParts.Any(p => int.Parse(p) != 0)
                    ? new TimeSpan(int.Parse(tsParts[0]), int.Parse(tsParts[1]), int.Parse(tsParts[2]))
                    : TimeSpan.Zero;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TimeSpan);
        }
    }
}
