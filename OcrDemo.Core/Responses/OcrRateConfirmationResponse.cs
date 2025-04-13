using OcrDemo.Core.Base;

namespace OcrDemo.Core;

public class OcrRateConfirmationResponse: OcrResponseBase<RateConfirmation>
{
    public OcrRateConfirmationResponse(RateConfirmation document) : base(document)
    {
    }
}