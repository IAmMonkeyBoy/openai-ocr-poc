using System.Text.Json;
using System.Text.Json.Schema;
using Microsoft.Extensions.Logging;
using OcrDemo.Core.Models;
using OcrDemo.Core.Requests;
using OcrDemo.Core.Responses;
using OpenAI;
using OpenAI.Chat;
// using Betalgo.Ranul.OpenAI.Interfaces;
// using Betalgo.Ranul.OpenAI.ObjectModels;
// using Betalgo.Ranul.OpenAI.ObjectModels.RequestModels;
// using Betalgo.Ranul.OpenAI.ObjectModels.SharedModels;

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
    private readonly OpenAIClient _openAiClient;

    public OpenAiChatService(
        ILogger<OpenAiChatService> logger,
        OpenAIClient openAiClient
    )
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _openAiClient = openAiClient ?? throw new ArgumentNullException(nameof(openAiClient));
    }

    public async Task<IdentifyDocumentResponse> IdentifyDocument(DocumentRequest request)
    {
        var bytes = GetFileBytes(request.FileContent);
        var inferredImageType = InferImageTypeFromBytes(bytes);

        var response = await _openAiClient.GetChatClient("gpt-4o").CompleteChatAsync(
            new SystemChatMessage("""
                                  You are an AI assistant tasked with identifying the type of document based on its content.
                                  The possible document types are: "Bill of Lading", "Invoice", "Rate Confirmation", and "Fuel Receipt".
                                  If you cannot determine the type, respond with "Unknown".
                                  """),
            new UserChatMessage(ChatMessageContentPart.CreateImagePart(new BinaryData(bytes), inferredImageType)),
            new UserChatMessage("What is the document type?  Respond with only the document type name")
        );

        return new IdentifyDocumentResponse
        {
            DocumentType = response?.Value.Content.FirstOrDefault()?.Text ?? "Unknown"
        };
    }

    public async Task<OcrRateConfirmationResponse> OcrRateConfirmation(OcrRequest request)
    {
        return new OcrRateConfirmationResponse(await OcrDocument<RateConfirmation>(request));
    }

    public async Task<OcrInvoiceResponse> OcrInvoice(OcrRequest request)
    {
        return new OcrInvoiceResponse(await OcrDocument<Invoice>(request));
    }

    public async Task<OcrBillOfLadingResponse> OcrBillOfLading(OcrRequest request)
    {
        return new OcrBillOfLadingResponse(await OcrDocument<BillOfLading>(request));
    }

    public async Task<OcrFuelReceiptResponse> OcrFuelReceipt(OcrRequest request)
    {
        return new OcrFuelReceiptResponse(await OcrDocument<FuelReceipt>(request));
    }

    private static string InferImageTypeFromBytes(byte[] bytes)
    {
        if (bytes.Length < 4)
            throw new ArgumentException("Byte array is too small to determine the image type.");

        // Check for PNG
        if (bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47)
            return "image/png";

        // Check for JPEG
        if (bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF)
            return "image/jpeg";

        // Check for GIF
        if (bytes[0] == 0x47 && bytes[1] == 0x49 && bytes[2] == 0x46)
            return "image/gif";

        // Check for WEBP
        if (bytes[0] == 0x52 && bytes[1] == 0x49 && bytes[2] == 0x46 && bytes[3] == 0x46)
            return "image/webp";

        throw new NotSupportedException("Unsupported image type.");
    }


    private static byte[] GetFileBytes(Stream requestFileContent)
    {
        using var memoryStream = new MemoryStream();
        requestFileContent.CopyTo(memoryStream);
        return memoryStream.ToArray();
    }


    private static string TypeNameToRealName<T>()
    {
        var typeName = typeof(T).Name;
        return string.Concat(typeName.Select((x, i) => i > 0 && char.IsUpper(x) ? " " + x : x.ToString())).ToLower();
    }

    protected async Task<T?> OcrDocument<T>(OcrRequest request)
    {
        var bytes = GetFileBytes(request.FileContent);
        var inferredImageType = InferImageTypeFromBytes(bytes);
        var messages = new List<ChatMessage>
        {
            new SystemChatMessage(GeneratePrompt(typeof(T))),
            new UserChatMessage(ChatMessageContentPart.CreateImagePart(new BinaryData(bytes), inferredImageType)),
            new UserChatMessage($"""
                                 Please generate a {TypeNameToRealName<T>()} from the supplied image in the specified 
                                 format.  Please be sure that when working with dates and date times, you follow the 
                                 format specified.  In particular date-time should be provided as specified in RFC 3339 
                                 Section 5.6 format.
                                 """)
        };

        var responseSchema = GenerateJsonSchemaString(typeof(T));
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


    private static string GenerateJsonSchemaString(Type type)
    {
        var options = JsonSerializerOptions.Default;
        var jsonSchemaExporterOptions = new JsonSchemaExporterOptions { TreatNullObliviousAsNonNullable = true };
        var schemaNode = options.GetJsonSchemaAsNode(type, jsonSchemaExporterOptions);
        return schemaNode.ToString();
    }
}