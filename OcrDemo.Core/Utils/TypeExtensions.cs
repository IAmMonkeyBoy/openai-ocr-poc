using System.Text.Json;
using System.Text.Json.Schema;

namespace OcrDemo.Core.Utils;

public static class TypeExtensions
{
  
  public static string TypeNameToRealName(this Type type)
  {
    var typeName = type.Name;
    return string.Concat(typeName.Select((x, i) => i > 0 && char.IsUpper(x) ? " " + x : x.ToString())).ToLower();
  }

  public static string ToJsonSchema(this Type type)
  {
    var options = JsonSerializerOptions.Default;
    var jsonSchemaExporterOptions = new JsonSchemaExporterOptions { TreatNullObliviousAsNonNullable = true };
    var schemaNode = options.GetJsonSchemaAsNode(type, jsonSchemaExporterOptions);
    return schemaNode.ToString();
  }
}
