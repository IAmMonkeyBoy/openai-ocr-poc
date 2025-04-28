namespace OcrDemo.Api.Requests;

public class GenerateStructuredDocumentRequest
{
  public string StructuredDocumentServiceId { get; set; }
  public string Model { get; set; }
  public string OcrServiceId { get; set; }
}
