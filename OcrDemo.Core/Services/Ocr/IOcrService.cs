using OcrDemo.Core.Requests;

namespace OcrDemo.Core.Services.Ocr;

public interface IOcrService
{
  Task<string> OcrDocumentToText(OcrRequest request);
}
