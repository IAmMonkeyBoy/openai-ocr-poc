using System.Reflection;
using Anthropic;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OcrDemo.Core.Services;
using OcrDemo.Core.Services.Document.Identification;
using OcrDemo.Core.Services.Document.Structuring;
using OcrDemo.Core.Services.Ocr;
using OllamaSharp;
using OpenAI;

namespace OcrDemo.Core;

public static class ComponentRegistration
{
    public static IServiceCollection RegisterOcrDemoServices(this IServiceCollection services, string openAiApiKey, string anthropicApiKey)
    {
        services.AddSingleton<OpenAIClient>(x => new OpenAIClient(openAiApiKey));
        services.AddSingleton<AnthropicClient>(x => new AnthropicClient(anthropicApiKey));
        services.AddKeyedSingleton<IStructuredDocumentService, OpenAiStructuredDocumentService>(
          nameof(OpenAiStructuredDocumentService));
        services.AddKeyedSingleton<IStructuredDocumentService, OllamaStructuredDocumentService>(
          nameof(OllamaStructuredDocumentService));
        services.AddKeyedSingleton<IStructuredDocumentService, MeaiOpenAiStructuredDocumentService>(
          nameof(MeaiOpenAiStructuredDocumentService));
        services.AddKeyedSingleton<IStructuredDocumentService, MeaiAnthropicStructuredDocumentService>(
          nameof(MeaiAnthropicStructuredDocumentService));
        services.AddKeyedSingleton<IStructuredDocumentService, AnthropicStructuredDocumentService>(
          nameof(AnthropicStructuredDocumentService));
        services.AddKeyedSingleton<IOcrService, TesseractOcrService>(OcrProvider.Tesseract);
        services.AddKeyedSingleton<IOcrService, IronOcrService>(OcrProvider.Iron);
        services.AddKeyedSingleton<IOcrService, OpenAiOcrService>(OcrProvider.OpenAi);
        services.AddSingleton<IDocumentIdentificationService, OpenAiDocumentService>();
        services.AddSingleton<IOcrResponseScoringService, OcrResponseScoringService>();
        //services.AddSingleton<MEAIStructuredDocumentServiceBase>();
        services.AddChatClient(x => new OpenAIClient(openAiApiKey).GetChatClient("gpt-4o").AsIChatClient());
        //services.AddSingleton<OllamaStructuredDocumentService>();
        services.AddSingleton<IOllamaApiClient>(x => new OllamaApiClient("http://localhost:11434"));
        return services;
    }

    public enum OcrProvider
    {
        Tesseract,
        Iron,
        OpenAi
    }
    
}

