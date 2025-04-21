using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using OcrDemo.Core.Requests;
using OpenAI;

namespace OcrDemo.Core.Services.Document.Structuring;

public interface IStructuredDocumentService
{

    Task<T?> OcrDocument<T>(OcrRequest request);
}

