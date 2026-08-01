namespace PaymentApi.Models;

public class CreatePaymentRequest
{
    public string OrderNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "TRY";
    public string? Description { get; set; }
    public bool Use3DSecure { get; set; } = true;
    public string? CallbackUrl { get; set; }
    public CardInfo Card { get; set; } = new();
    public BuyerInfo Buyer { get; set; } = new();
    public AddressInfo BillingAddress { get; set; } = new();
    public List<BasketItem> Items { get; set; } = new();
}
