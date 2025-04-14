using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Schema;
// using Betalgo.Ranul.OpenAI.Interfaces;
// using Betalgo.Ranul.OpenAI.ObjectModels;
// using Betalgo.Ranul.OpenAI.ObjectModels.RequestModels;
// using Betalgo.Ranul.OpenAI.ObjectModels.SharedModels;
using Microsoft.Extensions.Logging;
using OcrDemo.Core.Requests;
using OcrDemo.Core.Responses;
using OpenAI;
using OpenAI.Chat;

namespace OcrDemo.Core.Services;

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
    private readonly ILogger<OpenAiChatService> _logger;

//    private readonly IOpenAIService _openAiService;
    private readonly OpenAIClient _openAiClient;

    public OpenAiChatService(
        ILogger<OpenAiChatService> logger,
//        IOpenAIService openAiService,
        OpenAI.OpenAIClient openAiClient
    )
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
//        _openAiService = openAiService ?? throw new ArgumentNullException(nameof(openAiService));
        _openAiClient = openAiClient ?? throw new ArgumentNullException(nameof(openAiClient));
    }

    public async Task<IdentifyDocumentResponse> IdentifyDocument(DocumentRequest request)
    {
        byte[] bytes = GetFileBytes(request.FileContent);
        string inferredImageType = InferImageTypeFromBytes(bytes);

        var response = await _openAiClient.GetChatClient("gpt-4o").CompleteChatAsync(
            new SystemChatMessage("""
                                  You are an AI assistant tasked with identifying the type of document based on its content.
                                  The possible document types are: "Bill of Lading", "Invoice", "Rate Confirmation", and "Fuel Receipt".
                                  If you cannot determine the type, respond with "Unknown".
                                  """),
            new UserChatMessage(ChatMessageContentPart.CreateImagePart(new BinaryData(bytes), inferredImageType)),
            new UserChatMessage("What is the document type?  Respond with only the document type name")
        );

        return new IdentifyDocumentResponse
        {
            DocumentType = response?.Value.Content.FirstOrDefault()?.Text ?? "Unknown"
        };
    }

    private string InferImageTypeFromBytes(byte[] bytes)
    {
        if (bytes.Length < 4)
            throw new ArgumentException("Byte array is too small to determine the image type.");

        // Check for PNG
        if (bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47)
            return "image/png";

        // Check for JPEG
        if (bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF)
            return "image/jpeg";

        // Check for GIF
        if (bytes[0] == 0x47 && bytes[1] == 0x49 && bytes[2] == 0x46)
            return "image/gif";

        // Check for WEBP
        if (bytes[0] == 0x52 && bytes[1] == 0x49 && bytes[2] == 0x46 && bytes[3] == 0x46)
            return "image/webp";

        throw new NotSupportedException("Unsupported image type.");
    }


    private byte[] GetFileBytes(Stream requestFileContent)
    {
        using MemoryStream memoryStream = new MemoryStream();
        requestFileContent.CopyTo(memoryStream);
        return memoryStream.ToArray();
    }

    private static async Task<StringBuilder> Base64EncodedFileContents(Stream stream)
    {
        StringBuilder base64EncodedFileContents = new StringBuilder();


        while (stream.CanRead)
        {
            byte[] buffer = new byte[4096];
            int bytesRead = await stream.ReadAsync(buffer.AsMemory(0, buffer.Length));
            if (bytesRead == 0)
                break;
            base64EncodedFileContents.Append(Convert.ToBase64String(buffer, 0, bytesRead));
        }

        return base64EncodedFileContents;
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


    private string TypeNameToRealName<T>()
    {
       var typeName = typeof(T).Name;
       return string.Concat(typeName.Select((x, i) => i > 0 && char.IsUpper(x) ? " " + x : x.ToString())).ToLower();
       
        
    }
    
    public async Task<T?> OcrDocument<T>(OcrRequest request)
    {
        byte[] bytes = GetFileBytes(request.FileContent);
        string inferredImageType = InferImageTypeFromBytes(bytes);
        var messages = new List<ChatMessage>
        {
            new SystemChatMessage(GeneratePrompt(typeof(T))),
            new UserChatMessage(ChatMessageContentPart.CreateImagePart(new BinaryData(bytes), inferredImageType)),
            new UserChatMessage($"""
                                 Please generate a {TypeNameToRealName<T>()} from the supplied image in the specified 
                                 format.  Please be sure that when working with dates and date times, you follow the 
                                 format specified.  In particular date-time should be provided as specified in RFC 3339 
                                 Section 5.6 format.
                                 """)
        };
        
        var responseSchema = GenerateJsonSchemaString(typeof(T));
        var options = new ChatCompletionOptions()
        {
            ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(nameof(T), BinaryData.FromString(responseSchema)),
            Temperature = 0.7F,
            MaxOutputTokenCount = 1000,
        };
        
        var result = await _openAiClient.GetChatClient("gpt-4o").CompleteChatAsync(messages, options);
        var outputAsText = result.Value.Content.FirstOrDefault()?.Text ?? string.Empty;
       return          JsonSerializer.Deserialize<T>(outputAsText, new JsonSerializerOptions { });
       ;
        
    }

    public async Task<OcrRateConfirmationResponse> OcrRateConfirmation(OcrRequest request)
    {
        // byte[] bytes = GetFileBytes(request.FileContent);
        // string inferredImageType = InferImageTypeFromBytes(bytes);
        // var messages = new List<ChatMessage>
        // {
        //     new SystemChatMessage(GeneratePrompt(typeof(RateConfirmation))),
        //     new UserChatMessage(ChatMessageContentPart.CreateImagePart(new BinaryData(bytes), inferredImageType)),
        //     new UserChatMessage("Please generate a rate confirmation from the supplied image in the specified format.")
        // };
        //
        // var responseSchema = GenerateJsonSchemaString(typeof(RateConfirmation));
        // var options = new ChatCompletionOptions()
        // {
        //     ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(nameof(RateConfirmation), BinaryData.FromString(responseSchema)),
        //     Temperature = 0.7F,
        //     MaxOutputTokenCount = 1000,
        // };
        //
        // var result = await _openAiClient.GetChatClient("gpt-4o").CompleteChatAsync(messages, options);
        // var outputAsText = result.Value.Content.FirstOrDefault()?.Text ?? string.Empty;
        

        return new OcrRateConfirmationResponse(await OcrDocument<RateConfirmation>(request));
    }


    private string GeneratePrompt(Type type)
    {
        string documentTypeName = "";
        string prompt = $"""
                         You are an AI assistant of performing OCR on documents, and then transcribing the data into a 
                         structured format.  Please analyze the following {documentTypeName} document and extract the 
                         relevant information into the structured format below.


                         """;
        // string schema = GenerateJsonSchemaString(type);

        return prompt;
    }


    private string GenerateJsonSchemaString(Type type)
    {
        JsonSerializerOptions options = JsonSerializerOptions.Default;

        var schemaNode = options.GetJsonSchemaAsNode(type,
            new JsonSchemaExporterOptions() { TreatNullObliviousAsNonNullable = true });
        var s = schemaNode.ToString();
        return schemaNode.ToString();
    }
}

/*
private JsonSchema GenerateJsonSchema(Type type)
{

    JsonSerializerOptions options = JsonSerializerOptions.Default;
    var node = options.GetJsonSchemaAsNode(type);

    var nodeName = node.Parent != null ? node.GetPropertyName() : type.Name;

    var returnValue = new JsonSchema()
    {

        Name = nodeName,
        Schema = GetPropertyDefinitionFromType(type)
    };

return returnValue;

}

private PropertyDefinition? GetPropertyDefinitionFromType(Type type)
{

    if (type.IsPrimitive)
    {
        if (type == typeof(int) || type == typeof(long) || type == typeof(short))
        {
            return PropertyDefinition.DefineInteger();
        }
        if (type == typeof(float) || type == typeof(double) || type == typeof(decimal))
        {
            return PropertyDefinition.DefineNumber();
        }
        if (type == typeof(bool))
        {
            return PropertyDefinition.DefineBoolean();
        }
    }

    if (type.IsEnum)
    {
        return PropertyDefinition.DefineEnum(type.GetEnumNames().ToList());
    }


    if (type.IsArray || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)))
    {
        return PropertyDefinition.DefineArray(GetPropertyDefinitionFromType(type.IsArray ? type.GetElementType()! : type.GetGenericArguments()[0]));
    }

    if (type == typeof(string))
    {
        return PropertyDefinition.DefineString();
    }

    if (type == typeof(DateTime))
    {
        return PropertyDefinition.DefineString("ISO 8601 date format");
    }
    var returnValue = new PropertyDefinition();
    returnValue.Type = "object";
    returnValue.Properties = new Dictionary<string, PropertyDefinition>();
    foreach (var prop in type.GetProperties())
    {
        var propertyDefinition = GetPropertyDefinitionFromType(prop.PropertyType);
        if (propertyDefinition != null)
        {
            returnValue.Properties.Add(prop.Name, propertyDefinition);
        }
    }
    return returnValue;
}

private PropertyDefinition? GetPropertyDefinitionFromNode(JsonNode? node)
{
    switch (node?.GetValueKind())
    {
        case JsonValueKind.Object:
            var properties = new Dictionary<string,PropertyDefinition>();
            foreach (var property in node.AsObject())
            {
                var propertyDefinition = GetPropertyDefinitionFromNode(property.Value);
                if (propertyDefinition != null)
                {
                    properties.Add(property.Key, new PropertyDefinition
                    {

                        Type = propertyDefinition.Type,
                        //Required = true
                    });
                }
            }
            return new PropertyDefinition
            {
                Type = "object",

                Properties = properties
            };
        case JsonValueKind.Array:
            var arrayItems = node.AsArray().Select(item => GetPropertyDefinitionFromNode(item)).ToList();
            return new PropertyDefinition
            {
                Type = "array",
                Items = arrayItems.FirstOrDefault()
            };
        case JsonValueKind.String:
            return new PropertyDefinition
            {
                Type = "string"
            };
        case JsonValueKind.Number:
            return new PropertyDefinition
            {
                Type = "number"
            };
        case JsonValueKind.True:
        case JsonValueKind.False:
            return new PropertyDefinition
            {
                Type = "boolean"
            };
        case JsonValueKind.Null:
            return new PropertyDefinition
            {
                Type = "null"
            };
        default:
            return null;
    }
}
}
*/