using BuscoProfe.Api.DTOs.ProfessorSkills;
using BuscoProfe.Api.Entities;
using BuscoProfe.Api.Enums;
using BuscoProfe.Api.Helpers;
using BuscoProfe.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuscoProfe.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProfessorSkillsController : ControllerBase
{
    private readonly IProfessorSkillRepository _repository;
    private readonly IUserRepository _userRepository;

    public ProfessorSkillsController(
        IProfessorSkillRepository repository,
        IUserRepository userRepository)
    {
        _repository = repository;
        _userRepository = userRepository;
    }

    [HttpGet("user/{userId}")]
    [Authorize(Roles = nameof(UserRole.Professor) + "," + nameof(UserRole.Admin))]
    public async Task<ActionResult<List<ProfessorSkill>>> GetByUserId(int userId)
    {
        var loggedUserId = ClaimsHelper.GetUserId(User);
        var loggedRole = ClaimsHelper.GetRole(User);

        if (loggedUserId is null) return Unauthorized();

        if (loggedRole != nameof(UserRole.Admin) && loggedUserId.Value != userId)
            return Forbid();

        var items = await _repository.GetByUserIdAsync(userId);
        return Ok(items);
    }

    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Professor) + "," + nameof(UserRole.Admin))]
    public async Task<ActionResult> Create(CreateProfessorSkillDto dto)
    {
        var loggedUserId = ClaimsHelper.GetUserId(User);
        var loggedRole = ClaimsHelper.GetRole(User);

        if (loggedUserId is null) return Unauthorized();

        if (loggedRole != nameof(UserRole.Admin) && loggedUserId.Value != dto.UserId)
            return Forbid();

        var user = await _userRepository.GetByIdAsync(dto.UserId);
        if (user is null) return NotFound("Usuario no encontrado.");
        if (user.Role != UserRole.Professor) return BadRequest("El usuario no es profesor.");

        var entity = new ProfessorSkill
        {
            UserId = dto.UserId,
            Name = dto.Name
        };

        await _repository.AddAsync(entity);

        return Ok(new
        {
            entity.Id,
            entity.UserId,
            entity.Name
        });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = nameof(UserRole.Professor) + "," + nameof(UserRole.Admin))]
    public async Task<ActionResult> Delete(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity is null) return NotFound();

        var loggedUserId = ClaimsHelper.GetUserId(User);
        var loggedRole = ClaimsHelper.GetRole(User);

        if (loggedUserId is null) return Unauthorized();

        if (loggedRole != nameof(UserRole.Admin) && entity.UserId != loggedUserId.Value)
            return Forbid();

        await _repository.DeleteAsync(entity);
        return NoContent();
    }
}