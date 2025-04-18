using Microsoft.Extensions.DependencyInjection;
using OcrDemo.Core.Services;
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
        services.AddSingleton<IDocumentService, OpenAiDocumentService>();
        return services;
    }

    public enum OcrProvider
    {
        Tesseract,
        Iron,
        OpenAi
    }
    
}

