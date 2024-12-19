namespace Charlie.Payment.Shared.DTOs;

public class PaymentDTO
{
    public string Id { get; set; }
    public string CustomerId { get; set; }
    public string CustomerName { get; set; }
    public List<int> ProductIds { get; set; }
    public decimal TotalPrice { get; set; }
}