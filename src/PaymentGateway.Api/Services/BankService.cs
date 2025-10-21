
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services.Interfaces;

namespace PaymentGateway.Api.Services;

public class BankService(HttpClient client): IBankService
{
    private readonly HttpClient _httpClient = client;

    public async Task<BankResponse> ProcessPayment(PostPaymentRequest payment, CancellationToken cancellationToken)
    {
        var request = new PostPaymentRequest
        {
            CardNumber = payment.CardNumber,
            ExpiryDate = payment.ExpiryDate,
            Currency = payment.Currency,
            Amount = payment.Amount,
            Cvv = payment.Cvv
        };

        var response = await _httpClient.PostAsJsonAsync("payments", request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Bank Service has errored: {response.StatusCode}");
        }

        var result = await response.Content.ReadFromJsonAsync<BankResponse>(cancellationToken: cancellationToken);
        return result;
    }
}