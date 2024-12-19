using System.Security.Cryptography.X509Certificates;
using Charlie.Payment.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace Charlie.Payment.DataAccess.Repostiory;

public class PaymentRepository : IPaymentRepository<PaymentModel>
{
    private readonly PaymentDbContext _context;

    public PaymentRepository(PaymentDbContext context)
    {
        _context = context;
    }
    public async Task UpdatePaymentAsync(PaymentModel payment)
    {
        _context.Payments.Update(payment);
        await _context.SaveChangesAsync();
    }

    public async Task<PaymentModel> GetPaymentByOrderIdAsync(int id)
    {
        return await _context.Payments.FirstOrDefaultAsync(x=>x.Id == id);
    }

    public async Task<IEnumerable<PaymentModel>> GetPaymentsAsync()
    {
        return await _context.Payments.ToListAsync();
    }

    public async Task AddPaymentAsync(PaymentModel payment)
    {
        _context.Payments.AddAsync(payment);
        await _context.SaveChangesAsync();
    }

    public async Task DeletePaymentAsync(int id)
    {
        var payment = await GetPaymentByOrderIdAsync(id);
        if (payment != null)
        {
            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();
        }
    }
}