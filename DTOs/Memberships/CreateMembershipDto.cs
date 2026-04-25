using BuscoProfe.Api.Enums;

namespace BuscoProfe.Api.DTOs.Memberships;

public class CreateMembershipDto
{
    public int InstitutionUserId { get; set; }
    public MembershipPlanType PlanType { get; set; }
    public DateTime? EndDate { get; set; }
}