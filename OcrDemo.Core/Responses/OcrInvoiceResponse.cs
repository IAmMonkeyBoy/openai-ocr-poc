using OcrDemo.Core.Base;
using OcrDemo.Core.Models;

namespace OcrDemo.Core;

public class OcrInvoiceResponse: OcrResponseBase<BillOfLading>
{
    public OcrInvoiceResponse(BillOfLading document) : base(document)
    {
    }
}