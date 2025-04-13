namespace OcrDemo.Core.Requests;

public class OcrRequest : DocumentRequest
{
    public required string DocumentType { get; init; }
}