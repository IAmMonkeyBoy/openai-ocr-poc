using System.Text.Json;
using System.Text.Json.Serialization;
using Anthropic;
using Microsoft.Extensions.Logging;
using OcrDemo.Core.Requests;
using OcrDemo.Core.Utils;

namespace OcrDemo.Core.Services.Document.Structuring;

public class AnthropicStructuredDocumentService(
  ILogger<AnthropicStructuredDocumentService> logger,
  AnthropicClient anthropicClient)
  : IStructuredDocumentService
{
  private readonly ILogger<AnthropicStructuredDocumentService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
  private readonly AnthropicClient _anthropicClient = anthropicClient ?? throw new ArgumentNullException(nameof(anthropicClient));

  public async Task<T?> OcrDocument<T>(OcrRequest request)
  {
    var bytes = request.FileContent.ToByteArray();
    var inferredImageType = bytes.InferImageTypeFromBytes();
    
    // Create messages for the Anthropic API
    var messages = new List<InputMessage>();
    
    // Add user message with image
    var imageBlock = new InputContentBlock(new RequestImageBlock
    {
        Source = new Base64ImageSource
        {
            MediaType = inferredImageType switch
            {
                "image/png" => Base64ImageSourceMediaType.ImagePng,
                "image/gif" => Base64ImageSourceMediaType.ImageGif,
                "image/webp" => Base64ImageSourceMediaType.ImageWebp,
                _ => Base64ImageSourceMediaType.ImageJpeg,
            },
            Data = bytes,
            Type = Base64ImageSourceType.Base64,
        }
    });
    
    // Add user message with image and instruction
    var userMessage = new InputMessage
    {
        Role = InputMessageRole.User,
        Content = new List<InputContentBlock> 
        { 
            imageBlock,
            new InputContentBlock(new RequestTextBlock
            {
                Text = "Please analyze this document and extract the relevant information and provide the output "
            })
        }
    };
    messages.Add(userMessage);

    var responseSchema = typeof(T).ToJsonSchema();
    
    // Create the tool definition
    var tool = new Tool
    {
        Name = "extract_document_info",
        Description = $"Extract information from the document into a structured {typeof(T).Name} format",
        InputSchema = responseSchema
    };

    // Create message request
    var createMessageRequest = new CreateMessageParams
    {
        Model = new Model(ModelVariant6.Claude35SonnetLatest),
        MaxTokens = 1000,
        System = "You are an AI assistant specialized in extracting structured information from documents. " +
                "Use the provided tool to output the extracted information in the required format.",
        Messages = messages,
        Tools = new List<OneOf<Tool, BashTool20250124, TextEditor20250124>> { tool },
        ToolChoice = new ToolChoice { Type = "function", Name = "extract_document_info" }
    };

    // Call the API
    var response = await _anthropicClient.Messages.MessagesPostAsync(createMessageRequest);
    
    // Extract the tool use content
    foreach (var block in response.Content)
    {
        if (block.Type == "tool_use" && block.Name == "extract_document_info")
        {
            var jsonString = JsonSerializer.Serialize(block.Input);
            return JsonSerializer.Deserialize<T>(jsonString, new JsonSerializerOptions() 
            { 
                NumberHandling = JsonNumberHandling.AllowReadingFromString 
            });
        }
    }
    
    return default;
  }

  public string LLMName { get; set; } = "Anthropic Claude";
  public string Description { get; set; } = "Anthropic";

  public async Task<List<LLMModel>> GetModels()
  {
    var modelList = await _anthropicClient.ModelsListAsync();
    
    return modelList.Data
      .Select(model => new LLMModel
      {
        Name = model.Id,
        Description = model.Id,
        // Set Claude 3 Opus as default if available
        IsDefault = model.Id.Contains("claude-3-opus", StringComparison.OrdinalIgnoreCase)
      })
      .ToList();
  }

  private static string GeneratePrompt(Type type)
  {
    return $"""
           You are an AI assistant of performing OCR on documents, and then transcribing the data into a 
           structured format.  Please analyze the following document and extract the 
           relevant information into the structured format below.
           """;
  }
}
