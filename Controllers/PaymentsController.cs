using Microsoft.AspNetCore.Mvc;
using PaymentApi.Models;
using PaymentApi.Services;

namespace PaymentApi.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    /// <summary>
    /// Creates a payment through the mock provider.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(PaymentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(PaymentResult), StatusCodes.Status400BadRequest)]
    public ActionResult<PaymentResult> Create([FromBody] CreatePaymentRequest request)
    {
        var result = _paymentService.CreatePayment(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Gets a payment by id.
    /// </summary>
    [HttpGet("{paymentId}")]
    [ProducesResponseType(typeof(PaymentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<PaymentResult> Get(string paymentId)
    {
        var result = _paymentService.GetPayment(paymentId);
        return result is null ? NotFound(new { Message = "Payment not found." }) : Ok(result);
    }

    /// <summary>
    /// Lists all payments stored in memory.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<PaymentResult>), StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyList<PaymentResult>> List()
        => Ok(_paymentService.ListPayments());
}
