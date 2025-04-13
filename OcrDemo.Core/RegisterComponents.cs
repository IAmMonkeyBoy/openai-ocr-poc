using Betalgo.Ranul.OpenAI;
using Betalgo.Ranul.OpenAI.Extensions;
using Microsoft.Extensions.DependencyInjection;
using OcrDemo.Core.Services;

namespace OcrDemo.Core;

public static class ComponentRegistration
{
    
    public static IServiceCollection RegisterOcrDemoServices(this IServiceCollection services, string? openAiApiKey)
    {
        services.AddOpenAIService(settings => settings.ApiKey = openAiApiKey);
        services.AddSingleton<IOpenAiChatService, OpenAiChatService>();
        return services;
    }
}
