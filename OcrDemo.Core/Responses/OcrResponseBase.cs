using OcrDemo.Core.Models;

namespace OcrDemo.Core.Responses;

public class OcrResponseBase<T>(T? document)
{
    public T? Document { get; set; } = document;
}


public class OcrRateConfirmationResponse(RateConfirmation document) : OcrResponseBase<RateConfirmation>(document);

public class OcrInvoiceResponse(BillOfLading document) : OcrResponseBase<BillOfLading>(document);

public class OcrFuelReceiptResponse(FuelReceipt document) : OcrResponseBase<FuelReceipt>(document);

public class OcrBillOfLadingResponse(BillOfLading document) : OcrResponseBase<BillOfLading>(document);
