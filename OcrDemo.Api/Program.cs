using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using OcrDemo.Api.Requests;
using OcrDemo.Api.Responses;
using OcrDemo.Core;
using OcrDemo.Core.Models;
using OcrDemo.Core.Requests;
using OcrDemo.Core.Services;
using OcrDemo.Core.Services.Document.Identification;
using OcrDemo.Core.Services.Document.Structuring;

var builder = WebApplication.CreateBuilder(args);
var openAiApiKey = builder.Configuration["OpenAIServiceOptions:ApiKey"];
var anthropicApiKey = builder.Configuration["AnthropicServiceOptions:ApiKey"];

// Add services to the container.
builder.Services.AddAuthorization();
builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowFrontend", policy =>
  {
    policy.WithOrigins("http://localhost:8675") // Replace with your frontend's URL
      .AllowAnyHeader()
      .AllowAnyMethod();
  });
});
builder.Services.RegisterOcrDemoServices(openAiApiKey, anthropicApiKey);

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseCors("AllowFrontend");


var jsonOptions = new System.Text.Json.JsonSerializerOptions
{
  PropertyNamingPolicy = null, Converters = { new JsonStringEnumConverter() }
};


app.MapPost("/identify-document", async (HttpRequest request, IDocumentIdentificationService documentService) =>
  {
    if (!request.HasFormContentType || request.Form.Files.Count == 0)
    {
      return Results.BadRequest("No file uploaded.");
    }
    var file = request.Form.Files[0];
    var identifyRequest = new DocumentRequest
    {
      FileName = file.FileName, FileContent = file.OpenReadStream(), Model = String.Empty
    };
    var response = await documentService.IdentifyDocument(identifyRequest);
    return Results.Json(response, jsonOptions); // Apply Pascal case serialization
  })
  .WithName("IdentifyDocument")
  .DisableAntiforgery();


app.MapPost("/ocr-document/{documentType}",
    async (
      [FromForm] GenerateStructuredDocumentRequest request,
      [FromForm] IFormFileCollection files,
      [FromRoute] string documentType,
      [FromKeyedServices(nameof(OpenAiStructuredDocumentService))]
      IStructuredDocumentService openAiService,
      [FromKeyedServices(nameof(OllamaStructuredDocumentService))]
      IStructuredDocumentService ollamaService,
      [FromKeyedServices(nameof(MeaiOpenAiStructuredDocumentService))]
      IStructuredDocumentService meaiService,
      [FromKeyedServices(nameof(MeaiAnthropicStructuredDocumentService))]
      IStructuredDocumentService meaiAnthropicService,
      [FromKeyedServices(nameof(AnthropicStructuredDocumentService))]
      IStructuredDocumentService anthropicService,
      IOcrResponseScoringService ocrResponseScoringService) =>
    {
      Dictionary<string, IStructuredDocumentService> services = new Dictionary<string, IStructuredDocumentService>()
      {
        { nameof(OpenAiStructuredDocumentService), openAiService },
        { nameof(OllamaStructuredDocumentService), ollamaService },
        { nameof(MeaiOpenAiStructuredDocumentService), meaiService },
        { nameof(MeaiAnthropicStructuredDocumentService), meaiAnthropicService },
        { nameof(AnthropicStructuredDocumentService), anthropicService }
      };
      var structuredDocumentServiceId = request.StructuredDocumentServiceId ?? nameof(OpenAiStructuredDocumentService);
      var documentService = services[structuredDocumentServiceId];
      var file = files.First();
      var ocrRequest = new OcrRequest
      {
        FileName = file.FileName, FileContent = file.OpenReadStream(), Model = request.Model
      };
      switch (documentType.ToLowerInvariant().Replace(" ", "").Replace("_", ""))
      {
        case "billoflading":
          return
            Results.Json(
              await StructureDocumentHelper<BillOfLading>(ocrRequest, documentService, ocrResponseScoringService),
              jsonOptions);
        case "invoice":
          return Results.Json(
            await StructureDocumentHelper<Invoice>(ocrRequest, documentService, ocrResponseScoringService),
            jsonOptions);
        case "rateconfirmation":
          return Results.Json(
            await StructureDocumentHelper<RateConfirmation>(ocrRequest, documentService, ocrResponseScoringService),
            jsonOptions);
        case "fuelreceipt":
          return Results.Json(
            await StructureDocumentHelper<FuelReceipt>(ocrRequest, documentService, ocrResponseScoringService),
            jsonOptions);
      }
      return Results.BadRequest("Document type is not supported.");
    })
  .WithName("OcrDocument")
  .DisableAntiforgery();


app.MapGet("/llm-services", async (
    HttpRequest request,
    [FromKeyedServices(nameof(OpenAiStructuredDocumentService))]
    IStructuredDocumentService oaiService,
    [FromKeyedServices(nameof(OllamaStructuredDocumentService))]
    IStructuredDocumentService ollamaService,
    [FromKeyedServices(nameof(MeaiOpenAiStructuredDocumentService))]
    IStructuredDocumentService meaiService,
    [FromKeyedServices(nameof(MeaiAnthropicStructuredDocumentService))]
    IStructuredDocumentService meaiAnthropicService,
    [FromKeyedServices(nameof(AnthropicStructuredDocumentService))]
    IStructuredDocumentService anthropicService

    ) =>
  {
    var structuredDocumentServices = new[] { oaiService, ollamaService, meaiService, meaiAnthropicService, anthropicService };
    var returnValue = new List<GetLLMResponseItem>();
    foreach (var s in structuredDocumentServices)
    {
      var availableModels = await s.GetModels();
      var models = availableModels.Select(m => new GetLLMResponseItemModel()
      {
        Name = m.Name,
        Description = m.Description ?? "No description available.",
        ModelType = m.ModelType ?? "Unknown",
        IsDefault = m.IsDefault
      }).ToList();

      returnValue.Add(new GetLLMResponseItem()
      {
        Name = s.LLMName ?? s.GetType().Name,
        ServiceId = s.GetType().Name,
        Description = s.Description ?? "No description available.",
        Models = models
      });
    }
    return Results.Json(returnValue, jsonOptions);
  })
  .WithName("LLMServices")
  .DisableAntiforgery();


app.MapGet("/ocr-services", async (HttpRequest request, IServiceProvider services) =>
  {
    IStructuredDocumentService[] structuredDocumentServices = services.GetServices<IStructuredDocumentService>().ToArray();
    var keyedServices = services.GetServices<IStructuredDocumentService>();
    var returnValue = new List<GetOcrResponseItem>();
    returnValue.AddRange(
      structuredDocumentServices.Select(s => new GetOcrResponseItem()
      {
        Name = s.LLMName, ServiceId = nameof(s), Description = s.Description ?? "No description available.",
      }).ToList());
    return Results.Json(returnValue, jsonOptions);
  })
  .WithName("OCRServices")
  .DisableAntiforgery();

app.Run();
return;


async Task<GenerateStructuredDocumentResponseImpl<T>> StructureDocumentHelper<T>(OcrRequest ocrRequest,
  IStructuredDocumentService structuredDocumentService,
  IOcrResponseScoringService ocrResponseScoringService)
  where T : IStructuredDocumentParent, new()
{
  var doc = await structuredDocumentService.OcrDocument<T>(ocrRequest);
  var score = await ocrResponseScoringService.ScoreDocumentAsync(doc);
  return new GenerateStructuredDocumentResponseImpl<T>(doc, score);
}
