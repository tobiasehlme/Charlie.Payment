namespace Charlie.Payment.Shared.Models;

public class PaymentModel
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; }
    public DateTime ProcessedAt { get; set; }
}
