namespace Charlie.Payment.Shared.DTOs;

public class PaymentDTO
{
    public int OrderId { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; }
    public DateTime ProcessedAt { get; set; }
}