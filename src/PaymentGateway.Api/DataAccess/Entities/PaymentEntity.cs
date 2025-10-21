using PaymentGateway.Api.Models;

public class PaymentEntity
{
        public Guid Id { get; set; }

        public PaymentStatus Status { get; set; }

        /// <summary>
        /// The card number 
        /// </summary>
        public string CardNumber { get; set; }

        /// <summary>
        /// Expiry month (1–12)
        /// </summary>
        public int ExpiryMonth { get; set; }

        /// <summary>
        /// Expiry year (e.g., 2026)
        /// </summary>
        public int ExpiryYear { get; set; }

        /// <summary>
        /// Currency code, e.g., "USD", "EUR"
        /// </summary>
        public string Currency { get; set; } = string.Empty;

        /// <summary>
        /// Amount in the smallest currency unit (e.g., cents)
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Masked card number for display (e.g., ************1234)
        /// </summary>
        public string MaskedCardNumber { get; set; } = string.Empty;

        /// <summary>
        /// CVV
        /// </summary>
        public string Cvv { get; set; }
}