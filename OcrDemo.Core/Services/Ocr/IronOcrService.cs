using OcrDemo.Core.Requests;

namespace OcrDemo.Core.Services.Ocr;

public class IronOcrService : IOcrService
{
  public Task<string> OcrDocumentToText(OcrRequest request) => throw new NotImplementedException();
}
