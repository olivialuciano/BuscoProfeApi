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
public class FavoriteJobPostingsController : ControllerBase
{
    private readonly IFavoriteJobPostingRepository _repository;
    private readonly IUserRepository _userRepository;
    private readonly IJobPostingRepository _jobPostingRepository;
    private readonly ILogger<FavoriteJobPostingsController> _logger;

    public FavoriteJobPostingsController(
        IFavoriteJobPostingRepository repository,
        IUserRepository userRepository,
        IJobPostingRepository jobPostingRepository,
        ILogger<FavoriteJobPostingsController> logger)
    {
        _repository = repository;
        _userRepository = userRepository;
        _jobPostingRepository = jobPostingRepository;
        _logger = logger;
    }

    [HttpGet("professor/{professorUserId}")]
    [Authorize(Roles = nameof(UserRole.Professor) + "," + nameof(UserRole.Admin))]
    public async Task<ActionResult> GetByProfessorUserId(int professorUserId)
    {
        try
        {
            var loggedUserId = ClaimsHelper.GetUserId(User);
            var loggedRole = ClaimsHelper.GetRole(User);

            if (loggedUserId is null)
                return Unauthorized();

            if (loggedRole != nameof(UserRole.Admin) && loggedUserId.Value != professorUserId)
                return Forbid();

            var items = await _repository.GetByProfessorUserIdAsync(professorUserId);

            var response = items.Select(x => new
            {
                x.Id,
                x.ProfessorUserId,
                x.JobPostingId,
                x.CreatedAt,
                JobPosting = x.JobPosting == null
                    ? null
                    : new
                    {
                        x.JobPosting.Id,
                        x.JobPosting.InstitutionUserId,
                        x.JobPosting.Title,
                        x.JobPosting.Description,
                        x.JobPosting.RequirementsText,
                        x.JobPosting.BenefitsText,
                        x.JobPosting.WorkMode,
                        x.JobPosting.ContractType,
                        x.JobPosting.Availability,
                        x.JobPosting.Country,
                        x.JobPosting.Province,
                        x.JobPosting.City,
                        x.JobPosting.Address,
                        x.JobPosting.SalaryText,
                        x.JobPosting.Status,
                        x.JobPosting.PublishedAt,
                        x.JobPosting.CreatedAt,
                        x.JobPosting.UpdatedAt,
                        Institution = x.JobPosting.InstitutionUser == null
                            ? null
                            : new
                            {
                                x.JobPosting.InstitutionUser.Id,
                                x.JobPosting.InstitutionUser.TradeName,
                                x.JobPosting.InstitutionUser.LegalName,
                                x.JobPosting.InstitutionUser.City,
                                x.JobPosting.InstitutionUser.Province,
                                x.JobPosting.InstitutionUser.Country,
                                x.JobPosting.InstitutionUser.ProfileImageUrl,
                                x.JobPosting.InstitutionUser.ShortDescription
                            }
                    }
            }).ToList();

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener favoritos de vacantes del profesor {ProfessorUserId}", professorUserId);
            return StatusCode(500, "Ocurrió un error interno del servidor.");
        }
    }

    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Professor) + "," + nameof(UserRole.Admin))]
    public async Task<ActionResult> Create(CreateFavoriteJobPostingDto dto)
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

            var jobPosting = await _jobPostingRepository.GetByIdAsync(dto.JobPostingId);
            if (jobPosting is null)
                return NotFound("Vacante no encontrada.");

            if (jobPosting.Status == JobPostingStatus.Eliminado)
                return BadRequest("No se puede guardar una vacante eliminada.");

            var existing = await _repository.GetByProfessorAndJobPostingAsync(dto.ProfessorUserId, dto.JobPostingId);

            if (existing is not null)
            {
                return Ok(new
                {
                    existing.Id,
                    existing.ProfessorUserId,
                    existing.JobPostingId,
                    existing.CreatedAt
                });
            }

            var entity = new FavoriteJobPosting
            {
                ProfessorUserId = dto.ProfessorUserId,
                JobPostingId = dto.JobPostingId
            };

            await _repository.AddAsync(entity);

            return Ok(new
            {
                entity.Id,
                entity.ProfessorUserId,
                entity.JobPostingId,
                entity.CreatedAt
            });
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
        {
            _logger.LogError(ex, "Error de base de datos al crear favorito de vacante.");
            return BadRequest("No se pudo guardar el favorito por un error de base de datos.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al crear favorito de vacante.");
            return StatusCode(500, "Ocurrió un error interno del servidor.");
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = nameof(UserRole.Professor) + "," + nameof(UserRole.Admin))]
    public async Task<ActionResult> Delete(int id)
    {
        try
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar favorito de vacante {FavoriteId}", id);
            return StatusCode(500, "Ocurrió un error interno del servidor.");
        }
    }

    [HttpDelete("professor/{professorUserId}/jobposting/{jobPostingId}")]
    [Authorize(Roles = nameof(UserRole.Professor) + "," + nameof(UserRole.Admin))]
    public async Task<ActionResult> DeleteByProfessorAndJobPosting(int professorUserId, int jobPostingId)
    {
        try
        {
            var loggedUserId = ClaimsHelper.GetUserId(User);
            var loggedRole = ClaimsHelper.GetRole(User);

            if (loggedUserId is null)
                return Unauthorized();

            if (loggedRole != nameof(UserRole.Admin) && loggedUserId.Value != professorUserId)
                return Forbid();

            var entity = await _repository.GetByProfessorAndJobPostingAsync(professorUserId, jobPostingId);
            if (entity is null)
                return NotFound();

            await _repository.DeleteAsync(entity);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error al eliminar favorito por professorUserId {ProfessorUserId} y jobPostingId {JobPostingId}",
                professorUserId, jobPostingId);
            return StatusCode(500, "Ocurrió un error interno del servidor.");
        }
    }
}