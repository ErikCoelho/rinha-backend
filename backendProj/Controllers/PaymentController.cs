using backendProj.Models;
using backendProj.Services;
using backendProj.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace backendProj.Controllers;

[ApiController]
public class PaymentController : ControllerBase
{
    [HttpPost("payments")]
    public async Task<IActionResult> PostPayment([FromBody] Payment payment, [FromServices] IPaymentQueue paymentQueue,
        CancellationToken cancellationToken)
    {
        await paymentQueue.EnqueueAsync(payment, cancellationToken);
        return Accepted();
    }

    [HttpGet("payments/summary")]
    public async Task<PaymentSummary> GetPaymentsSummary([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var test1 = new PaymentSummary();
        return test1;
    }
}