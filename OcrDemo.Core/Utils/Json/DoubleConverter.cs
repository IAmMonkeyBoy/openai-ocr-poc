using System.Text.Json;
using System.Text.Json.Serialization;

namespace OcrDemo.Core.Utils.Json;

public class DoubleConverter : JsonConverter<double>
{
  public override double Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    switch (reader.TokenType)
    {
      case JsonTokenType.String:
        {
          var str = reader.GetString();
          if (string.IsNullOrWhiteSpace(str))
            throw new JsonException("Cannot parse double from empty string.");

          if (double.TryParse(str, out var value))
            return value;

          throw new JsonException($"Cannot parse double from string '{str}'.");
        }
      case JsonTokenType.Number:
        return reader.GetDouble();
      default:
        throw new JsonException($"Unexpected token parsing double. Token: {reader.TokenType}");
    }
  }

  public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
  {
    writer.WriteNumberValue(value);
  }
}
