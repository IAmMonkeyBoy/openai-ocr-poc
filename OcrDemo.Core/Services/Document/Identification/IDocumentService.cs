using OcrDemo.Core.Responses;

namespace OcrDemo.Core.Services.Document.Identification;

public interface IDocumentIdentificationService
{
  Task<IdentifyDocumentResponse> IdentifyDocument(DocumentRequest request);
}
