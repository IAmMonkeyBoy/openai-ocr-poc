namespace OcrDemo.Core.Models;

public class FieldEvaluation
{
  public required string FieldName { get; set; }
  public FieldPriority Priority { get; set; }
  public bool IsPresent { get; set; }
  public string Value { get; set; }
}

public enum FieldPriority
{
  High,
  Medium,
  Low,
  NotImportant
}
[AttributeUsage(AttributeTargets.Property)]
public class FieldPriorityAttribute(FieldPriority priority) : Attribute
{
    public FieldPriority Priority { get; } = priority;
}

