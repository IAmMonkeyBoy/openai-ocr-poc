namespace OcrDemo.Core;

public class DocumentRequest
{
    public required string FileName { get; init; }
    public required Stream FileContent { get; init; }
    public string? RequestedModel { get; init; }
    public string? ProviderName { get; init; }
}
