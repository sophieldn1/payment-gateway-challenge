using PaymentGateway.Api.DataAccess.Repositories.Interfaces;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Models;

namespace PaymentGateway.Api.DataAccess.Repositories;

public class PaymentsRepository() : IPaymentsRepository
{
    private static readonly List<PaymentEntity> Payments = new()
    {
        new PaymentEntity
        {
            Id = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
            Status = PaymentStatus.Authorized,
            ExpiryMonth = 12,
            ExpiryYear = 2026,
            Currency = "USD",
            Amount = 10000,
            CardNumber = "123456789"
        }
    };


    public Task<GetPaymentResponse?> GetPayment(Guid id, CancellationToken cancellationToken)
    {
        var payment = Payments.FirstOrDefault(p => p.Id == id);
        // logger.LogInfo("Payment found in getPayment Repo", payment);


        GetPaymentResponse? response = payment is null
            ? null
            : PaymentMapper.MapToGetPaymentResponse(payment);

        // logger.LogInfo("Payment mapped response", response);

        return Task.FromResult(response);
    }

    public Task InsertPayment(PaymentEntity payment, CancellationToken cancellationToken)
    {
        Payments.Add(payment);

        return Task.CompletedTask;
    }
}
