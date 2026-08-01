namespace PaymentApi.Models;

public class AddressInfo
{
    public string ContactName { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = "Turkey";
    public string Address { get; set; } = string.Empty;
    public string? ZipCode { get; set; }
}
