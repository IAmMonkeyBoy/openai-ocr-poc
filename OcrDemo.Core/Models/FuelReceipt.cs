namespace OcrDemo.Core.Models;
using System;
using System.Collections.Generic;

using System;
using System.Collections.Generic;

/// <summary>
/// Represents a single receipt for a fuel purchase (and related items).
/// </summary>
public class FuelReceipt
{
    /// <summary>
    /// Unique identifier for the receipt (e.g. transaction # or store receipt #).
    /// </summary>
    public string? ReceiptNumber { get; set; }

    /// <summary>
    /// Date/time of the purchase, if known.
    /// </summary>
    public DateTime? PurchaseDate { get; set; }

    /// <summary>
    /// The driver who made the purchase.
    /// </summary>
    public Driver? Driver { get; set; }

    /// <summary>
    /// The truck (or vehicle) used at the time of purchase.
    /// </summary>
    public Truck? Truck { get; set; }

    /// <summary>
    /// Where the fuel was purchased (e.g., station info, address, pump).
    /// </summary>
    public PurchaseLocation? PurchaseLocation { get; set; }

    /// <summary>
    /// Form of payment (e.g., Company Card, Cash, etc.).
    /// </summary>
    public string? PaymentMethod { get; set; }

    /// <summary>
    /// Line items purchased (fuel, reefer fuel, DEF, cash advance, etc.).
    /// </summary>
    public List<FuelReceiptLineItem>? Items { get; set; }

    /// <summary>
    /// Subtotal before taxes and discounts (if the receipt total is itemized).
    /// </summary>
    public decimal? Subtotal { get; set; }

    /// <summary>
    /// Taxes applied to the entire receipt.
    /// </summary>
    public decimal? TaxAmount { get; set; }

    /// <summary>
    /// Grand total after taxes, fees, and discounts.
    /// </summary>
    public decimal? TotalAmount { get; set; }

    /// <summary>
    /// Any notes, additional references, or store info captured from OCR.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Arbitrary key/value pairs for extra data that may come from OCR,
    /// such as transaction codes or other metadata that doesn't fit well in other fields.
    /// </summary>
    public Dictionary<string, string>? AdditionalTransactionDetails { get; set; }
}

/// <summary>
/// Represents an individual item on a fuel receipt (e.g. diesel fuel, DEF).
/// </summary>
public class FuelReceiptLineItem
{
    /// <summary>
    /// A brief description of the item (e.g., "Diesel Fuel", "Cash Advance").
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Categorizes the line item (Fuel, Reefer Fuel, DEF, Cash Advance, etc.).
    /// </summary>
    public FuelLineItemCategory? Category { get; set; }

    /// <summary>
    /// Quantity purchased (e.g., gallons or liters of fuel).
    /// </summary>
    public decimal? Quantity { get; set; }

    /// <summary>
    /// Indicates whether Quantity is in gallons or liters (or future expansions).
    /// </summary>
    public VolumeUnit? VolumeUnit { get; set; }

    /// <summary>
    /// Price per unit if applicable. For example, price per gallon of diesel.
    /// </summary>
    public decimal? UnitPrice { get; set; }

    /// <summary>
    /// The total line amount before any discounts.
    /// </summary>
    public decimal? LineSubtotal { get; set; }

    /// <summary>
    /// List of discounts applied to this line item (e.g. loyalty, promotions).
    /// </summary>
    public List<Discount>? Discounts { get; set; }

    /// <summary>
    /// Final total for this line item after discounts.
    /// </summary>
    public decimal? LineTotal { get; set; }
}

/// <summary>
/// Stores detailed info on where the fuel was purchased.
/// </summary>
public class PurchaseLocation
{
    /// <summary>
    /// The station's or vendor's name (e.g., "Pilot", "Love's", "Local Gas Station").
    /// </summary>
    public string? LocationName { get; set; }

    /// <summary>
    /// Physical address of the fuel station.
    /// </summary>
    public Address? LocationAddress { get; set; }

    /// <summary>
    /// The pump number or identifier used for the purchase.
    /// </summary>
    public string? PumpNumber { get; set; }
}

/// <summary>
/// Represents a discount, often applied at the line level (e.g., per gallon discount).
/// </summary>
public class Discount
{
    /// <summary>
    /// The name or type of the discount (e.g., "Rewards Discount", "Promo Code").
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// The monetary amount of the discount.
    /// </summary>
    public decimal? Amount { get; set; }
}

/// <summary>
/// Basic info about the driver who purchased the fuel.
/// </summary>
public class Driver
{
    public string? DriverId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    /// <summary>
    /// Could store the CDL number or other relevant identification.
    /// </summary>
    public string? LicenseNumber { get; set; }

    /// <summary>
    /// Optional contact phone or email for the driver.
    /// </summary>
    public string? ContactInfo { get; set; }
}

/// <summary>
/// Basic info about the truck or vehicle used during the purchase.
/// </summary>
public class Truck
{
    public string? TruckId { get; set; }
    public string? LicensePlate { get; set; }
    public string? Make { get; set; }
    public string? Model { get; set; }
    public int? Year { get; set; }

    /// <summary>
    /// A trucking company's internal ID for the vehicle.
    /// </summary>
    public string? UnitNumber { get; set; }
}



/// <summary>
/// Enum categorizing fuel-related line items.
/// </summary>
public enum FuelLineItemCategory
{
    Fuel,
    ReeferFuel,
    DEF,
    CashAdvance,
    Oil,
    WasherFluid,
    Additive,
    Other
}

/// <summary>
/// Defines the volume units (gallons or liters).
/// </summary>
public enum VolumeUnit
{
    Gallons,
    Liters
}
