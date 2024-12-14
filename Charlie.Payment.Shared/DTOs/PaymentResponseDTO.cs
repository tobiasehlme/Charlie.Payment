namespace Charlie.Payment.Shared.DTOs;

public class PaymentResponseDTO
{
    public string CorrelationId { get; set; }
    public string Status { get; set; }
    public string Message { get; set; }
    public PaymentDTO? Payload { get; set; }
}