using Charlie.Payment.Shared.DTOs;
using Charlie.Payment.Shared.Models;

namespace Charlie.Payment.Service;

public interface IPaymentService
{
    PaymentDTO MapModelToDTO(PaymentModel payment);
    PaymentModel MapDTOToModel(PaymentDTO payment);

    Task<IEnumerable<PaymentModel>> GetPaymentsAsync();
    Task<PaymentModel> GetPaymentByOrderIdAsync(string id);

    Task AddPaymentAsync(PaymentModel payment);
    Task UpdatePaymentAsync(PaymentModel payment);

    Task ProcessPaymentAsync(PaymentModel payment);
    Task RejectPaymentAsync(PaymentModel payment);
    Task CancelPaymentAsync(PaymentModel payment);
    Task CompletePaymentAsync(PaymentModel payment);
}