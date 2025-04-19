using System.Text.Json;
using System.Text.Json.Serialization;
using OcrDemo.Core;
using OcrDemo.Core.Models;
using OcrDemo.Core.Requests;
using OcrDemo.Core.Responses;
using OcrDemo.Core.Services;
using OcrDemo.Core.Services.Document.Identification;
using OcrDemo.Core.Services.Document.Structuring;
using TesseractOCR.Renderers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthorization();

// Add CORS services
builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowFrontend", policy =>
  {
    policy.WithOrigins("http://localhost:5173") // Replace with your frontend's URL
      .AllowAnyHeader()
      .AllowAnyMethod();
  });
});

// Register services from OcrDemo.Core

var openAiApiKey = builder.Configuration["OpenAIServiceOptions:ApiKey"];
builder.Services.RegisterOcrDemoServices(openAiApiKey);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// Use CORS
app.UseCors("AllowFrontend");

// Configure JSON serialization to use Pascal case globally
var jsonOptions = new System.Text.Json.JsonSerializerOptions
{
  PropertyNamingPolicy = null // Use Pascal case
  , Converters = { new JsonStringEnumConverter() }
};


// Endpoint 1: Identify Document Type
app.MapPost("/identify-document", async (HttpRequest request, IDocumentIdentificationService documentService) =>
  {
    if (!request.HasFormContentType || request.Form.Files.Count == 0)
    {
      return Results.BadRequest("No file uploaded.");
    }

    var file = request.Form.Files[0];
    var identifyRequest = new DocumentRequest { FileName = file.FileName, FileContent = file.OpenReadStream() };

    var response = await documentService.IdentifyDocument(identifyRequest);
    return Results.Json(response, jsonOptions); // Apply Pascal case serialization
  })
  .WithName("IdentifyDocument");

// Endpoint 2: OCR Document
app.MapPost("/ocr-document/{documentType}",
    async (HttpRequest request, string documentType, IStructuredDocumentService openAiChatService, IOcrResponseScoringService ocrResponseScoringService) =>
    {
      switch (documentType.ToLowerInvariant().Replace(" ", "").Replace("_", ""))
      {
        case "billoflading":
          return Results.Json(await ProcessOcrDocumentAsync<BillOfLading>(request, documentType, openAiChatService, ocrResponseScoringService), jsonOptions);
        case "invoice":
          return Results.Json(await ProcessOcrDocumentAsync<Invoice>(request, documentType, openAiChatService, ocrResponseScoringService), jsonOptions);
        case "rateconfirmation":
          return Results.Json(await ProcessOcrDocumentAsync<RateConfirmation>(request, documentType, openAiChatService, ocrResponseScoringService), jsonOptions);
        case "fuelreceipt":
          return Results.Json(await ProcessOcrDocumentAsync<FuelReceipt>(request, documentType, openAiChatService, ocrResponseScoringService), jsonOptions);
      }
      return Results.BadRequest("Document type is not supported.");

    })
  .WithName("OcrDocument");

app.Run();
return;


static async Task<OcrAndStructureResponseBase<T>> ProcessOcrDocumentAsync<T>(
  HttpRequest request,
  string documentType,
  IStructuredDocumentService openAiChatService,
  IOcrResponseScoringService ocrResponseScoringService) 
  where T : IStructuredDocumentParent, new()
{
  if (string.IsNullOrWhiteSpace(documentType))
  {
    //return Results.BadRequest("Document type is required.");
  }

  if (!request.HasFormContentType || request.Form.Files.Count == 0)
  {
    //return Results.BadRequest("No file uploaded.");
  }

  var file = request.Form.Files[0];
  var ocrRequest = new OcrRequest
  {
    FileName = file.FileName,
    FileContent = file.OpenReadStream()
  };

  var document = await openAiChatService.OcrDocument<T>(ocrRequest);


  if (document == null)
  {
    //return // Results.BadRequest("Document type is not supported.");
  }

  var score = await ocrResponseScoringService.ScoreDocumentAsync(document);
  return new OcrAndStructureResponseBase<T>(document, score);
}
