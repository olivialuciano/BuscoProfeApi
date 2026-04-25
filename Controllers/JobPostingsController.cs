using BuscoProfe.Api.DTOs.JobPostings;
using BuscoProfe.Api.Entities;
using BuscoProfe.Api.Enums;
using BuscoProfe.Api.Helpers;
using BuscoProfe.Api.Interfaces;
using BuscoProfe.Api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuscoProfe.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobPostingsController : ControllerBase
{
    private readonly IJobPostingRepository _jobPostingRepository;
    private readonly IUserRepository _userRepository;
    private readonly ISportRepository _sportRepository;
    private readonly IFavoriteJobPostingRepository _favoriteJobPostingRepository;

    public JobPostingsController(
        IJobPostingRepository jobPostingRepository,
        IUserRepository userRepository,
        ISportRepository sportRepository,
        IFavoriteJobPostingRepository favoriteJobPostingRepository)
    {
        _jobPostingRepository = jobPostingRepository;
        _userRepository = userRepository;
        _sportRepository = sportRepository;
        _favoriteJobPostingRepository = favoriteJobPostingRepository;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult> GetAll()
    {
        var jobPostings = await _jobPostingRepository.GetAllAsync();

        var response = jobPostings
            .Select(x => new
            {
                x.Id,
                x.InstitutionUserId,
                x.Title,
                x.Description,
                x.RequirementsText,
                x.BenefitsText,
                x.SportId,
                SportName = x.Sport != null ? x.Sport.Name : null,
                x.WorkMode,
                x.ContractType,
                x.Availability,
                x.Country,
                x.Province,
                x.City,
                x.Address,
                x.SalaryText,
                x.Status,
                x.PublishedAt,
                x.CreatedAt,
                x.UpdatedAt,
                Institution = x.InstitutionUser == null
                    ? null
                    : new
                    {
                        x.InstitutionUser.Id,
                        x.InstitutionUser.TradeName,
                        x.InstitutionUser.LegalName,
                        x.InstitutionUser.City,
                        x.InstitutionUser.Province,
                        x.InstitutionUser.Country,
                        x.InstitutionUser.ProfileImageUrl,
                        x.InstitutionUser.ShortDescription
                    }
            })
            .ToList();

        return Ok(response);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult> GetById(int id)
    {
        try
        {
            var jobPosting = await _jobPostingRepository.GetByIdAsync(id);
            if (jobPosting is null)
                return NotFound();

            return Ok(new
            {
                jobPosting.Id,
                jobPosting.InstitutionUserId,
                jobPosting.Title,
                jobPosting.Description,
                jobPosting.RequirementsText,
                jobPosting.BenefitsText,
                jobPosting.SportId,
                SportName = jobPosting.Sport?.Name,
                jobPosting.WorkMode,
                jobPosting.ContractType,
                jobPosting.Availability,
                jobPosting.Country,
                jobPosting.Province,
                jobPosting.City,
                jobPosting.Address,
                jobPosting.SalaryText,
                jobPosting.Status,
                jobPosting.PublishedAt,
                jobPosting.CreatedAt,
                jobPosting.UpdatedAt,

                Institution = jobPosting.InstitutionUser is null
                    ? null
                    : new
                    {
                        jobPosting.InstitutionUser.Id,
                        jobPosting.InstitutionUser.TradeName,
                        jobPosting.InstitutionUser.LegalName,
                        jobPosting.InstitutionUser.City,
                        jobPosting.InstitutionUser.Province,
                        jobPosting.InstitutionUser.Country,
                        jobPosting.InstitutionUser.ProfileImageUrl,
                        jobPosting.InstitutionUser.CoverImageUrl,
                        jobPosting.InstitutionUser.ShortDescription,
                        jobPosting.InstitutionUser.Description,
                        jobPosting.InstitutionUser.Website,
                        jobPosting.InstitutionUser.InstagramUrl,
                        jobPosting.InstitutionUser.FacebookUrl,
                        jobPosting.InstitutionUser.LinkedInUrl
                    },

                ApplicationsCount = jobPosting.Applications?.Count ?? 0
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ocurrió un error inesperado al obtener el detalle de la vacante: {ex.Message}");
        }
    }

    [HttpGet("institution/{institutionUserId}")]
    [Authorize]
    public async Task<ActionResult<List<JobPosting>>> GetByInstitutionUserId(int institutionUserId)
    {
        var loggedUserId = ClaimsHelper.GetUserId(User);
        var loggedRole = ClaimsHelper.GetRole(User);

        if (loggedUserId is null)
            return Unauthorized();

        if (loggedRole != nameof(UserRole.Admin) && loggedUserId.Value != institutionUserId)
            return Forbid();

        var jobPostings = await _jobPostingRepository.GetByInstitutionUserIdAsync(institutionUserId);
        return Ok(jobPostings);
    }

    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Institution) + "," + nameof(UserRole.Admin))]
    public async Task<ActionResult> Create(CreateJobPostingDto dto)
    {
        try
        {
            var loggedUserId = ClaimsHelper.GetUserId(User);
            var loggedRole = ClaimsHelper.GetRole(User);

            if (loggedUserId is null)
                return Unauthorized();

            if (loggedRole != nameof(UserRole.Admin) && loggedUserId.Value != dto.InstitutionUserId)
                return Forbid();

            if (dto.InstitutionUserId <= 0)
                return BadRequest("El InstitutionUserId es obligatorio.");

            if (string.IsNullOrWhiteSpace(dto.Title))
                return BadRequest("El título es obligatorio.");

            if (string.IsNullOrWhiteSpace(dto.Description))
                return BadRequest("La descripción es obligatoria.");

            if (!Enum.IsDefined(typeof(WorkMode), dto.WorkMode))
                return BadRequest("La modalidad seleccionada no es válida.");

            if (!Enum.IsDefined(typeof(ContractType), dto.ContractType))
                return BadRequest("El tipo de contrato seleccionado no es válido.");

            if (!Enum.IsDefined(typeof(Availability), dto.Availability))
                return BadRequest("La disponibilidad seleccionada no es válida.");

            var institution = await _userRepository.GetByIdAsync(dto.InstitutionUserId);
            if (institution is null)
                return NotFound("Institución no encontrada.");

            if (institution.Role != UserRole.Institution)
                return BadRequest("El usuario no es una institución.");

            if (!institution.IsActive || institution.ValidationStatus != ValidationStatus.Aprobado)
                return BadRequest("La institución no está aprobada o activa.");

            if (dto.SportId.HasValue)
            {
                var sport = await _sportRepository.GetByIdAsync(dto.SportId.Value);
                if (sport is null)
                    return BadRequest("El deporte seleccionado no existe.");
            }

            var jobPosting = new JobPosting
            {
                InstitutionUserId = dto.InstitutionUserId,
                Title = dto.Title.Trim(),
                Description = dto.Description.Trim(),
                RequirementsText = string.IsNullOrWhiteSpace(dto.RequirementsText) ? null : dto.RequirementsText.Trim(),
                BenefitsText = string.IsNullOrWhiteSpace(dto.BenefitsText) ? null : dto.BenefitsText.Trim(),
                SportId = dto.SportId,
                WorkMode = dto.WorkMode,
                ContractType = dto.ContractType,
                Availability = dto.Availability,
                Country = string.IsNullOrWhiteSpace(dto.Country) ? null : dto.Country.Trim(),
                Province = string.IsNullOrWhiteSpace(dto.Province) ? null : dto.Province.Trim(),
                City = string.IsNullOrWhiteSpace(dto.City) ? null : dto.City.Trim(),
                Address = string.IsNullOrWhiteSpace(dto.Address) ? null : dto.Address.Trim(),
                SalaryText = string.IsNullOrWhiteSpace(dto.SalaryText) ? null : dto.SalaryText.Trim(),
                Status = JobPostingStatus.Activo,
                PublishedAt = DateTime.UtcNow
            };

            await _jobPostingRepository.AddAsync(jobPosting);

            return CreatedAtAction(nameof(GetById), new { id = jobPosting.Id }, new
            {
                jobPosting.Id,
                jobPosting.InstitutionUserId,
                jobPosting.Title,
                jobPosting.Description,
                jobPosting.RequirementsText,
                jobPosting.BenefitsText,
                jobPosting.SportId,
                jobPosting.WorkMode,
                jobPosting.ContractType,
                jobPosting.Availability,
                jobPosting.Country,
                jobPosting.Province,
                jobPosting.City,
                jobPosting.Address,
                jobPosting.SalaryText,
                jobPosting.Status,
                jobPosting.PublishedAt,
                jobPosting.CreatedAt,
                jobPosting.UpdatedAt
            });
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
        {
            return BadRequest($"No se pudo guardar la vacante por un error de base de datos: {ex.InnerException?.Message ?? ex.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ocurrió un error inesperado al crear la vacante: {ex.Message}");
        }
    }

    [HttpPut("{id}/activate")]
    [Authorize(Roles = nameof(UserRole.Institution) + "," + nameof(UserRole.Admin))]
    public async Task<ActionResult> Activate(int id)
    {
        var entity = await _jobPostingRepository.GetByIdAsync(id);
        if (entity is null) return NotFound();

        var loggedUserId = ClaimsHelper.GetUserId(User);
        var loggedRole = ClaimsHelper.GetRole(User);

        if (loggedUserId is null)
            return Unauthorized();

        if (loggedRole != nameof(UserRole.Admin) && entity.InstitutionUserId != loggedUserId.Value)
            return Forbid();

        entity.Status = JobPostingStatus.Activo;
        entity.UpdatedAt = DateTime.UtcNow;

        await _jobPostingRepository.UpdateAsync(entity);
        return NoContent();
    }

    [HttpPut("{id}/inactivate")]
    [Authorize(Roles = nameof(UserRole.Institution) + "," + nameof(UserRole.Admin))]
    public async Task<ActionResult> Inactivate(int id)
    {
        var entity = await _jobPostingRepository.GetByIdAsync(id);
        if (entity is null) return NotFound();

        var loggedUserId = ClaimsHelper.GetUserId(User);
        var loggedRole = ClaimsHelper.GetRole(User);

        if (loggedUserId is null)
            return Unauthorized();

        if (loggedRole != nameof(UserRole.Admin) && entity.InstitutionUserId != loggedUserId.Value)
            return Forbid();

        entity.Status = JobPostingStatus.Inactivo;
        entity.UpdatedAt = DateTime.UtcNow;

        await _jobPostingRepository.UpdateAsync(entity);
        return NoContent();
    }

    [HttpPut("{id}/close")]
    [Authorize(Roles = nameof(UserRole.Institution) + "," + nameof(UserRole.Admin))]
    public async Task<ActionResult> Close(int id)
    {
        var entity = await _jobPostingRepository.GetByIdAsync(id);
        if (entity is null) return NotFound();

        var loggedUserId = ClaimsHelper.GetUserId(User);
        var loggedRole = ClaimsHelper.GetRole(User);

        if (loggedUserId is null)
            return Unauthorized();

        if (loggedRole != nameof(UserRole.Admin) && entity.InstitutionUserId != loggedUserId.Value)
            return Forbid();

        entity.Status = JobPostingStatus.Cerrado;
        entity.UpdatedAt = DateTime.UtcNow;

        await _jobPostingRepository.UpdateAsync(entity);
        return NoContent();
    }

    [HttpPut("{id}/delete")]
    [Authorize(Roles = nameof(UserRole.Institution) + "," + nameof(UserRole.Admin))]
    public async Task<ActionResult> DeleteLogical(int id)
    {
        var entity = await _jobPostingRepository.GetByIdAsync(id);
        if (entity is null) return NotFound();

        var loggedUserId = ClaimsHelper.GetUserId(User);
        var loggedRole = ClaimsHelper.GetRole(User);

        if (loggedUserId is null)
            return Unauthorized();

        if (loggedRole != nameof(UserRole.Admin) && entity.InstitutionUserId != loggedUserId.Value)
            return Forbid();

        await _favoriteJobPostingRepository.DeleteByJobPostingIdAsync(entity.Id);

        entity.Status = JobPostingStatus.Eliminado;
        entity.UpdatedAt = DateTime.UtcNow;

        await _jobPostingRepository.UpdateAsync(entity);
        return NoContent();
    }
}