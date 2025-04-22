using OcrDemo.Core.Models;
using OcrDemo.Core.Utils;

namespace OcrDemo.Core.Services;

public interface IOcrResponseScoringService
{
    Task<OcrDocumentQualityAssessment> ScoreDocumentAsync(IStructuredDocumentParent? document);
}

public class OcrResponseScoringService : IOcrResponseScoringService
{
    public Task<OcrDocumentQualityAssessment> ScoreDocumentAsync(IStructuredDocumentParent? document)
    {
      return Task.FromResult(new OcrDocumentQualityAssessment()
      {
        FieldEvaluations = document?.EvaluateOcrFieldResults() ?? [],
      });
    }
}
