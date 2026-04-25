using BuscoProfe.Api.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuscoProfe.Api.Entities;

public class Payment
{
    public int Id { get; set; }

    public int MembershipId { get; set; }

    [ForeignKey(nameof(MembershipId))]
    public Membership Membership { get; set; } = null!;

    public decimal Amount { get; set; }

    [MaxLength(10)]
    public string Currency { get; set; } = "ARS";

    public PaymentStatus Status { get; set; } = PaymentStatus.Pendiente;

    [MaxLength(200)]
    public string? MercadoPagoPaymentId { get; set; }

    [MaxLength(200)]
    public string? ExternalReference { get; set; }

    public DateTime? PaidAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}