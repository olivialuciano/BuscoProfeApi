using BuscoProfe.Api.DTOs.Payments;
using BuscoProfe.Api.Entities;
using BuscoProfe.Api.Enums;
using BuscoProfe.Api.Helpers;
using BuscoProfe.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuscoProfe.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IMembershipRepository _membershipRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IMercadoPagoService _mercadoPagoService;

    public PaymentsController(
        IPaymentRepository paymentRepository,
        IMembershipRepository membershipRepository,
        INotificationRepository notificationRepository,
        IMercadoPagoService mercadoPagoService)
    {
        _paymentRepository = paymentRepository;
        _membershipRepository = membershipRepository;
        _notificationRepository = notificationRepository;
        _mercadoPagoService = mercadoPagoService;
    }

    [HttpGet("membership/{membershipId}")]
    [Authorize(Roles = nameof(UserRole.Institution) + "," + nameof(UserRole.Admin))]
    public async Task<ActionResult<List<Payment>>> GetByMembershipId(int membershipId)
    {
        var membership = await _membershipRepository.GetByIdAsync(membershipId);
        if (membership is null) return NotFound();

        var loggedUserId = ClaimsHelper.GetUserId(User);
        var loggedRole = ClaimsHelper.GetRole(User);

        if (loggedUserId is null) return Unauthorized();

        if (loggedRole != nameof(UserRole.Admin) && membership.InstitutionUserId != loggedUserId.Value)
            return Forbid();

        var payments = await _paymentRepository.GetByMembershipIdAsync(membershipId);
        return Ok(payments);
    }

    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<Payment>> Create(CreatePaymentDto dto)
    {
        var membership = await _membershipRepository.GetByIdAsync(dto.MembershipId);
        if (membership is null) return NotFound("Membresía no encontrada.");

        var payment = new Payment
        {
            MembershipId = dto.MembershipId,
            Amount = dto.Amount,
            Currency = dto.Currency,
            Status = PaymentStatus.Pendiente,
            ExternalReference = dto.ExternalReference
        };

        await _paymentRepository.AddAsync(payment);
        return Ok(payment);
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult> UpdateStatus(int id, UpdatePaymentStatusDto dto)
    {
        var payment = await _paymentRepository.GetByIdAsync(id);
        if (payment is null) return NotFound();

        payment.Status = dto.Status;
        payment.MercadoPagoPaymentId = dto.MercadoPagoPaymentId;
        payment.UpdatedAt = DateTime.UtcNow;

        if (dto.Status == PaymentStatus.Aprobado)
            payment.PaidAt = DateTime.UtcNow;

        await _paymentRepository.UpdateAsync(payment);

        if (dto.Status == PaymentStatus.Rechazado)
        {
            var membership = await _membershipRepository.GetByIdAsync(payment.MembershipId);
            if (membership is not null)
            {
                membership.Status = MembershipStatus.Expired;
                membership.EndDate = DateTime.UtcNow;
                membership.UpdatedAt = DateTime.UtcNow;
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
                    Title = "Pago rechazado",
                    Message = "Tu pago fue rechazado. Tu cuenta volvió al plan FREE.",
                    Type = NotificationType.Rojo
                });
            }
        }

        return NoContent();
    }

    [HttpPost("mercadopago/subscription")]
    [Authorize(Roles = nameof(UserRole.Institution) + "," + nameof(UserRole.Admin))]
    public async Task<ActionResult<MercadoPagoSubscriptionResponseDto>> CreateMercadoPagoSubscription(CreateMercadoPagoSubscriptionDto dto)
    {
        var loggedUserId = ClaimsHelper.GetUserId(User);
        var loggedRole = ClaimsHelper.GetRole(User);

        if (loggedUserId is null) return Unauthorized();

        if (loggedRole != nameof(UserRole.Admin) && loggedUserId.Value != dto.InstitutionUserId)
            return Forbid();

        var result = await _mercadoPagoService.CreateSubscriptionAsync(dto);

        if (!result.Success && !result.IsStandBy)
            return BadRequest(result);

        return Ok(result);
    }
}