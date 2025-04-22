using System.Reflection;
using OcrDemo.Core.Models;

namespace OcrDemo.Core.Utils;

public static class OcrDocumentExtensions
{
public static List<FieldEvaluation> EvaluateOcrFieldResults(this object obj, string parentPath = "", HashSet<object> visited = null)
  {
      var evaluations = new List<FieldEvaluation>();
      visited ??= new HashSet<object>();
  
      // Prevent infinite recursion by skipping already visited objects
      if (obj == null || visited.Contains(obj))
          return evaluations;
  
      visited.Add(obj);
  
      var properties = obj.GetType().GetProperties();
  
      foreach (var property in properties)
      {
          var priorityAttribute = property.GetCustomAttribute<FieldPriorityAttribute>();
          var value = property.GetValue(obj);
          var fieldPath = string.IsNullOrEmpty(parentPath) ? property.Name : $"{parentPath}.{property.Name}";
  
          if (priorityAttribute != null)
          {
              evaluations.Add(new FieldEvaluation
              {
                  FieldName = fieldPath,
                  Priority = priorityAttribute.Priority,
                  IsPresent = value != null,
                  Value = value?.ToString() ?? String.Empty
              });
          }
  
          // Handle collections (e.g., lists, arrays)
          if (value is IEnumerable<object> collection)
          {
              int index = 0;
              foreach (var item in collection)
              {
                  var collectionPath = $"{fieldPath}[{index}]";
                  evaluations.AddRange(item.EvaluateOcrFieldResults(collectionPath, visited));
                  index++;
              }
          }
          // Handle nested classes
          else if (value != null && typeof(IStructuredDocument).IsAssignableFrom(property.PropertyType))
          {
            evaluations.AddRange(value.EvaluateOcrFieldResults(fieldPath, visited));
          }
      }
  
      return evaluations;
  }
}
