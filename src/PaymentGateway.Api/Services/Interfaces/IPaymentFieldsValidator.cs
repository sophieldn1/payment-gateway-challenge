using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Services.Interfaces;

public interface IPaymentFieldsValidator
{
    Task<ValidationResult> PaymentFieldValidator(PostPaymentRequest payment);
}