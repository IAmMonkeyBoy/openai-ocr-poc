using System.Text.Json;
using System.Text.Json.Serialization;

namespace OcrDemo.Core.Utils.Json;

public class DecimalConverter : JsonConverter<decimal>
{
  public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    switch (reader.TokenType)
    {
      case JsonTokenType.String:
        {
          var str = reader.GetString();
          if (string.IsNullOrWhiteSpace(str))
            throw new JsonException("Cannot parse decimal from empty string.");

          if (decimal.TryParse(str, out var value))
            return value;

          throw new JsonException($"Cannot parse decimal from string '{str}'.");
        }
      case JsonTokenType.Number:
        return reader.GetDecimal();
      default:
        throw new JsonException($"Unexpected token parsing decimal. Token: {reader.TokenType}");
    }
  }

  public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
  {
    writer.WriteNumberValue(value);
  }
}
