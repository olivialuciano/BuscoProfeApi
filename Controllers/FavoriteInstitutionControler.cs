using BuscoProfe.Api.DTOs.Favorites;
using BuscoProfe.Api.Entities;
using BuscoProfe.Api.Enums;
using BuscoProfe.Api.Helpers;
using BuscoProfe.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuscoProfe.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FavoriteInstitutionsController : ControllerBase
{
    private readonly IFavoriteInstitutionRepository _repository;
    private readonly IUserRepository _userRepository;

    public FavoriteInstitutionsController(
        IFavoriteInstitutionRepository repository,
        IUserRepository userRepository)
    {
        _repository = repository;
        _userRepository = userRepository;
    }

    [HttpGet("professor/{professorUserId}")]
    [Authorize(Roles = nameof(UserRole.Professor) + "," + nameof(UserRole.Admin))]
    public async Task<ActionResult<List<FavoriteInstitution>>> GetByProfessorUserId(int professorUserId)
    {
        var loggedUserId = ClaimsHelper.GetUserId(User);
        var loggedRole = ClaimsHelper.GetRole(User);

        if (loggedUserId is null)
            return Unauthorized();

        if (loggedRole != nameof(UserRole.Admin) && loggedUserId.Value != professorUserId)
            return Forbid();

        var items = await _repository.GetByProfessorUserIdAsync(professorUserId);
        return Ok(items);
    }

    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Professor) + "," + nameof(UserRole.Admin))]
    public async Task<ActionResult> Create(CreateFavoriteInstitutionDto dto)
    {
        try
        {
            var loggedUserId = ClaimsHelper.GetUserId(User);
            var loggedRole = ClaimsHelper.GetRole(User);

            if (loggedUserId is null)
                return Unauthorized();

            if (loggedRole != nameof(UserRole.Admin) && loggedUserId.Value != dto.ProfessorUserId)
                return Forbid();

            var professor = await _userRepository.GetByIdAsync(dto.ProfessorUserId);
            if (professor is null)
                return NotFound("Profesor no encontrado.");

            if (professor.Role != UserRole.Professor)
                return BadRequest("El usuario no es profesor.");

            var institution = await _userRepository.GetByIdAsync(dto.InstitutionUserId);
            if (institution is null)
                return NotFound("Institución no encontrada.");

            if (institution.Role != UserRole.Institution)
                return BadRequest("El usuario favorito indicado no es una institución.");

            var existing = await _repository.GetByProfessorAndInstitutionAsync(dto.ProfessorUserId, dto.InstitutionUserId);
            if (existing is not null)
                return BadRequest("La institución ya está en favoritos.");

            var entity = new FavoriteInstitution
            {
                ProfessorUserId = dto.ProfessorUserId,
                InstitutionUserId = dto.InstitutionUserId
            };

            await _repository.AddAsync(entity);

            return Ok(new
            {
                entity.Id,
                entity.ProfessorUserId,
                entity.InstitutionUserId,
                entity.CreatedAt
            });
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException)
        {
            return BadRequest("No se pudo guardar el favorito por un error de base de datos.");
        }
        catch (Exception)
        {
            return StatusCode(500, "Ocurrió un error interno del servidor.");
        }
    }

    [HttpGet("institution/{institutionUserId}")]
    [Authorize(Roles = nameof(UserRole.Institution) + "," + nameof(UserRole.Admin))]
    public async Task<ActionResult> GetByInstitutionUserId(int institutionUserId)
    {
        var loggedUserId = ClaimsHelper.GetUserId(User);
        var loggedRole = ClaimsHelper.GetRole(User);

        if (loggedUserId is null)
            return Unauthorized();

        if (loggedRole != nameof(UserRole.Admin) && loggedUserId.Value != institutionUserId)
            return Forbid();

        var items = await _repository.GetByInstitutionUserIdAsync(institutionUserId);

        return Ok(items.Select(x => new
        {
            x.Id,
            x.ProfessorUserId,
            x.InstitutionUserId,
            x.CreatedAt
        }));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = nameof(UserRole.Professor) + "," + nameof(UserRole.Admin))]
    public async Task<ActionResult> Delete(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity is null)
            return NotFound();

        var loggedUserId = ClaimsHelper.GetUserId(User);
        var loggedRole = ClaimsHelper.GetRole(User);

        if (loggedUserId is null)
            return Unauthorized();

        if (loggedRole != nameof(UserRole.Admin) && entity.ProfessorUserId != loggedUserId.Value)
            return Forbid();

        await _repository.DeleteAsync(entity);
        return NoContent();
    }

    [HttpDelete("professor/{professorUserId}/institution/{institutionUserId}")]
    [Authorize(Roles = nameof(UserRole.Professor) + "," + nameof(UserRole.Admin))]
    public async Task<ActionResult> DeleteByProfessorAndInstitution(int professorUserId, int institutionUserId)
    {
        var loggedUserId = ClaimsHelper.GetUserId(User);
        var loggedRole = ClaimsHelper.GetRole(User);

        if (loggedUserId is null)
            return Unauthorized();

        if (loggedRole != nameof(UserRole.Admin) && loggedUserId.Value != professorUserId)
            return Forbid();

        var entity = await _repository.GetByProfessorAndInstitutionAsync(professorUserId, institutionUserId);
        if (entity is null)
            return NotFound();

        await _repository.DeleteAsync(entity);
        return NoContent();
    }
}