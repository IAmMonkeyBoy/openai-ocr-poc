using System.Text.Json;
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
      MaxOutputTokenCount = 1000
    };

    var result = await _openAiClient.GetChatClient("gpt-4o").CompleteChatAsync(messages, options);
    var outputAsText = result.Value.Content.FirstOrDefault()?.Text ?? string.Empty;
    return JsonSerializer.Deserialize<T>(outputAsText, new JsonSerializerOptions());
  }

  public Task<List<string>> GetAvailableModels() => Task.FromResult(new List<string>(){"gpt-4o", "gpt-4o-mini", "gpt-3.5-turbo"});
  public string GetProviderName() => "OpenAiLibrary";
  public string GetProviderDisplayName() => "OpenAI";


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
