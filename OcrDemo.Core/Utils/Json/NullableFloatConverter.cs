using System.Text.Json;
using System.Text.Json.Serialization;

namespace OcrDemo.Core.Utils.Json;

public class NullableFloatConverter : JsonConverter<float?>
{
  public override float? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    if (reader.TokenType == JsonTokenType.String)
    {
      var str = reader.GetString();
      if (string.IsNullOrWhiteSpace(str))
        return null;

      if (float.TryParse(str, out var value))
        return value;

      throw new JsonException($"Cannot parse float from string '{str}'.");
    }

    if (reader.TokenType == JsonTokenType.Null)
      return null;

    if (reader.TokenType == JsonTokenType.Number)
      return reader.GetSingle();

    throw new JsonException($"Unexpected token parsing float. Token: {reader.TokenType}");
  }

  public override void Write(Utf8JsonWriter writer, float? value, JsonSerializerOptions options)
  {
    if (value.HasValue)
      writer.WriteNumberValue(value.Value);
    else
      writer.WriteNullValue();
  }
}
