namespace OcrDemo.Core;

public class DocumentRequest
{
    public required string FileName { get; init; }
    public required Stream FileContent { get; init; }
    public required string Model { get; init; }
}
