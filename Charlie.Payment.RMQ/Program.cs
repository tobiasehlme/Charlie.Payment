using Charlie.Payment.DataAccess;
using Charlie.Payment.DataAccess.Repostiory;
using Charlie.Payment.RMQ;
using Charlie.Payment.Service;
using Charlie.Payment.Shared.Models;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("PaymentDb");

builder.Services.AddDbContext<PaymentDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddSingleton<RabbitMqClient>();


builder.Services.AddScoped<IPaymentRepository<PaymentModel>, PaymentRepository>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

builder.Services.AddSingleton<IHostedService, RMQSub>();

var host = builder.Build();
host.Run();
