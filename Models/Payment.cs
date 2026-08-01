namespace PaymentApi.Models;

/// <summary>
/// Persisted payment record (in-memory for learning).
/// </summary>
public class Payment
{
    public string Id { get; set; } = string.Empty;
    public string OrderNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "TRY";
    public PaymentStatus Status { get; set; }
    public string? FailureReason { get; set; }
    public string? ProviderReference { get; set; }
    public string? CardLastFourDigits { get; set; }
    public string? BuyerEmail { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
}
