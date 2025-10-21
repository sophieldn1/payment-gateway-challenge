

using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.DataAccess.Repositories.Interfaces;

public interface IPaymentsRepository
{
    Task<GetPaymentResponse?> GetPayment(Guid id, CancellationToken cancellationToken);
    Task InsertPayment(PaymentEntity payment, CancellationToken cancellationToken);
}
