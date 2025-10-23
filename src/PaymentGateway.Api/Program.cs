using PaymentGateway.Api.DataAccess.Repositories.Interfaces;
using PaymentGateway.Api.DataAccess.Repositories;
using PaymentGateway.Api.Services;

using PaymentGateway.Api.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<IPaymentsService, PaymentsService>();
builder.Services.AddScoped<IPaymentFieldsValidator, PaymentFieldsValidator>();
builder.Services.AddScoped<IPaymentsRepository, PaymentsRepository>();
builder.Services.AddHttpClient<IBankService, BankService>(client =>
    {
        client.BaseAddress = new Uri("http://localhost:8080/");
    }
);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
