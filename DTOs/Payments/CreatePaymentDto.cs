namespace BuscoProfe.Api.DTOs.Payments;

public class CreatePaymentDto
{
    public int MembershipId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "ARS";
    public string? ExternalReference { get; set; }
}