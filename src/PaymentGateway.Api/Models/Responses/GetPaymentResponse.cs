﻿namespace PaymentGateway.Api.Models.Responses;

public class GetPaymentResponse
{
    public Guid Id { get; set; }
    public required PaymentStatus Status { get; set; }
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
    public required string Currency { get; set; }
    public decimal Amount { get; set; }
    public required string MaskedCardNumber { get; set; }
}