using BuscoProfe.Api.DTOs;
using BuscoProfe.Api.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuscoProfe.Api.Entities;

public class Membership
{
    public int Id { get; set; }

    public int InstitutionUserId { get; set; }

    [ForeignKey(nameof(InstitutionUserId))]
    public User InstitutionUser { get; set; } = null!;

    public MembershipPlanType PlanType { get; set; } = MembershipPlanType.Gratis;
    public MembershipStatus Status { get; set; } = MembershipStatus.Activo;

    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime? EndDate { get; set; }

    public string? MercadoPagoSubscriptionId { get; set; }
    public string? MercadoPagoPreferenceId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}