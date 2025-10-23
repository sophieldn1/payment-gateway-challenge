
using PaymentGateway.Api.DataAccess.Repositories.Interfaces;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services.Interfaces;
using PaymentGateway.Api.Models;
using System.Globalization;

namespace PaymentGateway.Api.Services;

public class PaymentsService(IPaymentsRepository
paymentsRepository, IPaymentFieldsValidator validator, IBankService bankService, ILogger<PaymentsService> logger) : IPaymentsService
{
    private readonly ILogger<PaymentsService> _logger = logger;

    public async Task<GetPaymentResponse> GetPayment(Guid paymentId, CancellationToken cancellationToken)
    {
        try
        {
            var payment = await paymentsRepository.GetPayment(paymentId, cancellationToken);
            if (payment == null) return null;

            _logger.LogInformation("Payment found ${payment}", payment);

            return payment;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving payment with ID: {PaymentId}", paymentId);
            return null;
        }
    }

    public async Task<PaymentResult> InsertPayment(PostPaymentRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var paymentID = Guid.NewGuid();

            // check if existing payment
            var payment = await paymentsRepository.GetPayment(paymentID, cancellationToken);
            if (payment != null)
            {
                _logger.LogInformation("Payment already exists {PaymentId}", payment.Id);

                var existingResponse = new PostPaymentResponse
                {
                    Id = payment.Id,
                    Status = MapStatus(payment.Status),
                    MaskedCardNumber = payment.MaskedCardNumber,
                    ExpiryMonth = payment.ExpiryMonth,
                    ExpiryYear = payment.ExpiryYear,
                    Currency = payment.Currency,
                    Amount = payment.Amount,
                };
                return new PaymentResult
                {
                    Success = true,
                    Response = existingResponse
                };
            }

            // call validation service
            var validationResult = await validator.PaymentFieldValidator(request);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Payment failed validation. {Errors}", string.Join(", ", validationResult.Errors));
                return new PaymentResult
                {
                    Success = false,
                    Errors = validationResult.Errors
                };
            }

            var paymentEntity = MapToPaymentEntity(request, paymentID);

            // call bank service to process payment
            var bankResponse = await bankService.ProcessPayment(request, cancellationToken, logger);
            _logger.LogInformation("Bank response: Authorized={Authorized}", bankResponse);

            paymentEntity.Status = bankResponse.Authorized ? PaymentStatus.Authorized : PaymentStatus.Rejected;

            await paymentsRepository.InsertPayment(paymentEntity, cancellationToken);

            var response = new PostPaymentResponse
            {
                Id = paymentEntity.Id,
                Status = MapStatus(paymentEntity.Status),
                MaskedCardNumber = GetLastFourDigits(paymentEntity.CardNumber),
                ExpiryMonth = paymentEntity.ExpiryMonth,
                ExpiryYear = paymentEntity.ExpiryYear,
                Currency = paymentEntity.Currency,
                Amount = paymentEntity.Amount,
            };
            return new PaymentResult
            {
                Success = true,
                Response = response
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while inserting payment.");
            return new PaymentResult
            {
                Success = false,
                Errors = new List<string> { "An unexpected error occurred while processing the payment." }
            };
        }
    }


    public static string MapStatus(PaymentStatus status)
    {
        return status == PaymentStatus.Authorized
            ? "Authorized"
            : "Declined";
    }
    private PaymentEntity MapToPaymentEntity(PostPaymentRequest request, Guid id)
    {
        const string format = "MM/yyyy";

        if (!DateTime.TryParseExact(request.ExpiryDate, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime expiryDate))
        {
            throw new ValidationException("Invalid expiry date format. Expected MM/yyyy.");
        }


        return new PaymentEntity
        {
            Id = id,
            CardNumber = request.CardNumber,
            ExpiryMonth = expiryDate.Month,
            ExpiryYear = expiryDate.Year,
            Currency = request.Currency,
            Amount = request.Amount,
            Cvv = request.Cvv
        };
    }

    private string GetLastFourDigits(string cardNumber)
    {
        if (string.IsNullOrEmpty(cardNumber) || cardNumber.Length < 4)
        {
            return cardNumber ?? string.Empty;
        }

        return cardNumber.Substring(cardNumber.Length - 4);
    }
}