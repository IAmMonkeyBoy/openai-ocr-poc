using System.Text.Json;
using System.Text.Json.Serialization;

namespace OcrDemo.Core.Utils.Json;

public class NullableLongConverter : JsonConverter<long?>
{
  public override long? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    if (reader.TokenType == JsonTokenType.String)
    {
      var str = reader.GetString();
      if (string.IsNullOrWhiteSpace(str))
        return null;

      if (long.TryParse(str, out var value))
        return value;

      throw new JsonException($"Cannot parse long from string '{str}'.");
    }

    if (reader.TokenType == JsonTokenType.Null)
      return null;

    if (reader.TokenType == JsonTokenType.Number)
      return reader.GetInt64();

    throw new JsonException($"Unexpected token parsing long. Token: {reader.TokenType}");
  }

  public override void Write(Utf8JsonWriter writer, long? value, JsonSerializerOptions options)
  {
    if (value.HasValue)
      writer.WriteNumberValue(value.Value);
    else
      writer.WriteNullValue();
  }
}
