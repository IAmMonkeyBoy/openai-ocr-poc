namespace OcrDemo.Core.Models;
//
// public class RateConfirmation : IStructuredDocumentParent
// {
//     [FieldPriority(FieldPriority.High)]
//     public string? ConfirmationNumber { get; set; }
//     [FieldPriority(FieldPriority.Low)]
//     public DateTime DateIssued { get; set; }
//     [FieldPriority(FieldPriority.High)]
//     public Party? Broker { get; set; }
//     public Party? Carrier { get; set; }
//     public string? LoadNumber { get; set; }
//     public List<Stop>? Stops { get; set; } = new();
//     public FreightDetails? Freight { get; set; }
//     public FinancialTerms? FinancialTerms { get; set; }
//     public EquipmentInfo? Equipment { get; set; }
//     public ComplianceSection? Compliance { get; set; }
//     [FieldPriority(FieldPriority.Medium)]
//     public List<InstructionNote>? Instructions { get; set; }
//     public SignatureSection? Signatures { get; set; }
// }
//
// public class Stop
// {
//     public StopType? StopType { get; set; } // Pickup or Delivery
//     public string? LocationName { get; set; }
//     [FieldPriority(FieldPriority.High)]
//     public string? Address { get; set; }
//     [FieldPriority(FieldPriority.Medium)]
//     public string? Contact { get; set; }
//     [FieldPriority(FieldPriority.High)]
//     public DateTime? ScheduledDateTime { get; set; }
//     public AppointmentType? AppointmentType { get; set; }
// }
//
// public enum StopType
// {
//     Pickup,
//     Delivery,
//     Intermediate
// }
//
// public enum AppointmentType
// {
//     Scheduled,
//     FirstComeFirstServe
// }
//
// public class FreightDetails
// {
//     [FieldPriority(FieldPriority.High)]
//     public string? Commodity { get; set; }
//     [FieldPriority(FieldPriority.Medium)]
//     public double? Weight { get; set; } // lbs or kg
//     [FieldPriority(FieldPriority.Medium)]
//     public string? WeightUnit { get; set; }
//     [FieldPriority(FieldPriority.Medium)]
//     public int? PalletCount { get; set; }
//     [FieldPriority(FieldPriority.Medium)]
//     public int? PieceCount { get; set; }
//
//     public List<SpecialRequirement>? SpecialRequirements { get; set; }
//
//     public LoadType? LoadType { get; set; }
//     [FieldPriority(FieldPriority.Medium)]
//     public TemperatureRequirement? Temperature { get; set; }
// }
//
// public enum LoadType
// {
//     FullTruckload,
//     LessThanTruckload
// }
//
// public class TemperatureRequirement
// {
//     public double? TemperatureF { get; set; }
//     public string? Notes { get; set; }
// }
//
// public class SpecialRequirement
// {
//     public string? Description { get; set; }
// }
//
// public class FinancialTerms
// {
//     [FieldPriority(FieldPriority.High)]
//     public decimal? LinehaulRate { get; set; }
//     [FieldPriority(FieldPriority.Medium)]
//     public List<AccessorialCharge>? Accessorials { get; set; }
//     [FieldPriority(FieldPriority.High)]
//     public decimal? TotalRate { get; set; }
//     [FieldPriority(FieldPriority.Low)]
//     public string? PaymentTerms { get; set; }
//     public string? InvoiceSubmissionInstructions { get; set; }
// }
//
// public class AccessorialCharge
// {
//     public string? Description { get; set; }
//     public decimal? Amount { get; set; }
// }
//
// public class EquipmentInfo
// {
//     public string? DriverName { get; set; }
//     public string? DriverPhone { get; set; }
//     public string? TruckNumber { get; set; }
//     public string? TrailerNumber { get; set; }
//     public string? LicensePlate { get; set; }
//     public string? TruckType { get; set; }
//     public bool? RequiresTrackingConsent { get; set; }
// }
//
// public class ComplianceSection
// {
//     public decimal? AutoInsuranceMin { get; set; }
//     public decimal? CargoInsuranceMin { get; set; }
//     public decimal? LiabilityInsuranceMin { get; set; }
//
//     public bool? FmcsaCompliant { get; set; }
//     public bool? NoDoubleBrokering { get; set; }
//     public string? AdditionalNotes { get; set; }
// }
//
// public class InstructionNote
// {
//     public string? Title { get; set; }
//     public string? Content { get; set; }
// }
//
// public class SignatureSection
// {
//     public SignatureInfo? BrokerSignature { get; set; }
//     public SignatureInfo? CarrierSignature { get; set; }
// }
//
// public class SignatureInfo
// {
//     public string? Name { get; set; }
//     public string? Title { get; set; }
//     public DateTime? SignedDate { get; set; }
// }


public class RateConfirmation :IStructuredDocumentParent
{
    public LoadIdentification? LoadIdentification { get; set; }
    public Stop? Origin { get; set; }
    public Stop? Destination { get; set; }
    public FreightDetails? FreightDetails { get; set; }
    public FinancialInformation? FinancialInformation { get; set; }
    public AdditionalInfo? Other { get; set; }
}

public class LoadIdentification :IStructuredDocumentChild
{
    [FieldPriority(FieldPriority.High)]
    public string? LoadNumberReferenceId { get; set; }

    [FieldPriority(FieldPriority.High)]
    public string? BrokerNameContactInformation { get; set; }

    [FieldPriority(FieldPriority.Low)]
    public DateTime? RateConfirmationDate { get; set; }

    [FieldPriority(FieldPriority.Medium)]
    public string? PONumber { get; set; }
}

public class Stop : IStructuredDocumentChild
{
    [FieldPriority(FieldPriority.High)]
    public string? Address { get; set; }
    [FieldPriority(FieldPriority.High)]
    public string? City { get; set; }
    [FieldPriority(FieldPriority.High)]
    public string? State { get; set; }
    [FieldPriority(FieldPriority.High)]
    public string? Zip { get; set; }

    [FieldPriority(FieldPriority.High)]
    public DateTime? AppointmentDateTime { get; set; }

    [FieldPriority(FieldPriority.Medium)]
    public string? ContactName { get; set; }

    [FieldPriority(FieldPriority.Medium)]
    public string? ContactPhoneNumber { get; set; }
}

public class FreightDetails : IStructuredDocumentChild
{
    [FieldPriority(FieldPriority.High)]
    public string? CommodityDescription { get; set; }

    [FieldPriority(FieldPriority.Medium)]
    public decimal? Weight { get; set; }

    [FieldPriority(FieldPriority.Medium)]
    public int? NumberOfPalletsOrPieces { get; set; }

    [FieldPriority(FieldPriority.High)]
    public string? EquipmentType { get; set; }

    [FieldPriority(FieldPriority.Medium)]
    public string? TemperatureControl { get; set; }
}

public class FinancialInformation : IStructuredDocumentChild
{
    [FieldPriority(FieldPriority.High)]
    public decimal? LineHaulRate { get; set; }

    [FieldPriority(FieldPriority.Medium)]
    public decimal? AccessorialCharges { get; set; }

    [FieldPriority(FieldPriority.High)]
    public decimal? TotalRatePayAmount { get; set; }

    [FieldPriority(FieldPriority.Low)]
    public string? PaymentTerms { get; set; }
}

public class AdditionalInfo : IStructuredDocumentChild
{
    [FieldPriority(FieldPriority.Medium)]
    public string? SpecialInstructionsNotes { get; set; }

    [FieldPriority(FieldPriority.Medium)]
    public string? HazmatSpecialHandlingFlags { get; set; }
}
