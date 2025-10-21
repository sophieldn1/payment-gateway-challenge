using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Models;

public static class PaymentMapper
{
    public static GetPaymentResponse MapToGetPaymentResponse(PaymentEntity payment)
    {
        return new GetPaymentResponse
        {
            Id = payment.Id,
            Amount = payment.Amount,
            Status = payment.Status,
            MaskedCardNumber = MaskCardNumber(payment.CardNumber.ToString()),
            ExpiryMonth = payment.ExpiryMonth,
            ExpiryYear = payment.ExpiryYear,
            Currency = payment.Currency,
        };
    }

    private static string MaskCardNumber(string cardNumber)
    {
        if (string.IsNullOrEmpty(cardNumber) || cardNumber.Length < 4)
            return "****";

        return new string('*', cardNumber.Length - 4) + cardNumber[^4..];
    }

    public static string MapStatusToText(PaymentStatus status)
    {
        return status switch
        {
            PaymentStatus.Authorized => "Authorized",
            PaymentStatus.Declined => "Declined",
            PaymentStatus.Rejected => "Rejected",
            _ => "Unknown"
        };
    }

}
