namespace OcrDemo.Core;

public class RateConfirmation
{
    
    public string ConfirmationNumber { get; set; }
    public DateTime DateIssued { get; set; }

    public PartyInfo Broker { get; set; }
    public PartyInfo Carrier { get; set; }

    public string? LoadNumber { get; set; }

    public List<Stop> Stops { get; set; } = new();
    public FreightDetails Freight { get; set; }

    public FinancialTerms FinancialTerms { get; set; }

    public EquipmentInfo? Equipment { get; set; }

    public ComplianceSection Compliance { get; set; }

    public List<InstructionNote>? Instructions { get; set; }

    public SignatureSection Signatures { get; set; }
}

public class PartyInfo
{
    public string Name { get; set; }
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? MCNumber { get; set; }
    public string? DOTNumber { get; set; }
}

public class Stop
{
    public StopType Type { get; set; } // Pickup or Delivery
    public string LocationName { get; set; }
    public string Address { get; set; }
    public string? Contact { get; set; }
    public DateTime ScheduledDateTime { get; set; }
    public AppointmentType Appointment { get; set; }
}

public enum StopType
{
    Pickup,
    Delivery,
    Intermediate
}

public enum AppointmentType
{
    Scheduled,
    FirstComeFirstServe
}

public class FreightDetails
{
    public string Commodity { get; set; }
    public double? Weight { get; set; } // lbs or kg
    public string? WeightUnit { get; set; }
    public int? PalletCount { get; set; }
    public int? PieceCount { get; set; }

    public List<SpecialRequirement>? SpecialRequirements { get; set; }
    public LoadType LoadType { get; set; }
    public TemperatureRequirement? Temperature { get; set; }
}

public enum LoadType
{
    FullTruckload,
    LessThanTruckload
}

public class TemperatureRequirement
{
    public double TemperatureF { get; set; }
    public string? Notes { get; set; }
}

public class SpecialRequirement
{
    public string Description { get; set; }
}

public class FinancialTerms
{
    public decimal LinehaulRate { get; set; }
    public List<AccessorialCharge>? Accessorials { get; set; }
    public decimal TotalRate => LinehaulRate + (Accessorials?.Sum(a => a.Amount) ?? 0);
    public string? PaymentTerms { get; set; }
    public string? InvoiceSubmissionInstructions { get; set; }
}

public class AccessorialCharge
{
    public string Description { get; set; }
    public decimal Amount { get; set; }
}

public class EquipmentInfo
{
    public string? DriverName { get; set; }
    public string? DriverPhone { get; set; }
    public string? TruckNumber { get; set; }
    public string? TrailerNumber { get; set; }
    public string? LicensePlate { get; set; }
    public string? TruckType { get; set; }
    public bool? RequiresTrackingConsent { get; set; }
}

public class ComplianceSection
{
    public decimal? AutoInsuranceMin { get; set; }
    public decimal? CargoInsuranceMin { get; set; }
    public decimal? LiabilityInsuranceMin { get; set; }

    public bool? FmcsaCompliant { get; set; }
    public bool? NoDoubleBrokering { get; set; }
    public string? AdditionalNotes { get; set; }
}

public class InstructionNote
{
    public string Title { get; set; }
    public string Content { get; set; }
}

public class SignatureSection
{
    public SignatureInfo BrokerSignature { get; set; }
    public SignatureInfo CarrierSignature { get; set; }
}

public class SignatureInfo
{
    public string Name { get; set; }
    public string? Title { get; set; }
    public DateTime SignedDate { get; set; }
}
