namespace OcrDemo.Core.Models;
using System;
using System.Collections.Generic;

/// <summary>
/// Represents an invoice issued by a shipping carrier.
/// </summary>
public class Invoice : IStructuredDocumentParent
{
    /// <summary>
    /// Unique identifier or code for the invoice.
    /// </summary>
    public string? InvoiceNumber { get; set; }

    /// <summary>
    /// Date the invoice was generated or became effective.
    /// </summary>
    public DateTime? InvoiceDate { get; set; }

    /// <summary>
    /// Date the payment is due. If not provided by OCR, remains null.
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// (Optional) Reference to a Bill of Lading number if this invoice
    /// pertains to a specific BOL. 
    /// </summary>
    public string? BillOfLadingNumber { get; set; }

    /// <summary>
    /// The entity responsible for paying the invoice.
    /// </summary>
    public Party? Payer { get; set; }

    /// <summary>
    /// The entity issuing the invoice (carrier, 3PL, or freight forwarder).
    /// </summary>
    public Party? Carrier { get; set; }

    /// <summary>
    /// Payment terms or conditions (e.g. NET 30, NET 60).
    /// </summary>
    public string? PaymentTerms { get; set; }

    /// <summary>
    /// Any special instructions, notes, or remarks related to the invoice.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// A collection of detailed charges or line items for the invoice.
    /// </summary>
    public List<InvoiceLineItem>? LineItems { get; set; }

    /// <summary>
    /// Subtotal before taxes or additional fees.
    /// </summary>
    public decimal? Subtotal { get; set; }

    /// <summary>
    /// Total amount of tax applied, if applicable.
    /// </summary>
    public decimal? TaxAmount { get; set; }

    /// <summary>
    /// Sum of all amounts (Subtotal + TaxAmount, plus any other fees).
    /// </summary>
    public decimal? TotalAmount { get; set; }

    /// <summary>
    /// Status of the invoice (e.g., Paid, Unpaid, PartiallyPaid).
    /// Optional if you do not track status in OCR.
    /// </summary>
    public InvoiceStatus? Status { get; set; }
}

/// <summary>
/// A single line item or charge within the invoice.
/// </summary>
public class InvoiceLineItem
{
    /// <summary>
    /// Description or name of the charge (e.g. "Freight Charge", "Fuel Surcharge").
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Quantity of the charge or units. Often 1 for a flat rate, but can be multiple.
    /// </summary>
    public int? Quantity { get; set; }

    /// <summary>
    /// Price per unit if multiple items or a single rate for one charge.
    /// </summary>
    public decimal? UnitPrice { get; set; }

    /// <summary>
    /// ChargeType can help categorize line items (e.g., Freight, Surcharge, Fee).
    /// </summary>
    public ChargeType? ChargeType { get; set; }

    /// <summary>
    /// (Optional) If taxes or fees are applied individually, you can track them here.
    /// </summary>
    public decimal? TaxAmount { get; set; }

    /// <summary>
    /// Calculated or explicit total for this line item (Quantity * UnitPrice).
    /// Might need separate logic if multiple fees or surcharges apply.
    /// </summary>
    public decimal? LineTotal { get; set; }
}

/// <summary>
/// Basic party information. Could be reused from the BillOfLading classes,
/// but defined separately here for clarity.
/// </summary>




// ----- Enums -----
public enum InvoiceStatus
{
    Paid,
    Unpaid,
    PartiallyPaid,
    Cancelled
}

public enum ChargeType
{
    Freight,
    FuelSurcharge,
    AccessorialFee,
    CustomsDuty,
    Tax,
    Other
}

