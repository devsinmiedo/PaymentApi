using PaymentApi.Models;

namespace PaymentApi.Services;

/// <summary>
/// Mock payment provider. Simulates gateway rules for learning — not for production.
/// </summary>
public class PaymentService : IPaymentService
{
    private readonly IPaymentStore _store;
    private readonly ILogger<PaymentService> _logger;

    // Demo cards for concept learning
    private static readonly HashSet<string> SuccessCards =
    [
        "5528790000000008", // Visa success
        "5890040000000016"  // Troy success
    ];

    private static readonly HashSet<string> DeclineCards =
    [
        "5406670000000009" // Forced decline
    ];

    public PaymentService(IPaymentStore store, ILogger<PaymentService> logger)
    {
        _store = store;
        _logger = logger;
    }

    public PaymentResult CreatePayment(CreatePaymentRequest request)
    {
        var validationError = Validate(request);
        if (validationError is not null)
        {
            return new PaymentResult
            {
                Success = false,
                Message = validationError,
                OrderNumber = request.OrderNumber,
                Amount = request.Amount,
                Currency = request.Currency,
                Status = PaymentStatus.Failed
            };
        }

        var existing = _store.GetByOrderNumber(request.OrderNumber);
        if (existing is not null)
        {
            return new PaymentResult
            {
                Success = false,
                Message = "A payment already exists for this order number.",
                PaymentId = existing.Id,
                OrderNumber = existing.OrderNumber,
                Amount = existing.Amount,
                Currency = existing.Currency,
                Status = existing.Status,
                ProviderReference = existing.ProviderReference,
                CreatedAtUtc = existing.CreatedAtUtc
            };
        }

        var normalizedCard = NormalizeCardNumber(request.Card.CardNumber);
        var (succeeded, failureReason) = EvaluateCard(normalizedCard, request.Amount);

        var payment = new Payment
        {
            Id = Guid.NewGuid().ToString("N"),
            OrderNumber = request.OrderNumber,
            Amount = request.Amount,
            Currency = request.Currency.ToUpperInvariant(),
            Status = succeeded ? PaymentStatus.Succeeded : PaymentStatus.Failed,
            FailureReason = failureReason,
            ProviderReference = $"MOCK-{DateTime.UtcNow:yyyyMMddHHmmss}-{Random.Shared.Next(1000, 9999)}",
            CardLastFourDigits = normalizedCard[^4..],
            BuyerEmail = request.Buyer.Email,
            CreatedAtUtc = DateTime.UtcNow,
            CompletedAtUtc = DateTime.UtcNow
        };

        _store.Save(payment);

        _logger.LogInformation(
            "Payment {PaymentId} for order {OrderNumber} -> {Status}",
            payment.Id,
            payment.OrderNumber,
            payment.Status);

        return Map(payment, succeeded
            ? "Payment succeeded."
            : failureReason ?? "Payment failed.");
    }

    public PaymentResult? GetPayment(string paymentId)
    {
        var payment = _store.GetById(paymentId);
        return payment is null
            ? null
            : Map(payment, payment.Status == PaymentStatus.Succeeded
                ? "Payment succeeded."
                : payment.FailureReason ?? "Payment failed.");
    }

    public IReadOnlyList<PaymentResult> ListPayments()
        => _store.GetAll()
            .Select(p => Map(p, p.Status == PaymentStatus.Succeeded
                ? "Payment succeeded."
                : p.FailureReason ?? "Payment failed."))
            .ToList();

    private static string? Validate(CreatePaymentRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.OrderNumber))
            return "OrderNumber is required.";

        if (request.Amount <= 0)
            return "Amount must be greater than zero.";

        if (string.IsNullOrWhiteSpace(request.Currency))
            return "Currency is required.";

        if (string.IsNullOrWhiteSpace(request.Card.CardNumber)
            || string.IsNullOrWhiteSpace(request.Card.CardHolderName)
            || string.IsNullOrWhiteSpace(request.Card.ExpireMonth)
            || string.IsNullOrWhiteSpace(request.Card.ExpireYear)
            || string.IsNullOrWhiteSpace(request.Card.Cvv))
            return "Card information is incomplete.";

        var card = NormalizeCardNumber(request.Card.CardNumber);
        if (card.Length is < 15 or > 16 || !card.All(char.IsDigit))
            return "CardNumber must be 15-16 digits.";

        if (string.IsNullOrWhiteSpace(request.Buyer.FirstName)
            || string.IsNullOrWhiteSpace(request.Buyer.LastName)
            || string.IsNullOrWhiteSpace(request.Buyer.Email)
            || string.IsNullOrWhiteSpace(request.Buyer.Phone))
            return "Buyer information is incomplete.";

        if (string.IsNullOrWhiteSpace(request.BillingAddress.Address)
            || string.IsNullOrWhiteSpace(request.BillingAddress.City))
            return "BillingAddress is incomplete.";

        if (request.Items.Count == 0)
            return "At least one basket item is required.";

        var itemsTotal = request.Items.Sum(i => i.Price * i.Quantity);
        if (Math.Abs(itemsTotal - request.Amount) > 0.01m)
            return $"Basket total ({itemsTotal}) does not match Amount ({request.Amount}).";

        return null;
    }

    private static (bool Succeeded, string? FailureReason) EvaluateCard(string cardNumber, decimal amount)
    {
        if (DeclineCards.Contains(cardNumber))
            return (false, "Card declined by issuer (demo rule).");

        if (SuccessCards.Contains(cardNumber))
            return amount > 50000
                ? (false, "Amount exceeds demo limit for this card.")
                : (true, null);

        // Unknown cards: succeed only when Luhn-valid and amount under limit
        if (!IsLuhnValid(cardNumber))
            return (false, "Invalid card number (Luhn check failed).");

        if (amount >= 10000)
            return (false, "Amount rejected by mock risk engine.");

        return (true, null);
    }

    private static string NormalizeCardNumber(string cardNumber)
        => new(cardNumber.Where(char.IsDigit).ToArray());

    private static bool IsLuhnValid(string number)
    {
        var sum = 0;
        var alternate = false;

        for (var i = number.Length - 1; i >= 0; i--)
        {
            var digit = number[i] - '0';
            if (alternate)
            {
                digit *= 2;
                if (digit > 9)
                    digit -= 9;
            }

            sum += digit;
            alternate = !alternate;
        }

        return sum % 10 == 0;
    }

    private static PaymentResult Map(Payment payment, string message)
        => new()
        {
            Success = payment.Status == PaymentStatus.Succeeded,
            Message = message,
            PaymentId = payment.Id,
            OrderNumber = payment.OrderNumber,
            Amount = payment.Amount,
            Currency = payment.Currency,
            Status = payment.Status,
            ProviderReference = payment.ProviderReference,
            CreatedAtUtc = payment.CreatedAtUtc
        };
}
