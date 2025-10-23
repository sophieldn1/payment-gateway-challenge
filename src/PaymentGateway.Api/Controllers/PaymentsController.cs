using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;
using PaymentGateway.Api.Services.Interfaces;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController(IPaymentsService paymentsService) : Controller
{
    /// <summary>
    /// Get a payment by ID
    /// </summary>
    /// <param name="id">Unique identifier of the payment</param>
    /// <returns>Payment status</returns>
    /// <response code="200">Payment found</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="503">Service unavailable</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status503ServiceUnavailable)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetPaymentResponse?>> GetPayment(Guid id, CancellationToken cancellationToken)
    {
        var payment = await paymentsService.GetPayment(id, cancellationToken);
        if (payment == null) return NotFound(new { message = $"Payment with ID '{id}' was not found." });

        return Ok(payment);
    }

    /// <summary>
    /// Insert a payment
    /// </summary>
    /// <returns>Payment status</returns>
    /// <response code="200">Payment created successfully</response>
    /// <response code="401">Unauthorized</response>
    //  <response code="400">Validation failed</response>
    /// <response code="503">Service unavailable</response>
    [HttpPost("create")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<PaymentResult?>> CreatePayment([FromBody]PostPaymentRequest request, CancellationToken cancellationToken)
    {
        var result = await paymentsService.InsertPayment(request, cancellationToken);
        if (!result.Success)
        {
            return BadRequest(new
            {
                message = "Payment Validation failed",
                errors = result.Errors
            });
        }

        return Ok(result);
    }
}