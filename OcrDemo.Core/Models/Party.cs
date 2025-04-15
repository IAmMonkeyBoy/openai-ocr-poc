namespace OcrDemo.Core.Models;

public class Party
{
  public string? Name { get; set; }
  public Address? Address { get; set; }
  public string? ContactName { get; set; }
  public string? PhoneNumber { get; set; }
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
