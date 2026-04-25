using BuscoProfe.Api.DTOs.Notifications;
using BuscoProfe.Api.Enums;
using BuscoProfe.Api.Helpers;
using BuscoProfe.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuscoProfe.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationsController(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    [HttpGet("user/{userId}")]
    [Authorize]
    public async Task<ActionResult<List<NotificationResponseDto>>> GetByUserId(int userId)
    {
        var loggedUserId = ClaimsHelper.GetUserId(User);
        var loggedRole = ClaimsHelper.GetRole(User);

        if (loggedUserId is null)
            return Unauthorized();

        if (loggedRole != nameof(UserRole.Admin) && loggedUserId.Value != userId)
            return Forbid();

        var items = await _notificationRepository.GetByUserIdAsync(userId);

        var response = items.Select(x => new NotificationResponseDto
        {
            Id = x.Id,
            Title = x.Title,
            Message = x.Message,
            Type = x.Type,
            IsRead = x.IsRead,
            CreatedAt = x.CreatedAt
        }).ToList();

        return Ok(response);
    }

    [HttpPut("{id}/read")]
    [Authorize]
    public async Task<ActionResult> MarkAsRead(int id)
    {
        var notification = await _notificationRepository.GetByIdAsync(id);
        if (notification is null)
            return NotFound();

        var loggedUserId = ClaimsHelper.GetUserId(User);
        var loggedRole = ClaimsHelper.GetRole(User);

        if (loggedUserId is null)
            return Unauthorized();

        if (loggedRole != nameof(UserRole.Admin) && notification.UserId != loggedUserId.Value)
            return Forbid();

        notification.IsRead = true;
        await _notificationRepository.UpdateAsync(notification);

        return NoContent();
    }
}