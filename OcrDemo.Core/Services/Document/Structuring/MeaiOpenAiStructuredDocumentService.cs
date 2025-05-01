using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace OcrDemo.Core.Services.Document.Structuring;

public class MeaiOpenAiStructuredDocumentService(
  ILogger<MeaiOpenAiStructuredDocumentService> logger,
  IChatClient chatClient) : MeaiStructuredDocumentServiceBase(logger, chatClient)
{
  public override string LLMName { get; set; } = "MEAI OpenAI Structured Document Service";
  public override string Description { get; set; } = "MeaiOpenAiStructuredDocumentService is a base class for structured document services using Microsoft.Extensions.AI.";
  public override Task<List<LLMModel>> GetModels() => Task.FromResult(new List<LLMModel>
  {
    new LLMModel
    {
      Name = "gpt-4o",
      Description = "GPT-4o is a multimodal model that can process both text and images.",
      IsDefault = true
    },
    new LLMModel
    {
      Name = "gpt-4o-mini",
      Description = "GPT-4o-mini is a smaller version of GPT-4o that is optimized for speed and efficiency."
    },
    new LLMModel
    {
      Name = "gpt-4o-nano",
      Description = ""
    },
    new LLMModel
    {
      Name = "o4-mini",
      Description = ""
    },
    new LLMModel
    {
      Name = "gpt-4.1",
      Description = ""
    },
    new LLMModel
    {
      Name = "gpt-4.1-mini",
      Description = ""
    },
    new LLMModel
    {
      Name = "gpt-4.1-nano",
      Description = ""
    }
  });

}
