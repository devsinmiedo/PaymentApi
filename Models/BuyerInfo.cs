namespace PaymentApi.Models;

public class BuyerInfo
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? IdentityNumber { get; set; }
    public string? IpAddress { get; set; }
}
