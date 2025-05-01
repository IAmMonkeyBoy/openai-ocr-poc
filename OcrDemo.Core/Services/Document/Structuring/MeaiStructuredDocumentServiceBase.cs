using System.Text.Json;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using OcrDemo.Core.Requests;
using OcrDemo.Core.Utils;
using OcrDemo.Core.Utils.Json;

namespace OcrDemo.Core.Services.Document.Structuring;

public abstract class MeaiStructuredDocumentServiceBase(
  ILogger<MeaiStructuredDocumentServiceBase> logger,
  IChatClient chatClient) : IStructuredDocumentService
{
  private readonly ILogger<MeaiStructuredDocumentServiceBase> _logger =
    logger ?? throw new ArgumentNullException(nameof(logger));

  private readonly IChatClient _chatClient = chatClient ?? throw new ArgumentNullException(nameof(chatClient));

  public virtual async Task<T?> OcrDocument<T>(OcrRequest request)
  {
    var bytes = request.FileContent.ToByteArray();
    var inferredImageType = bytes.InferImageTypeFromBytes();
    var messages = new List<ChatMessage>
    {
      new ChatMessage(ChatRole.System,
        new List<AIContent>()
        {
          new TextContent(
            $"""
              You are an AI assistant of performing OCR on documents, and then transcribing the data into a 
              structured format.
             """)}),
      new ChatMessage(ChatRole.User,
        new List<AIContent>()
        {
          new DataContent(bytes, inferredImageType),
          new TextContent(
            $"""
              Please generate a {typeof(T).TypeNameToRealName()} from the supplied image in the specified format.  
              Be sure that when working with dates and date times, you follow the format specified.  In particular 
              date-time should be provided as specified in RFC 3339 Section 5.6 format.
             """)
        })
    };

    JsonSerializerOptions  jsonSerializerOptions = new(AIJsonUtilities.DefaultOptions)
    {
      Converters =
      {
        new NullableDecimalConverter(),
        new NullableLongConverter(),
        new NullableFloatConverter(),
        new NullableDoubleConverter(),
        new NullableIntConverter(),
        new DecimalConverter(),
        new LongConverter(),
        new FloatConverter(),
        new DoubleConverter(),
        new IntConverter(),
        
      }
    };
    var schema = AIJsonUtilities.CreateJsonSchema(typeof(T));
    var chatOptions = new ChatOptions 
    {
      Temperature = 0.7F,
      MaxOutputTokens = 1000,
      ResponseFormat = new ChatResponseFormatJson(
        schema, 
        $"{typeof(T).Name}")
    };
    var response =
      await _chatClient.GetResponseAsync<T>(
        messages,
        jsonSerializerOptions,
        chatOptions, true);
    return response.Result;
  }

  public abstract string LLMName { get; set; }
  public abstract string Description { get; set; }
  public abstract Task<List<LLMModel>> GetModels();


}
