using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using OcrDemo.Core.Requests;
using OcrDemo.Core.Utils;
using OpenAI;
using OpenAI.Chat;

namespace OcrDemo.Core.Services.Document.Structuring;

public class OpenAiStructuredDocumentService(
  ILogger<OpenAiStructuredDocumentService> logger,
  OpenAIClient openAiClient)
  : IStructuredDocumentService
{
  private readonly ILogger<OpenAiStructuredDocumentService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
  private readonly OpenAIClient _openAiClient = openAiClient ?? throw new ArgumentNullException(nameof(openAiClient));


  public async Task<T?> OcrDocument<T>(OcrRequest request)
  {
    var bytes = request.FileContent.ToByteArray();
    var inferredImageType = bytes.InferImageTypeFromBytes();
    var messages = new List<ChatMessage>
    {
      new SystemChatMessage(GeneratePrompt(typeof(T))),
      new UserChatMessage(ChatMessageContentPart.CreateImagePart(new BinaryData(bytes), inferredImageType)),
      new UserChatMessage($"""
                           Please generate a {typeof(T).TypeNameToRealName()} from the supplied image in the specified 
                           format.  Please be sure that when working with dates and date times, you follow the 
                           format specified.  In particular date-time should be provided as specified in RFC 3339 
                           Section 5.6 format.
                           """)
    };
    var responseSchema = typeof(T).ToJsonSchema();
    var options = new ChatCompletionOptions
    {
      ResponseFormat =
        ChatResponseFormat.CreateJsonSchemaFormat(nameof(T), BinaryData.FromString(responseSchema)),
      Temperature = 0.7F,
      MaxOutputTokenCount = 1000,
    };

    var result = await _openAiClient.GetChatClient(request.Model).CompleteChatAsync(messages, options);
    var outputAsText = result.Value.Content.FirstOrDefault()?.Text ?? string.Empty;
    return JsonSerializer.Deserialize<T>(outputAsText, new JsonSerializerOptions() {NumberHandling = JsonNumberHandling.AllowReadingFromString});
  }

  public string LLMName { get; set; } = "OpenAI Chat";
  public string Description { get; set; } = "OpenAI";

  
  public Task<List<LLMModel>> GetModels() => Task.FromResult(new List<LLMModel>
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
 


  private static string GeneratePrompt(Type type)
  {
    var documentTypeName = "";
    var prompt = $"""
                  You are an AI assistant of performing OCR on documents, and then transcribing the data into a 
                  structured format.  Please analyze the following {documentTypeName} document and extract the 
                  relevant information into the structured format below.
                  """;
    return prompt;
  }
}
