using Charlie.Payment.DataAccess.Repostiory;
using Charlie.Payment.Shared.DTOs;
using Charlie.Payment.Shared.Models;
using Microsoft.Extensions.Logging;

namespace Charlie.Payment.Service;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository<PaymentModel> _paymentRepository;
    private ILogger<PaymentService> _logger;

    public PaymentService(IPaymentRepository<PaymentModel> paymentRepository, ILogger<PaymentService> logger)
    {
        _paymentRepository = paymentRepository;
        _logger = logger;
    }

    public PaymentDTO MapModelToDTO(PaymentModel payment)
    {
        return new PaymentDTO()
        {
            Amount = payment.Amount,
            OrderId = payment.OrderId,
            ProcessedAt = payment.ProcessedAt,
            Status = payment.Status
        };
    }

    public PaymentModel MapDTOToModel(PaymentDTO payment)
    {
        return new PaymentModel()
        {
            Id = Guid.NewGuid().GetHashCode(),
            Amount = payment.Amount,
            OrderId = payment.OrderId,
            ProcessedAt = payment.ProcessedAt,
            Status = payment.Status
        };
    }

    public async Task<IEnumerable<PaymentModel>> GetPaymentsAsync()
    {
        _logger.LogInformation("Getting all payments...");
        return await _paymentRepository.GetPaymentsAsync();
    }

    public async Task<PaymentModel> GetPaymentByOrderIdAsync(int id)
    {
        _logger.LogInformation($"Getting payment with id: {id}");
        return await _paymentRepository.GetPaymentByOrderIdAsync(id);
    }

    public async Task AddPaymentAsync(PaymentModel payment)
    {
        _logger.LogInformation($"Adding a new payment");
        await _paymentRepository.AddPaymentAsync(payment);
    }

    public async Task UpdatePaymentAsync(PaymentModel payment)
    {
        _logger.LogInformation($"Updating payment with id: {payment.Id}");
        await _paymentRepository.UpdatePaymentAsync(payment);
    }

    public async Task ProcessPaymentAsync(PaymentModel payment)
    {
        payment.Status = "Processed";
        payment.ProcessedAt = DateTime.Now;
        await UpdatePaymentAsync(payment);
    }

    public async Task RejectPaymentAsync(PaymentModel payment)
    {
        payment.Status = "Rejected";
        payment.ProcessedAt = DateTime.Now;
        await UpdatePaymentAsync(payment);
    }

    public async Task CancelPaymentAsync(PaymentModel payment)
    {
        payment.Status = "Cancelled";
        payment.ProcessedAt = DateTime.Now;
        await UpdatePaymentAsync(payment);
    }

    public async Task CompletePaymentAsync(PaymentModel payment)
    {
        payment.Status = "Cancelled";
        payment.ProcessedAt = DateTime.Now;
        await UpdatePaymentAsync(payment);
    }
}