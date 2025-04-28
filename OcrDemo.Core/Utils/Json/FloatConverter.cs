using System.Text.Json;
using System.Text.Json.Serialization;

namespace OcrDemo.Core.Utils.Json;

public class FloatConverter : JsonConverter<float>
{
  public override float Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    switch (reader.TokenType)
    {
      case JsonTokenType.String:
        {
          var str = reader.GetString();
          if (string.IsNullOrWhiteSpace(str))
            throw new JsonException("Cannot parse float from empty string.");

          if (float.TryParse(str, out var value))
            return value;

          throw new JsonException($"Cannot parse float from string '{str}'.");
        }
      case JsonTokenType.Number:
        return reader.GetSingle();
      default:
        throw new JsonException($"Unexpected token parsing float. Token: {reader.TokenType}");
    }
  }

  public override void Write(Utf8JsonWriter writer, float value, JsonSerializerOptions options)
  {
    writer.WriteNumberValue(value);
  }
}
