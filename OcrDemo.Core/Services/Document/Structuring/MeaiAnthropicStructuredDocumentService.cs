using Anthropic;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OcrDemo.Core.Requests;

namespace OcrDemo.Core.Services.Document.Structuring;

public class MeaiAnthropicStructuredDocumentService(
  ILogger<MeaiAnthropicStructuredDocumentService> logger,
  IChatClient chatClient,
  AnthropicClient anthropicClient) : MeaiStructuredDocumentServiceBase(logger, anthropicClient)
{
  private readonly ILogger<MeaiAnthropicStructuredDocumentService> _logger = logger;

  private readonly AnthropicClient _anthropicClient = anthropicClient;


  public override string LLMName { get; set; } = "MEAI Anthropic Structured Document Service";
  public override string Description { get; set; } = "MeaiAnthropicStructuredDocumentService is a base class for structured document services using Microsoft.Extensions.AI with Anthropic models.";

  public override async Task<List<LLMModel>> GetModels()
  {
    var modelList = await _anthropicClient.ModelsListAsync();
    
   return  modelList.Data
      .Select(model => new LLMModel
      {
        Name = model.Id,
        Description = model.Id,
        IsDefault =  false
      })
      .ToList();
  }
}
