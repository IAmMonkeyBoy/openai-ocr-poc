using OcrDemo.Core.Requests;

namespace OcrDemo.Core.Services;

public interface IOcrService
{
  Task<string> OcrDocumentToText(OcrRequest request);
}
