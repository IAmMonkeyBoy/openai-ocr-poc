using OcrDemo.Core.Models;

namespace OcrDemo.Api.Responses;

public class GetLLMResponseItem
{
  public string Name { get; set; }
  public string Description { get; set; }
  public string ServiceId { get; set; }
  public List<GetLLMResponseItemModel> Models { get; set; } = new List<GetLLMResponseItemModel>();
}

public class GetLLMResponseItemModel
{
  public string Name { get; set; }
  public string Description { get; set; }
  public string ModelType { get; set; }
  public bool IsDefault { get; set; }
}

public class GetOcrResponseItem
{
  public string Name { get; set; }
  public string Description { get; set; }
  public string ServiceId { get; set; }
}


public class GenerateStructuredDocumentResponse<T>(T? document, OcrDocumentQualityAssessment? qualityAssessment = null)
{
  public T? Document { get; set; } = document;
  public OcrDocumentQualityAssessment? QualityAssessment { get; set; } = qualityAssessment;
}

public class GenerateStructuredDocumentResponseImpl<T>(T? document, OcrDocumentQualityAssessment? qualityAssessment = null) : GenerateStructuredDocumentResponse<T>(document, qualityAssessment)
{
}
