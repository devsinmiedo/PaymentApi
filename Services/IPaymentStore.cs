using PaymentApi.Models;

namespace PaymentApi.Services;

public interface IPaymentStore
{
    void Save(Payment payment);
    Payment? GetById(string paymentId);
    Payment? GetByOrderNumber(string orderNumber);
    IReadOnlyList<Payment> GetAll();
}
