namespace BuscoProfe.Api.DTOs.Payments;

public class MercadoPagoSubscriptionResponseDto
{
    public bool IsStandBy { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? InitPoint { get; set; }
    public string? MercadoPagoSubscriptionId { get; set; }
}