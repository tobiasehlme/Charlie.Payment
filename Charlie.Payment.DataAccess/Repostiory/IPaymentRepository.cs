using Charlie.Payment.Shared.Models;

namespace Charlie.Payment.DataAccess.Repostiory;

public interface IPaymentRepository<T> where T : class
{
    Task UpdatePaymentAsync(PaymentModel payment);
    Task<PaymentModel> GetPaymentByOrderIdAsync(int id);
    Task<IEnumerable<PaymentModel>> GetPaymentsAsync();
    Task AddPaymentAsync(PaymentModel payment);
    Task DeletePaymentAsync(int id);
    
}