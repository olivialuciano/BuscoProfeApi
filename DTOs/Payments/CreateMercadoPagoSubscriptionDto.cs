namespace BuscoProfe.Api.DTOs.Payments;

public class CreateMercadoPagoSubscriptionDto
{
    public int InstitutionUserId { get; set; }
    public int MembershipId { get; set; }
    public string Reason { get; set; } = "Busco Profe - Membresía";
    public decimal Amount { get; set; }
    public string CurrencyId { get; set; } = "ARS";
    public int Frequency { get; set; } = 1;
    public string FrequencyType { get; set; } = "months";
    public string PayerEmail { get; set; } = string.Empty;
    public string BackUrl { get; set; } = string.Empty;
}