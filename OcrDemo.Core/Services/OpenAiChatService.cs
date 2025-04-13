using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Schema;
using Betalgo.Ranul.OpenAI.Interfaces;
using Betalgo.Ranul.OpenAI.ObjectModels.RequestModels;
using Microsoft.Extensions.Logging;
using OcrDemo.Core.Requests;
using OcrDemo.Core.Responses;

namespace OcrDemo.Core.Services;

public interface IOpenAiChatService
{
    Task<IdentifyDocumentResponse> IdentifyDocument(DocumentRequest request);
    Task<OcrInvoiceResponse> OcrInvoice(OcrRequest request);
    Task<OcrBillOfLadingResponse> OcrBillOfLading(OcrRequest request);
    Task<OcrFuelReceiptResponse> OcrFuelReceipt(OcrRequest request);
    Task<OcrRateConfirmationResponse> OcrRateConfirmation(OcrRequest request);
}

public class OpenAiChatService : IOpenAiChatService
{
    private readonly ILogger<OpenAiChatService> _logger;
    private readonly IOpenAIService _openAiService;

    public OpenAiChatService(
        ILogger<OpenAiChatService> logger,
        IOpenAIService openAiService
    )
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _openAiService = openAiService ?? throw new ArgumentNullException(nameof(openAiService));
    }

    public async Task<IdentifyDocumentResponse> IdentifyDocument(DocumentRequest request)
    {
        byte[] bytes = GetFileBytes(request.FileContent);
        string inferredImageType = InferImageTypeFromBytes(bytes);
        
        var response = await _openAiService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
        {
            Messages = new List<ChatMessage>
            {
                new ChatMessage
                {
                    Role = "system",
                    Content = """
                              You are an AI assistant tasked with identifying the type of document based on its content.
                              The possible document types are: "Bill of Lading", "Invoice", "Rate Confirmation", and "Fuel Receipt".
                              If you cannot determine the type, respond with "Unknown".
                              """
                },
                new ChatMessage()
                {
                    Role = "user",
                    Contents = new List<MessageContent>(){ MessageContent.ImageBinaryContent(bytes, inferredImageType)}
                },
                new ChatMessage
                {
                    Role = "user",
                    Content = $"""
                               What is the document type?
                               """
                }
            },
            MaxTokens = 10,
            Temperature = 0.7F,
            Model = "gpt-4o"
        });

        var documentType = response.Choices.FirstOrDefault()?.Message.Content?.Trim() ?? "Unknown";

        return new IdentifyDocumentResponse
        {
            
            DocumentType = documentType
        };
    }

    private string InferImageTypeFromBytes(byte[] bytes)
    {
        
        if (bytes.Length < 4)
            throw new ArgumentException("Byte array is too small to determine the image type.");
    
        // Check for PNG
        if (bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47)
            return "png";
    
        // Check for JPEG
        if (bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF)
            return "jpeg";
    
        // Check for GIF
        if (bytes[0] == 0x47 && bytes[1] == 0x49 && bytes[2] == 0x46)
            return "gif";
    
        // Check for WEBP
        if (bytes[0] == 0x52 && bytes[1] == 0x49 && bytes[2] == 0x46 && bytes[3] == 0x46)
            return "webp";
    
        throw new NotSupportedException("Unsupported image type.");
    }


    private byte[] GetFileBytes(Stream requestFileContent)
    {
        using MemoryStream memoryStream = new MemoryStream();
        requestFileContent.CopyTo(memoryStream);
        return memoryStream.ToArray();
    }

    private static async Task<StringBuilder> Base64EncodedFileContents(Stream stream)
    {
        StringBuilder base64EncodedFileContents = new StringBuilder();


        while (stream.CanRead)
        {
            byte[] buffer = new byte[4096];
            int bytesRead = await stream.ReadAsync(buffer.AsMemory(0, buffer.Length));
            if (bytesRead == 0)
                break;
            base64EncodedFileContents.Append(Convert.ToBase64String(buffer, 0, bytesRead));
        }

        return base64EncodedFileContents;
    }

    public Task<OcrInvoiceResponse> OcrInvoice(OcrRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<OcrBillOfLadingResponse> OcrBillOfLading(OcrRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<OcrFuelReceiptResponse> OcrFuelReceipt(OcrRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task<OcrRateConfirmationResponse> OcrRateConfirmation(OcrRequest request)
    {
        byte[] bytes = GetFileBytes(request.FileContent);
        string inferredImageType = InferImageTypeFromBytes(bytes);
        
        var prompt = GeneratePrompt(typeof(RateConfirmation));

        var response = await _openAiService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
        {
            Messages = new List<ChatMessage>
            {
                new ChatMessage
                {
                    Role = "system",
                    Content = prompt
                },
                new ChatMessage()
                {
                    Role = "user",
                    Contents = new List<MessageContent>()
                        { MessageContent.ImageBinaryContent(bytes, inferredImageType) }
                }

            },
            ResponseFormat = new ResponseFormat
            {

                JsonSchema = GenerateJsonSchema(typeof(RateConfirmation))
            },
            MaxTokens = 1000,
            Temperature = 0.7F,
            Model = "gpt-4o"
        });
        return new OcrRateConfirmationResponse(
                JsonSerializer.Deserialize<RateConfirmation>(response.Choices.FirstOrDefault()?.Message.Content ?? "{}"))
            ;

    }



    private string GeneratePrompt(Type type)
    {
        string documentTypeName = "";
        string prompt = $"""
                         You are an AI assistant of performing OCR on documents, and then transcribing the data into a 
                         structured format.  Please analyze the following {documentTypeName} document and extract the 
                         relevant information into the structured format below.


                         """;
        string schema = GenerateJsonSchemaString(type);

        return prompt + schema;
    }


    private string GenerateJsonSchemaString(Type type)
    {
        
        JsonSerializerOptions options = JsonSerializerOptions.Default;
        return options.GetJsonSchemaAsNode(type).ToString();
        
        
        
    }

    
    private JsonSchema GenerateJsonSchema(Type type)
    {

        return new JsonSchema();
    }

}
