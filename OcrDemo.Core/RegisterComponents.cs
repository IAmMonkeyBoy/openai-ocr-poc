// using Betalgo.Ranul.OpenAI;
// using Betalgo.Ranul.OpenAI.Extensions;
using Microsoft.Extensions.DependencyInjection;
using OcrDemo.Core.Services;
using OpenAI;

namespace OcrDemo.Core;

public static class ComponentRegistration
{
    
    public static IServiceCollection RegisterOcrDemoServices(this IServiceCollection services, string? openAiApiKey)
    {
//        services.AddOpenAIService(settings => settings.ApiKey = openAiApiKey);
        services.AddSingleton<OpenAIClient>(x => new OpenAIClient(openAiApiKey));
        services.AddSingleton<IOpenAiChatService, OpenAiChatService>();
        return services;
    }
}
