using PaymentApi.Models;

namespace PaymentApi.Services;

public interface IPaymentService
{
    PaymentResult CreatePayment(CreatePaymentRequest request);
    PaymentResult? GetPayment(string paymentId);
    IReadOnlyList<PaymentResult> ListPayments();
}
