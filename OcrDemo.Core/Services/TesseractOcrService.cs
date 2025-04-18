using TesseractOCR;
using OcrDemo.Core.Requests;
using Microsoft.Extensions.Logging;
using TesseractOCR.Enums;

namespace OcrDemo.Core.Services;

public class TesseractOcrService : IOcrService, IDisposable
{
    private readonly ILogger<TesseractOcrService> _logger;
    private readonly Engine _engine;
    private bool _disposed;

    public TesseractOcrService(
        ILogger<TesseractOcrService> logger,
        string tessDataPath = "./tessdata",
        string language = "eng")
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _engine = new Engine(tessDataPath, language, EngineMode.Default);
      
    }

    public async Task<string> OcrDocumentToText(OcrRequest request)
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(TesseractOcrService));
        }

        try
        {
            using var memoryStream = new MemoryStream();
            await request.FileContent.CopyToAsync(memoryStream);

            using var img =
              TesseractOCR.Pix.Image.LoadFromMemory(memoryStream.ToArray());

            if (img == null)
            {
                throw new InvalidOperationException("Failed to load image");
            }
            
            using var page = _engine.Process(img);
            return 
              
              page.Text ?? string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OCR processing failed");
            throw new InvalidOperationException($"OCR processing failed: {ex.Message}", ex);
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _engine.Dispose();
            _disposed = true;
        }
    }
}
