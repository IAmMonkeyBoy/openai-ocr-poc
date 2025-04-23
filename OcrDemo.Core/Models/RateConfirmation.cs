namespace OcrDemo.Core.Models;


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
