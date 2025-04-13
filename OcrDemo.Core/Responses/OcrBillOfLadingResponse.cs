using OcrDemo.Core.Base;
using OcrDemo.Core.Models;

namespace OcrDemo.Core;

public class OcrBillOfLadingResponse : OcrResponseBase<BillOfLading>
{
    public OcrBillOfLadingResponse(BillOfLading document) : base(document)
    {
    }
}