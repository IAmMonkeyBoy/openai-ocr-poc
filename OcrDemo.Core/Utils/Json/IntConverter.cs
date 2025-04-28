using System.Text.Json;
using System.Text.Json.Serialization;

namespace OcrDemo.Core.Utils.Json;

public class IntConverter : JsonConverter<int>
{
  public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    switch (reader.TokenType)
    {
      case JsonTokenType.String:
        {
          var str = reader.GetString();
          if (string.IsNullOrWhiteSpace(str))
            throw new JsonException("Cannot parse int from empty string.");

          if (int.TryParse(str, out var value))
            return value;

          throw new JsonException($"Cannot parse int from string '{str}'.");
        }
      case JsonTokenType.Number:
        return reader.GetInt32();
      default:
        throw new JsonException($"Unexpected token parsing int. Token: {reader.TokenType}");
    }
  }

  public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
  {
    writer.WriteNumberValue(value);
  }
}
