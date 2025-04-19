using OcrDemo.Core.Models;

namespace OcrDemo.Core.Responses;

public class OcrAndStructureResponseBase<T>(T? document, OcrDocumentQualityAssessment? qualityAssessment = null)
{
    public T? Document { get; set; } = document;
    public OcrDocumentQualityAssessment? QualityAssessment { get; set; } = qualityAssessment;
}

public class OcrRateConfirmationResponse(RateConfirmation? document) : OcrAndStructureResponseBase<RateConfirmation>(document);

public class OcrInvoiceResponse(Invoice? document) : OcrAndStructureResponseBase<Invoice>(document);

public class OcrFuelReceiptResponse(FuelReceipt? document) : OcrAndStructureResponseBase<FuelReceipt>(document);

public class OcrBillOfLadingResponse(BillOfLading? document) : OcrAndStructureResponseBase<BillOfLading>(document);
