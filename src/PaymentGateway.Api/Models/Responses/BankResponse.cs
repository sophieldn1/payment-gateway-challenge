namespace PaymentGateway.Api.Models.Responses;
using System.Text.Json.Serialization;

public class BankResponse
{
    [JsonPropertyName("authorized")]
    public bool Authorized { get; set; }
    [JsonPropertyName("authorization_code")]
    public Guid AuthorizationCode { get; set; }
}