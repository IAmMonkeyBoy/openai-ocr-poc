using System.Text.Json;
using System.Text.Json.Serialization;

namespace OcrDemo.Core.Services;

public class DateTimeIso8601Converter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            if (DateTime.TryParse(reader.GetString(), out var result))
            {
                return result;
            }
        }
        
        using (JsonDocument document = JsonDocument.ParseValue(ref reader))
        {
            var root = document.RootElement;

            // Check if the object has "Value" property
            if (root.TryGetProperty("Value", out var valueProperty))
            {
                // Convert the "Value" property to the enum
                if (DateTime.TryParse(valueProperty.GetString(), out var result))
                {
                    return result;
                }
            }
        }
        throw new JsonException($"Unable to deserialize DateTime from JSON.");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        
        writer.WriteStartObject();
        writer.WriteBoolean("HasValue", true);
        writer.WriteString("Value", value.ToString("o"));
        writer.WriteEndObject();
    }
}