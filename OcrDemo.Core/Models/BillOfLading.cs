
using System;
using System.Collections.Generic;

namespace OcrDemo.Core.Models;

public class BillOfLading : IStructuredDocumentParent
{
    public string? BillOfLadingNumber { get; set; }
    public DateTime? IssueDate { get; set; }

    public Party? Shipper { get; set; }
    public Party? Carrier { get; set; }
    public Party? Consignee { get; set; }
    public Party? NotifyParty { get; set; }

    public string? Origin { get; set; }
    public string? Destination { get; set; }
    public ShippingMethod? ShippingMethod { get; set; }
    public FreightTerms? FreightTerms { get; set; }

    // Note: This is made nullable so that we can handle cases where
    // no items have been parsed or provided in OCR data.
    public List<BillOfLadingItem>? Items { get; set; }

    public string? SpecialInstructions { get; set; }

    // Optional numeric values
    public decimal? TotalWeight { get; set; }
    public decimal? TotalVolume { get; set; }

    // Signature info
    public string? AuthorizedSignature { get; set; }
    public DateTime? SignedDate { get; set; }
}

public class BillOfLadingItem
{
    public string? Description { get; set; }
    public int? Quantity { get; set; }

    public FreightClass? FreightClass { get; set; }
    public string? NMFCItemNumber { get; set; }

    // Dimensions and weight
    public decimal? Weight { get; set; }
    public decimal? Length { get; set; }
    public decimal? Width { get; set; }
    public decimal? Height { get; set; }

    public HazardousMaterialInfo? HazardousMaterialInfo { get; set; }
}

public class HazardousMaterialInfo
{
    public bool? IsHazardous { get; set; }
    public string? HazardClass { get; set; }
    public string? UNOrNA { get; set; }
    public string? PackingGroup { get; set; }
}

// ----- Enums -----
public enum ShippingMethod
{
    Ground,
    Air,
    Ocean,
    Rail,
    Courier
}

public enum FreightClass
{
    Class50,
    Class55,
    Class60,
    Class65,
    Class70,
    // ...
    Class500
}

public enum FreightTerms
{
    Prepaid,
    Collect,
    ThirdParty
}


