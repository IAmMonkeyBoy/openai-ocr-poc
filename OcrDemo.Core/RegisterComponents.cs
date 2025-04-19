using Microsoft.Extensions.DependencyInjection;
using OcrDemo.Core.Services;
using OcrDemo.Core.Services.Document.Identification;
using OcrDemo.Core.Services.Document.Structuring;
using OcrDemo.Core.Services.Ocr;
using OpenAI;

namespace OcrDemo.Core;

public static class ComponentRegistration
{
    public static IServiceCollection RegisterOcrDemoServices(this IServiceCollection services, string? openAiApiKey)
    {
        services.AddSingleton<OpenAIClient>(x => new OpenAIClient(openAiApiKey));
        services.AddSingleton<IStructuredDocumentService, OpenAiStructuredDocumentService>();
        services.AddKeyedSingleton<IOcrService, TesseractOcrService>(OcrProvider.Tesseract);
        services.AddKeyedSingleton<IOcrService, IronOcrService>(OcrProvider.Iron);
        services.AddKeyedSingleton<IOcrService, OpenAiOcrService>(OcrProvider.OpenAi);
        services.AddSingleton<IDocumentIdentificationService, OpenAiDocumentService>();
        services.AddSingleton<IOcrResponseScoringService, OcrResponseScoringService>();
        return services;
    }

    public enum OcrProvider
    {
        Tesseract,
        Iron,
        OpenAi
    }
    
}

