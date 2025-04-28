using OcrDemo.Core.Requests;

namespace OcrDemo.Core.Services.Document.Structuring;

public interface IStructuredDocumentService
{

    Task<T?> OcrDocument<T>(OcrRequest request);
    string LLMName { get; set; }
    string Description { get; set; }
    Task<List<LLMModel>> GetModels();
}

public class LLMModel
{
  public string Name { get; set; } = string.Empty;
  public string Description { get; set; }  = string.Empty;
  public string ModelType { get; set; } = string.Empty;
  public bool IsDefault { get; set; } = false;
}
