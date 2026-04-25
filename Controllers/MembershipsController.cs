using BuscoProfe.Api.DTOs.Memberships;
using BuscoProfe.Api.Entities;
using BuscoProfe.Api.Enums;
using BuscoProfe.Api.Helpers;
using BuscoProfe.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuscoProfe.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MembershipsController : ControllerBase
{
    private readonly IMembershipRepository _membershipRepository;
    private readonly IUserRepository _userRepository;
    private readonly INotificationRepository _notificationRepository;

    public MembershipsController(
        IMembershipRepository membershipRepository,
        IUserRepository userRepository,
        INotificationRepository notificationRepository)
    {
        _membershipRepository = membershipRepository;
        _userRepository = userRepository;
        _notificationRepository = notificationRepository;
    }

    [HttpGet("institution/{institutionUserId}/active")]
    [Authorize(Roles = nameof(UserRole.Institution) + "," + nameof(UserRole.Admin))]
    public async Task<ActionResult<Membership>> GetActiveByInstitution(int institutionUserId)
    {
        var loggedUserId = ClaimsHelper.GetUserId(User);
        var loggedRole = ClaimsHelper.GetRole(User);

        if (loggedUserId is null) return Unauthorized();

        if (loggedRole != nameof(UserRole.Admin) && loggedUserId.Value != institutionUserId)
            return Forbid();

        var membership = await _membershipRepository.GetActiveByInstitutionUserIdAsync(institutionUserId);
        if (membership is null) return NotFound();

        return Ok(membership);
    }

    [HttpGet("institution/{institutionUserId}")]
    [Authorize(Roles = nameof(UserRole.Institution) + "," + nameof(UserRole.Admin))]
    public async Task<ActionResult<List<Membership>>> GetByInstitution(int institutionUserId)
    {
        var loggedUserId = ClaimsHelper.GetUserId(User);
        var loggedRole = ClaimsHelper.GetRole(User);

        if (loggedUserId is null) return Unauthorized();

        if (loggedRole != nameof(UserRole.Admin) && loggedUserId.Value != institutionUserId)
            return Forbid();

        var memberships = await _membershipRepository.GetByInstitutionUserIdAsync(institutionUserId);
        return Ok(memberships);
    }

    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<Membership>> Create(CreateMembershipDto dto)
    {
        var institution = await _userRepository.GetByIdAsync(dto.InstitutionUserId);
        if (institution is null) return NotFound("Institución no encontrada.");
        if (institution.Role != UserRole.Institution) return BadRequest("El usuario no es una institución.");

        var activeMembership = await _membershipRepository.GetActiveByInstitutionUserIdAsync(dto.InstitutionUserId);
        if (activeMembership is not null)
        {
            activeMembership.Status = MembershipStatus.Cancelled;
            activeMembership.UpdatedAt = DateTime.UtcNow;
            await _membershipRepository.UpdateAsync(activeMembership);
        }

        var membership = new Membership
        {
            InstitutionUserId = dto.InstitutionUserId,
            PlanType = dto.PlanType,
            Status = MembershipStatus.Activo,
            StartDate = DateTime.UtcNow,
            EndDate = dto.EndDate
        };

        await _membershipRepository.AddAsync(membership);
        return Ok(membership);
    }

    [HttpPost("{id}/expire-and-downgrade")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult> ExpireAndDowngrade(int id)
    {
        var membership = await _membershipRepository.GetByIdAsync(id);
        if (membership is null) return NotFound();

        membership.Status = MembershipStatus.Expired;
        membership.UpdatedAt = DateTime.UtcNow;
        membership.EndDate = DateTime.UtcNow;
        await _membershipRepository.UpdateAsync(membership);

        var freeMembership = new Membership
        {
            InstitutionUserId = membership.InstitutionUserId,
            PlanType = MembershipPlanType.Gratis,
            Status = MembershipStatus.Activo,
            StartDate = DateTime.UtcNow
        };

        await _membershipRepository.AddAsync(freeMembership);

        await _notificationRepository.AddAsync(new Notification
        {
            UserId = membership.InstitutionUserId,
            Title = "Tu plan volvió a FREE",
            Message = "Tu membresía paga venció o no pudo cobrarse. Tu cuenta volvió al plan FREE.",
            Type = NotificationType.Rojo
        });

        return NoContent();
    }
}