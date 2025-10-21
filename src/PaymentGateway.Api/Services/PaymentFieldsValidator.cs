using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Services.Interfaces;
using System.Globalization;

namespace PaymentGateway.Api.Services;

public class PaymentFieldsValidator : IPaymentFieldsValidator
{
    public Task<ValidationResult> PaymentFieldValidator(PostPaymentRequest payment)
    {


        var errorReasons = new List<string>
        {
            CheckCardNumber(payment),
            ValidateExpiryDate(payment),
            CheckCurrency(payment),
            CheckAmount(payment),
            CheckCvv(payment),

        }
        .Where(reason => !string.IsNullOrWhiteSpace(reason)) // remove nulls
        .ToList();

        var result = new ValidationResult
        {
            Errors = errorReasons
        };
        return Task.FromResult(result);
    }

    private string CheckCardNumber(PostPaymentRequest payment)
    {
        if (string.IsNullOrWhiteSpace(payment.CardNumber))
        {
            return PaymentFailureReasons.NoCardNumber;
        }

        string cardNumber = payment.CardNumber.Trim();

        if (cardNumber.Length < 14 || cardNumber.Length > 19)
        {
            return PaymentFailureReasons.InvalidCardNumberLength;
        }

        if (!cardNumber.All(char.IsDigit))
        {
            return PaymentFailureReasons.InvalidCardNumberCharacters;
        }
        return null;
    }

    private string ValidateExpiryDate(PostPaymentRequest payment)
    {
        const string format = "MM/yyyy";

        if (string.IsNullOrWhiteSpace(payment.ExpiryDate))
        {
            return PaymentFailureReasons.MissingExpiryDate;
        }

        if (!DateTime.TryParseExact(
            payment.ExpiryDate,
            format,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out DateTime expiryDate))
        {
            return PaymentFailureReasons.InvalidExpiryFormat; // e.g. "Expiry date must be in MM/yyyy format."
        }

        int month = expiryDate.Month;
        int year = expiryDate.Year;

        // Validate month range
        if (month < 1 || month > 12)
        {
            return PaymentFailureReasons.InvalidExpiryMonth;
        }

        var now = DateTime.UtcNow;

        // If expiry year is in the past
        if (year < now.Year)
        {
            return PaymentFailureReasons.ExpiredCard;
        }

        // If expiry year is this year and month has passed
        if (year == now.Year && month < now.Month)
        {
            return PaymentFailureReasons.ExpiredCard;
        }

        return null;
    }

    private string CheckAmount(PostPaymentRequest payment)
    {
        if (payment.Amount <= 0)
        {
            return PaymentFailureReasons.InvalidAmount;
        }
        return null;
    }

    private string CheckCvv(PostPaymentRequest payment)
    {
        string cvvString = payment.Cvv.ToString();
        if (string.IsNullOrWhiteSpace(cvvString))
        {
            return PaymentFailureReasons.MissingCvv;
        }

        if (cvvString.Length != 3 || !cvvString.All(char.IsDigit))
        {
            return PaymentFailureReasons.InvalidCvv;
        }
        return null;
    }

    private static readonly HashSet<string> ValidCurrencies = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "USD", "EUR", "GBP", "JPY", "AUD", "CAD", "CHF", "CNY", "SEK", "NZD",
    };

    private string CheckCurrency(PostPaymentRequest payment)
    {
        if (string.IsNullOrWhiteSpace(payment.Currency) || payment.Currency.Length != 3)
        {
            return PaymentFailureReasons.InvalidCurrencyLength;
        }

        if (!ValidCurrencies.Contains(payment.Currency.ToUpperInvariant()))
        {
            return PaymentFailureReasons.InvalidCurrency;
        }

        return null;
    }

}