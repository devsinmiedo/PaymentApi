namespace PaymentApi.Models;

public class BasketItem
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = "General";
    public decimal Price { get; set; }
    public int Quantity { get; set; } = 1;
}
