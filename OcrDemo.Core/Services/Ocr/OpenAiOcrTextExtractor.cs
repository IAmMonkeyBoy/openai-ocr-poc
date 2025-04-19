using Microsoft.Extensions.Logging;
using OcrDemo.Core.Requests;
using OcrDemo.Core.Utils;
using OpenAI;
using OpenAI.Chat;

namespace OcrDemo.Core.Services.Ocr;

public class OpenAiOcrService : IOcrService
{
    private readonly ILogger<OpenAiOcrService> _logger;
    private readonly OpenAIClient _openAiClient;

    public OpenAiOcrService(
        ILogger<OpenAiOcrService> logger,
        OpenAIClient openAiClient
    )
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _openAiClient = openAiClient ?? throw new ArgumentNullException(nameof(openAiClient));
    }

    public async Task<string> OcrDocumentToText(OcrRequest request)
    {
        var bytes = request.FileContent.ToByteArray();
        var inferredImageType = bytes.InferImageTypeFromBytes();

        var response = await _openAiClient.GetChatClient("gpt-4o").CompleteChatAsync(
            new SystemChatMessage("You are an AI assistant tasked with extracting text from documents. Please provide all visible text from the image in a clear, structured format."),
            new UserChatMessage(ChatMessageContentPart.CreateImagePart(new BinaryData(bytes), inferredImageType)),
            new UserChatMessage("Please extract and return all visible text from this image.")
        );

        return response?.Value.Content.FirstOrDefault()?.Text ?? string.Empty;
    }




    
    
    
}
