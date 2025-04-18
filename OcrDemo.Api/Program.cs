using OcrDemo.Core;
using OcrDemo.Core.Requests;
using OcrDemo.Core.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthorization();

// Add CORS services
builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowFrontend", policy =>
  {
    policy.WithOrigins("http://localhost:5174") // Replace with your frontend's URL
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
};


// Endpoint 1: Identify Document Type
app.MapPost("/identify-document", async (HttpRequest request, IDocumentService documentService) =>
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
    async (HttpRequest request, string documentType, IStructuredDocumentService openAiChatService) =>
    {
      if (string.IsNullOrWhiteSpace(documentType))
      {
        return Results.BadRequest("Document type is required.");
      }

      if (!request.HasFormContentType || request.Form.Files.Count == 0)
      {
        return Results.BadRequest("No file uploaded.");
      }

      var file = request.Form.Files[0];
      var ocrRequest = new OcrRequest
      {
        FileName = file.FileName, FileContent = file.OpenReadStream()
      };

      return documentType.ToLowerInvariant().Replace(" ", "").Replace("_", "") switch
      {
        "billoflading" => Results.Json(await openAiChatService.OcrBillOfLading(ocrRequest)),
        "invoice" => Results.Json(await openAiChatService.OcrInvoice(ocrRequest)),
        "rateconfirmation" => Results.Json(await openAiChatService.OcrRateConfirmation(ocrRequest)),
        "fuelreceipt" => Results.Json(await openAiChatService.OcrFuelReceipt(ocrRequest)),
        _ => Results.BadRequest("Unsupported document type.")
      };
    })
  .WithName("OcrDocument");

app.Run();
