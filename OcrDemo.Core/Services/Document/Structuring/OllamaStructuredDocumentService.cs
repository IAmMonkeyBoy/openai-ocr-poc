using System.Text;
using System.Text.Json;
using System.Text.Json.Schema;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using OcrDemo.Core.Requests;
using OcrDemo.Core.Utils;
using OllamaSharp;
using OllamaSharp.Models;
using OllamaSharp.Models.Chat;

namespace OcrDemo.Core.Services.Document.Structuring;

public class OllamaStructuredDocumentService(ILogger<OllamaStructuredDocumentService> logger, IOllamaApiClient client)
  : IStructuredDocumentService
{
  private readonly ILogger<OllamaStructuredDocumentService> _logger =
    logger ?? throw new ArgumentNullException(nameof(logger));

  private readonly IOllamaApiClient _client = client ?? throw new ArgumentNullException(nameof(client));

  public async Task<T?> OcrDocument<T>(OcrRequest request)
  {
    using var memoryStream = new MemoryStream();

    await request.FileContent.CopyToAsync(memoryStream);

    var imageString = Convert.ToBase64String(memoryStream.ToArray());


    ChatRequest chatRequest = new ChatRequest()
    {
      Format = JsonSerializerOptions.Default.GetJsonSchemaAsNode(typeof(T)),
      Model = "llava-llama3",
      Messages = new[]
      {
        new Message(ChatRole.System,
          $"""
             You are an AI assistant of performing OCR on documents, and then transcribing 
             the data into a structured format.                 
           """),
        new Message(ChatRole.User,
          $"""
             Please generate a {typeof(T).TypeNameToRealName()} from the supplied image in the specified 
             format.  Please be sure that when working with dates and date times, you follow the 
             format specified.  In particular date-time should be provided as specified in RFC 3339 
             Section 5.6 format.             
           """,
          [imageString]
        )
      }
    };
    var responseBuilder = new StringBuilder();
    await foreach (var response in _client.ChatAsync(chatRequest))
    {
      if (response?.Message?.Content != null)
      {
        responseBuilder.Append(response.Message.Content);
        _logger.LogInformation(responseBuilder.ToString());
      }
      else
      {
        _logger.LogInformation("{role}:{content} - done: {done}", response?.Message?.Role, response?.Message?.Content, response?.Done);
      }
    }
    _logger.LogInformation(responseBuilder.ToString());
    
    var outputAsText = responseBuilder.ToString();
    T result = JsonSerializer.Deserialize<T>(outputAsText, new JsonSerializerOptions
    {
      NumberHandling = JsonNumberHandling.AllowReadingFromString
    }) ?? throw new InvalidOperationException("Deserialization returned null.");
    return result;
  }
}

