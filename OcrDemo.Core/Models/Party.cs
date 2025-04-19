namespace OcrDemo.Core.Models;

public class Party
{
  [FieldPriority(FieldPriority.High)]
  public string? Name { get; set; }
  public Address? Address { get; set; }
  [FieldPriority(FieldPriority.High)]
  public string? ContactName { get; set; }
  [FieldPriority(FieldPriority.High)]
  public string? PhoneNumber { get; set; }
  [FieldPriority(FieldPriority.High)]
  public string? Email { get; set; }
  public PartyRole? Role { get; set; }
  public string? MCNumber { get; set; }
  public string? DOTNumber { get; set; }
}






public enum PartyRole
{
  Shipper,
  Carrier,
  Consignee,
  NotifyParty,
  Other,
  Payer,
}
