namespace OcrDemo.Core.Responses;

public class ModelsResponseItem
{
  public List<string> SupportedModels { get; set; }
  public string ProviderName { get; set; }
  public string ProviderDisplayName { get; set; }
  public string ServiceType { get; set; }
}
