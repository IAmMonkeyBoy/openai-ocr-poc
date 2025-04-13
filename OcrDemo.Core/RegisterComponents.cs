using Microsoft.Extensions.DependencyInjection;

namespace OcrDemo.Core;

public static class ComponentRegistration
{
    
    public static IServiceCollection RegisterOcrDemoServices(this IServiceCollection services)
    {
        services.AddSingleton<IOpenAiChatService, OpenAiChatService>();
        return services;
    }
}