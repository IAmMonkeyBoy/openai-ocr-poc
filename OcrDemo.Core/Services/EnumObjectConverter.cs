using System.Text.Json;
using System.Text.Json.Serialization;

namespace OcrDemo.Core.Services;

public class EnumObjectConverter<T> : JsonConverter<T> where T : struct, Enum
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.StartObject)
        {
            // Read the object
            using (JsonDocument document = JsonDocument.ParseValue(ref reader))
            {
                var root = document.RootElement;

                // Check if the object has "Value" property
                if (root.TryGetProperty("Value", out var valueProperty))
                {
                    // Convert the "Value" property to the enum
                    if (Enum.TryParse(valueProperty.GetString(), out T result))
                    {
                        return result;
                    }
                }
            }
        }
        else if (reader.TokenType == JsonTokenType.String)
        {
            if (Enum.TryParse<T>(reader.GetString(), out var result))
            {
                return result;
            }
        }
        throw new JsonException($"Unable to deserialize {typeof(T).Name} from JSON.");
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        // Serialize the enum as an object with "HasValue" and "Value" properties
        writer.WriteStartObject();
        writer.WriteBoolean("HasValue", true);
        writer.WriteString("Value", value.ToString());
        writer.WriteEndObject();
    }
}