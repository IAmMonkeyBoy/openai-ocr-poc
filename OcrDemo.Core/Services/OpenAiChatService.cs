using OcrDemo.Core.Requests;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Nodes;
namespace OcrDemo.Core;

public interface IOpenAiChatService
{
    Task<IdentifyDocumentResponse> IdentifyDocument(DocumentRequest request);
    Task<OcrInvoiceResponse> OcrInvoice(OcrRequest request);
    Task<OcrBillOfLadingResponse> OcrBillOfLading(OcrRequest request);
    Task<OcrFuelReceiptResponse> OcrFuelReceipt(OcrRequest request);
    Task<OcrRateConfirmationResponse> OcrRateConfirmation(OcrRequest request);
}


public class OpenAiChatService : IOpenAiChatService
{
    public Task<IdentifyDocumentResponse> IdentifyDocument(DocumentRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<OcrInvoiceResponse> OcrInvoice(OcrRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<OcrBillOfLadingResponse> OcrBillOfLading(OcrRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<OcrFuelReceiptResponse> OcrFuelReceipt(OcrRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<OcrRateConfirmationResponse> OcrRateConfirmation(OcrRequest request)
    {
        string prompt = GeneratePrompt(typeof(RateConfirmation));
        
        throw new NotImplementedException();
    }

    private string GeneratePrompt(Type type)
    {
        string documentTypeName = "";
        string prompt = $"""
                         You are an AI assistant of performing OCR on documents, and then transcribing the data into a 
                         structured format.  Please analyze the following {documentTypeName} document and extract the 
                         relevant information into the structured format below.
                         
                         
                         """;
        string schema = GenerateJsonSchema(type);
        
            return prompt + schema;
            
            
    }
    
    
    private string GenerateJsonSchema(Type type)
    {
        var schema = new JsonObject
        {
            ["$schema"] = "http://json-schema.org/draft-07/schema#",
            ["type"] = "object",
            ["properties"] = new JsonObject()
        };

        var properties = type.GetProperties();
        foreach (var property in properties)
        {
            var propertySchema = new JsonObject
            {
                ["type"] = GetJsonType(property.PropertyType)
            };

            ((JsonObject)schema["properties"])[property.Name] = propertySchema;
        }

        return JsonSerializer.Serialize(schema, new JsonSerializerOptions
        {
            WriteIndented = true
        });
    }

    private string GetJsonType(Type type)
    {
        if (type == typeof(string)) return "string";
        if (type == typeof(int) || type == typeof(long)) return "integer";
        if (type == typeof(float) || type == typeof(double) || type == typeof(decimal)) return "number";
        if (type == typeof(bool)) return "boolean";
        if (type == typeof(DateTime)) return "string"; // Dates are typically represented as strings in JSON
        if (type.IsArray || typeof(IEnumerable<>).IsAssignableFrom(type)) return "array";
        if (type.IsClass) return "object";

        return "string"; // Default to string for unknown types
    }
}