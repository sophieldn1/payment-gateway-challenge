using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services.Interfaces;

public interface IPaymentsService
{
    Task<GetPaymentResponse> GetPayment(Guid id, CancellationToken cancellationToken);
    Task<PaymentResult> InsertPayment(PostPaymentRequest request, CancellationToken cancellationToken);
}
