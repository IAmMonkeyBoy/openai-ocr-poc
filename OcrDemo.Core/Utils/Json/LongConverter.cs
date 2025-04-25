using System.Text.Json;
using System.Text.Json.Serialization;

namespace OcrDemo.Core.Utils.Json;

public class LongConverter : JsonConverter<long>
{
  public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    switch (reader.TokenType)
    {
      case JsonTokenType.String:
        {
          var str = reader.GetString();
          if (string.IsNullOrWhiteSpace(str))
            throw new JsonException("Cannot parse long from empty string.");

          if (long.TryParse(str, out var value))
            return value;

          throw new JsonException($"Cannot parse long from string '{str}'.");
        }
      case JsonTokenType.Number:
        return reader.GetInt64();
      default:
        throw new JsonException($"Unexpected token parsing long. Token: {reader.TokenType}");
    }
  }

  public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
  {
    writer.WriteNumberValue(value);
  }
}
