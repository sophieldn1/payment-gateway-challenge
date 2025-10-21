
using PaymentGateway.Api.Models.Responses;

public class PaymentResult
{
    public bool Success { get; set; }
    public List<string>? Errors { get; set; }
    public PostPaymentResponse Response { get; set; }
}