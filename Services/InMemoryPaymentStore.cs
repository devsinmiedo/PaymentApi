using System.Collections.Concurrent;
using PaymentApi.Models;

namespace PaymentApi.Services;

public class InMemoryPaymentStore : IPaymentStore
{
    private readonly ConcurrentDictionary<string, Payment> _payments = new();

    public void Save(Payment payment)
        => _payments[payment.Id] = payment;

    public Payment? GetById(string paymentId)
        => _payments.TryGetValue(paymentId, out var payment) ? payment : null;

    public Payment? GetByOrderNumber(string orderNumber)
        => _payments.Values.FirstOrDefault(p =>
            string.Equals(p.OrderNumber, orderNumber, StringComparison.OrdinalIgnoreCase));

    public IReadOnlyList<Payment> GetAll()
        => _payments.Values.OrderByDescending(p => p.CreatedAtUtc).ToList();
}
