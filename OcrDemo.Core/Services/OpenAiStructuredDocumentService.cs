using System.Text.Json;
using Microsoft.Extensions.Logging;
using OcrDemo.Core.Models;
using OcrDemo.Core.Requests;
using OcrDemo.Core.Responses;
using OcrDemo.Core.Utils;
using OpenAI;
using OpenAI.Chat;

namespace OcrDemo.Core.Services;

public class OpenAiStructuredDocumentService : IStructuredDocumentService
{
  private readonly ILogger<OpenAiStructuredDocumentService> _logger;
  private readonly OpenAIClient _openAiClient;

  public OpenAiStructuredDocumentService(
    ILogger<OpenAiStructuredDocumentService> logger,
    OpenAIClient openAiClient
  )
  {
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    _openAiClient = openAiClient ?? throw new ArgumentNullException(nameof(openAiClient));
  }

  public async Task<IdentifyDocumentResponse> IdentifyDocument(DocumentRequest request)
  {
    var bytes = request.FileContent.ToByteArray();
    var inferredImageType = bytes.InferImageTypeFromBytes();

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
