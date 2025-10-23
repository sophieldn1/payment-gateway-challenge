using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Services.Interfaces;

public interface IBankService
{
    Task<BankResponse> ProcessPayment(PostPaymentRequest request, CancellationToken cancellationToken, ILogger logger);
}