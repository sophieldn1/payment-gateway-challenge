
public class PaymentFailureReasons
{
    public const string NoCardNumber = "No card number provided";
    public const string InvalidAmount = "Invalid amount";

    public const string MissingCvv = "CVV is required";

    public const string InvalidCvv = "CVV must be 3-4 characters long and contain only numeric values";

    public const string InvalidCurrencyLength = "Currency must be 3 characters";

    public const string InvalidCurrency = "Currency is not recognised";

    public const string ExpiredCard = "Card expiry date cannot be in the past";

    public const string InvalidExpiryMonth = "Invalid expiry month";

    public const string InvalidCardNumberLength = "Card number must be between 14-19 characters long";

    public const string InvalidCardNumberCharacters = "Invalid characters in card";

    public const string InvalidExpiryFormat = "Expiry date incorrect format";

    public const string MissingExpiryDate = "Expiry date required";
}