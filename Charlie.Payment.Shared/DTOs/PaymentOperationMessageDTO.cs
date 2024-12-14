namespace Charlie.Payment.Shared.DTOs;

public class PaymentOperationMessageDTO
{
    public string CorrelationId { get; set; }
    public string Operation { get; set; }
    public PaymentDTO Payload { get; set; }
}