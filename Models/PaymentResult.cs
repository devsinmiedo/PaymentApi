namespace PaymentApi.Models;

public class PaymentResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string? PaymentId { get; set; }
    public string? OrderNumber { get; set; }
    public decimal? Amount { get; set; }
    public string? Currency { get; set; }
    public PaymentStatus Status { get; set; }
    public string? ProviderReference { get; set; }
    public DateTime? CreatedAtUtc { get; set; }
}
