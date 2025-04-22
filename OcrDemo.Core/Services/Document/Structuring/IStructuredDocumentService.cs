using OcrDemo.Core.Requests;

namespace OcrDemo.Core.Services.Document.Structuring;

public interface IStructuredDocumentService
{

    Task<T?> OcrDocument<T>(OcrRequest request);
}
