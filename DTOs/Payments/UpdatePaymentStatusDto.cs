using BuscoProfe.Api.Enums;

namespace BuscoProfe.Api.DTOs.Payments;

public class UpdatePaymentStatusDto
{
    public PaymentStatus Status { get; set; }
    public string? MercadoPagoPaymentId { get; set; }
}