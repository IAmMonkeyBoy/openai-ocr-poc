using System.Text.Json;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OcrDemo.Core.Requests;
using OcrDemo.Core.Utils;

namespace OcrDemo.Core.Services.Document.Structuring;

public class MsStructuredDocumentServiceBase(IChatClient chatClient, ILogger<MsStructuredDocumentServiceBase> logger) :IStructuredDocumentService
{
  protected readonly IChatClient _chatClient = chatClient ?? throw new ArgumentNullException(nameof(chatClient));
  protected readonly ILogger<MsStructuredDocumentServiceBase> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
  
  public async Task<T?> OcrDocument<T>(OcrRequest request)
  {
    var bytes = request.FileContent.ToByteArray();
    var messages = new List<ChatMessage>()
    {
      new ChatMessage(ChatRole.System, $"""
                                        You are an AI assistant of performing OCR on documents, and then transcribing the data into a 
                                        structured format.  Please analyze the following {typeof(T).TypeNameToRealName()} document and extract the 
                                        relevant information into the structured format below.
                                        """),
      new ChatMessage(ChatRole.User, new List<AIContent>(){new DataContent(new BinaryData(bytes), bytes.InferImageTypeFromBytes())}),
      new ChatMessage(ChatRole.User, $"""
                                        Please generate a {typeof(T).TypeNameToRealName()} from the supplied image in the specified 
                                        format.  Please be sure that when working with dates and date times, you follow the 
                                        format specified.  In particular date-time should be provided as specified in RFC 3339 
                                        Section 5.6 format.
                                        """)
    };
    var options = new ChatOptions()
    {
      ResponseFormat = ChatResponseFormat.ForJsonSchema(typeof(T).ToJsonSchemaJsonElement(), nameof(T)),
      Temperature = 0.7F,
      MaxOutputTokens = 1000
      
    };
    var response = await _chatClient.GetResponseAsync<T>(messages, options);
    return response.Result;
  }
  
}



public class MsOpenAiStructuredDocumentService(
  [FromKeyedServices(ComponentRegistration.ChatProvider.OpenAi)]IChatClient openAiClient,
  ILogger<MsOpenAiStructuredDocumentService> logger)
  : MsStructuredDocumentServiceBase(openAiClient, logger), IStructuredDocumentService;

public class OllamaStructuredDocumentService(
  [FromKeyedServices(ComponentRegistration.ChatProvider.Ollama)]IChatClient chatClient, ILogger<OllamaStructuredDocumentService> logger) :
  MsStructuredDocumentServiceBase(chatClient, logger),
  IStructuredDocumentService;
