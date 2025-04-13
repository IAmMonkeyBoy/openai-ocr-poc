using OcrDemo.Core.Base;
using OcrDemo.Core.Models;

namespace OcrDemo.Core;

public class OcrFuelReceiptResponse: OcrResponseBase<FuelReceipt>
{
    public OcrFuelReceiptResponse(FuelReceipt document) : base(document)
    {
    }
}