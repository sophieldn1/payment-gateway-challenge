using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using PaymentGateway.Api.Controllers;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.DataAccess.Repositories;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Services.Interfaces;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.DataAccess.Repositories.Interfaces;
using Xunit.Abstractions;

namespace PaymentGateway.Api.Tests;

public class PaymentsControllerTests(ITestOutputHelper output)
{
    private readonly Random _random = new();
    private readonly ITestOutputHelper _output = output;

    [Fact]
    public async Task RetrievesAPaymentSuccessfully()
    {
        // Arrange
        var paymentId = Guid.NewGuid();

        var payment = new PaymentEntity
        {
            Id = paymentId,
            ExpiryYear = _random.Next(2023, 2030),
            ExpiryMonth = _random.Next(1, 12),
            Amount = _random.Next(1, 10000),
            CardNumber = _random.Next(1111, 9999).ToString(),
            Currency = "GBP",
            Status = PaymentStatus.Authorized
        };

        var mockPaymentsRepository = new PaymentsRepository();
        mockPaymentsRepository.InsertPayment(payment, CancellationToken.None);

        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(services => ((ServiceCollection)services)
                .AddSingleton(mockPaymentsRepository)))
            .CreateClient();

        // Act
        var response = await client.GetAsync($"/api/Payments/{payment.Id}");
        var paymentResponse = await response.Content.ReadFromJsonAsync<GetPaymentResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(paymentId, paymentResponse.Id);
        Assert.NotNull(paymentResponse);
    }

    [Fact]
    public async Task Returns404IfPaymentNotFound()
    {
        // Arrange
        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.CreateClient();

        // Act
        var response = await client.GetAsync($"/api/Payments/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreatesAPaymentSuccessfully()

    {
        // Arrange
        var mockBankService = new Mock<IBankService>();
        mockBankService.Setup(b => b.ProcessPayment(It.IsAny<PostPaymentRequest>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(new BankResponse
        {
            Authorized = true,
            AuthorizationCode = Guid.NewGuid(),
        });

        var paymentsRepository = new Mock<IPaymentsRepository>();

        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(services => ((ServiceCollection)services)
                .AddSingleton(paymentsRepository)
                .AddSingleton(mockBankService.Object)))
            .CreateClient();


        var request = new PostPaymentRequest
        {
            CardNumber = "123456789111111",
            ExpiryDate = "12/2025",
            Currency = "GBP",
            Amount = 999,
            Cvv = "123"
        };

        _output.WriteLine($"Request: {request}");

        // Act
        var response = await client.PostAsJsonAsync($"/api/Payments/create", request);
        _output.WriteLine($"RESPONSE ********: {response}");

        var result = await response.Content.ReadFromJsonAsync<PaymentResult>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task CreatePayment_ReturnsBadRequest_WhenValidationFails()

    {
        // Arrange
        var mockBankService = new Mock<IBankService>();
        mockBankService.Setup(b => b.ProcessPayment(It.IsAny<PostPaymentRequest>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(new BankResponse
        {
            Authorized = true,
            AuthorizationCode = Guid.NewGuid(),
        });

        var paymentsRepository = new Mock<IPaymentsRepository>();

        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(services => ((ServiceCollection)services)
                .AddSingleton(paymentsRepository)
                .AddSingleton(mockBankService.Object)))
            .CreateClient();


        var request = new PostPaymentRequest
        {
            CardNumber = "",
            ExpiryDate = "12/2024",
            Currency = "GB",
            Amount = 0,
            Cvv = "12"
        };

        _output.WriteLine($"Request: {request}");

        // Act
        var response = await client.PostAsJsonAsync($"/api/Payments/create", request);
        _output.WriteLine($"RESPONSE ********: {response}");

        var result = await response.Content.ReadFromJsonAsync<PaymentResult>();

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}