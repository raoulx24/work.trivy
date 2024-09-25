using System.Text.Json;
using System.Text.Json.Serialization;

namespace TrivyOperator.Dashboard.Utils;

public class CustomDateTimeConverter : JsonConverter<DateTime>
{
    private readonly string _format = "yyyy-MM-ddTHH:mm:ssZ";

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? dateString = reader.GetString();
        if (string.IsNullOrWhiteSpace(dateString))
        {
            return DateTime.MinValue;
        }
        return DateTime.ParseExact(dateString, _format, null, System.Globalization.DateTimeStyles.AdjustToUniversal);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(_format));
    }
}
