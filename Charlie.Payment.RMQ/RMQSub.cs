using System.Text;
using System.Text.Json;
using Charlie.Payment.DataAccess.Repostiory;
using Charlie.Payment.Service;
using Charlie.Payment.Shared.DTOs;
using Charlie.Payment.Shared.Models;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Charlie.Payment.RMQ;

public class RMQSub : BackgroundService
{
    private readonly ILogger<RMQSub> _logger;
    private readonly RabbitMqClient _rabbitMqClient;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public RMQSub(ILogger<RMQSub> logger, RabbitMqClient rabbitMqClient, IServiceScopeFactory factory)
    {
        _logger = logger;
        _rabbitMqClient = rabbitMqClient;
        _serviceScopeFactory = factory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker is subscribing to customer.operations queue...");

        await _rabbitMqClient.SubscribeAsync("payment.operations", async message =>
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var _paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();

                try
                {
                    _logger.LogInformation($"Worker received message: {message}");


                    var operation = JsonSerializer.Deserialize<PaymentOperationMessageDTO>(message);

                    // Check if operation is not null and has the expected properties
                    if (operation != null)
                    {
                        string correlationId = operation.CorrelationId;
                        string operationType = operation.Operation;
                        var payment = operation.Payload;

                        _logger.LogInformation($"Processing operation for CorrelationId: {correlationId}");

                        if (operationType == "Create")
                        {
                            var paymentModel = _paymentService.MapDTOToModel(payment);
                            await _paymentService.AddPaymentAsync(paymentModel);

                            var response = new PaymentResponseDTO
                            {
                                CorrelationId = correlationId,
                                Status = "Processed",
                                Message = "Customer created successfully"
                            };
                            await _rabbitMqClient.PublishAsync("payment.responses", response);
                            _logger.LogInformation($"Response sent for CorrelationId: {response.CorrelationId}");
                        }
                        else if (operationType == "Read")
                        {

                            var existingPayment = await _paymentService.GetPaymentByOrderIdAsync(payment.Id);

                            PaymentDTO paymentDTO = null;
                            string status = "Failed";
                            string responseMessage = "Payment not found";

                            if (existingPayment != null)
                            {
                                paymentDTO = _paymentService.MapModelToDTO(existingPayment);
                                status = "Processed";
                                message = "Payment retrieved successfully";
                            }

                            var response = new PaymentResponseDTO
                            {
                                CorrelationId = correlationId,
                                Status = "Processed",
                                Message = responseMessage,
                                Payload = paymentDTO
                            };
                            await _rabbitMqClient.PublishAsync("payment.responses", response);
                            _logger.LogInformation($"Response sent for CorrelationId: {response.CorrelationId}");
                        }
                        else
                        {
                            var response = new PaymentResponseDTO
                            {
                                CorrelationId = correlationId,
                                Status = "Failed",
                                Message = "Unknown operation type"
                            };
                            await _rabbitMqClient.PublishAsync("payment.responses", response);
                            _logger.LogWarning($"Unknown operation type for CorrelationId: {correlationId}");
                        }

                    }
                    else
                    {
                        _logger.LogWarning("Received an invalid request message (null request).");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error processing message: {ex.Message}");
                }
            }
            
        }, stoppingToken);
    }
}
