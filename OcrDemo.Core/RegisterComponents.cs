using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Distributed;
using OcrDemo.Core.Services;
using OcrDemo.Core.Services.Document.Identification;
using OcrDemo.Core.Services.Document.Structuring;
using OcrDemo.Core.Services.Ocr;
using OpenAI;

namespace OcrDemo.Core;

public static class ComponentRegistration
{
    public static IServiceCollection RegisterOcrDemoServices(this IServiceCollection services, string? openAiApiKey, string ollamaEndpointUrl, string ollamaModel)
    {
      services.AddDistributedMemoryCache();
        services.AddSingleton<OpenAIClient>(x => new OpenAIClient(openAiApiKey));
        
        services.AddKeyedChatClient(ChatProvider.OpenAi,_ =>
            new OpenAI.Chat.ChatClient("gpt-4o-mini", openAiApiKey)
              .AsIChatClient())
          .UseDistributedCache()
          .UseLogging();
        
         services.AddKeyedChatClient(
             ChatProvider.Ollama, new OllamaChatClient("http://127.0.0.1:11434", "llama3.1"))
           .UseDistributedCache()
           .UseLogging();
        services.AddSingleton<IStructuredDocumentService, OpenAiStructuredDocumentService>();
        services.AddSingleton<OllamaStructuredDocumentService>();
        services.AddSingleton<MsOpenAiStructuredDocumentService>();
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

    public enum ChatProvider
    {
      OpenAi,
      AzureOpenAi,
      Ollama
    }
    
    
}


