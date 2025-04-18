using Microsoft.Extensions.Logging;
using OcrDemo.Core.Requests;
using OcrDemo.Core.Responses;
using OcrDemo.Core.Utils;
using OpenAI;
using OpenAI.Chat;


namespace OcrDemo.Core.Services;

public interface IDocumentService
{
    Task<IdentifyDocumentResponse> IdentifyDocument(DocumentRequest request);
}

public class OpenAiDocumentService : IDocumentService
{
  private readonly ILogger<OpenAiDocumentService> _logger;
  private readonly OpenAIClient _openAiClient;

  public OpenAiDocumentService(
    ILogger<OpenAiDocumentService> logger,
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
}

public interface IStructuredDocumentService
{
    Task<OcrInvoiceResponse> OcrInvoice(OcrRequest request);
    Task<OcrBillOfLadingResponse> OcrBillOfLading(OcrRequest request);
    Task<OcrFuelReceiptResponse> OcrFuelReceipt(OcrRequest request);
    Task<OcrRateConfirmationResponse> OcrRateConfirmation(OcrRequest request);
    Task<T?> OcrDocument<T>(OcrRequest request);
}
