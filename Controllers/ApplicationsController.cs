using BuscoProfe.Api.DTOs.Applications;
using BuscoProfe.Api.Entities;
using BuscoProfe.Api.Enums;
using BuscoProfe.Api.Helpers;
using BuscoProfe.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuscoProfe.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApplicationsController : ControllerBase
{
    private readonly IApplicationRepository _applicationRepository;
    private readonly IJobPostingRepository _jobPostingRepository;
    private readonly IUserRepository _userRepository;
    private readonly INotificationRepository _notificationRepository;

    public ApplicationsController(
        IApplicationRepository applicationRepository,
        IJobPostingRepository jobPostingRepository,
        IUserRepository userRepository,
        INotificationRepository notificationRepository)
    {
        _applicationRepository = applicationRepository;
        _jobPostingRepository = jobPostingRepository;
        _userRepository = userRepository;
        _notificationRepository = notificationRepository;
    }

    [HttpGet("{id}")]
    [Authorize(Roles = nameof(UserRole.Professor) + "," + nameof(UserRole.Institution) + "," + nameof(UserRole.Admin))]
    public async Task<ActionResult<Application>> GetById(int id)
    {
        var application = await _applicationRepository.GetByIdAsync2(id);

        if (application is null)
            return NotFound("Postulación no encontrada.");

        var loggedUserId = ClaimsHelper.GetUserId(User);
        var loggedRole = ClaimsHelper.GetRole(User);

        if (loggedUserId is null)
            return Unauthorized();

        var jobPosting = await _jobPostingRepository.GetByIdAsync(application.JobPostingId);

        if (jobPosting is null)
            return NotFound("Vacante no encontrada.");

        var isProfessorOwner = application.ProfessorUserId == loggedUserId.Value;
        var isInstitutionOwner = jobPosting.InstitutionUserId == loggedUserId.Value;
        var isAdmin = loggedRole == nameof(UserRole.Admin);

        if (!isAdmin && !isProfessorOwner && !isInstitutionOwner)
            return Forbid();

        return Ok(new
        {
            application.Id,
            application.JobPostingId,
            application.ProfessorUserId,
            application.Message,
            application.CvUrl,
            application.Status,
            application.AppliedAt,
            application.UpdatedAt,

            JobTitle = jobPosting.Title,
            JobPostingStatus = jobPosting.Status,
            InstitutionUserId = jobPosting.InstitutionUserId,

            ProfessorFirstName = application.ProfessorUser != null
                ? application.ProfessorUser.FirstName
                : null,

            ProfessorLastName = application.ProfessorUser != null
                ? application.ProfessorUser.LastName
                : null,

            ProfessorEmail = application.ProfessorUser != null
                ? application.ProfessorUser.Email
                : null
        });
    }

    [HttpGet]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult> GetAll()
    {
        var applications = await _applicationRepository.GetAllAsync();

        return Ok(applications.Select(application => new
        {
            application.Id,
            application.JobPostingId,
            application.ProfessorUserId,
            application.Message,
            application.CvUrl,
            application.Status,
            application.AppliedAt,
            application.UpdatedAt,
            JobTitle = application.JobPosting != null ? application.JobPosting.Title : null
        }));
    }

    [HttpGet("professor/{professorUserId}")]
    [Authorize(Roles = nameof(UserRole.Professor) + "," + nameof(UserRole.Admin))]
    public async Task<ActionResult> GetByProfessorUserId(int professorUserId)
    {
        var loggedUserId = ClaimsHelper.GetUserId(User);
        var loggedRole = ClaimsHelper.GetRole(User);

        if (loggedUserId is null)
            return Unauthorized();

        if (loggedRole != nameof(UserRole.Admin) && loggedUserId.Value != professorUserId)
            return Forbid();

        var applications = await _applicationRepository.GetByProfessorUserIdAsync(professorUserId);

        return Ok(applications.Select(application => new
        {
            application.Id,
            application.JobPostingId,
            application.ProfessorUserId,
            application.Message,
            application.CvUrl,
            application.Status,
            application.AppliedAt,
            application.UpdatedAt,
            JobTitle = application.JobPosting != null ? application.JobPosting.Title : null,
            InstitutionUserId = application.JobPosting != null ? application.JobPosting.InstitutionUserId : (int?)null
        }));
    }

    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Professor) + "," + nameof(UserRole.Admin))]
    public async Task<ActionResult> Create(CreateApplicationDto dto)
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

        if (jobPosting.Status != JobPostingStatus.Activo)
            return BadRequest("La vacante no está activa.");

        var existing = await _applicationRepository.GetByJobPostingAndProfessorAsync(dto.JobPostingId, dto.ProfessorUserId);
        if (existing is not null)
            return BadRequest("Ya existe una postulación para esta vacante.");

        var application = new Application
        {
            JobPostingId = dto.JobPostingId,
            ProfessorUserId = dto.ProfessorUserId,
            Message = dto.Message,
            CvUrl = dto.CvUrl,
            Status = ApplicationStatus.Aplicado
        };

        await _applicationRepository.AddAsync(application);

        await _notificationRepository.AddAsync(new Notification
        {
            UserId = jobPosting.InstitutionUserId,
            Title = "Nueva postulación",
            Message = $"Recibiste una nueva postulación para la vacante '{jobPosting.Title}'.",
            Type = NotificationType.Verde
        });

        return Ok(new
        {
            application.Id,
            application.JobPostingId,
            application.ProfessorUserId,
            application.Message,
            application.CvUrl,
            application.Status,
            application.AppliedAt,
            application.UpdatedAt
        });
    }

    [HttpGet("jobposting/{jobPostingId}")]
    [Authorize(Roles = nameof(UserRole.Institution) + "," + nameof(UserRole.Admin))]
    public async Task<ActionResult> GetByJobPostingId(int jobPostingId)
    {
        var loggedUserId = ClaimsHelper.GetUserId(User);
        var loggedRole = ClaimsHelper.GetRole(User);

        if (loggedUserId is null)
            return Unauthorized();

        var jobPosting = await _jobPostingRepository.GetByIdAsync(jobPostingId);
        if (jobPosting is null)
            return NotFound("Vacante no encontrada.");

        if (loggedRole != nameof(UserRole.Admin) && jobPosting.InstitutionUserId != loggedUserId.Value)
            return Forbid();

        var applications = await _applicationRepository.GetAllAsync();

        var filtered = applications
            .Where(x => x.JobPostingId == jobPostingId)
            .Select(application => new
            {
                application.Id,
                application.JobPostingId,
                application.ProfessorUserId,
                application.Message,
                application.CvUrl,
                application.Status,
                application.AppliedAt,
                application.UpdatedAt,
                ProfessorFirstName = application.ProfessorUser != null ? application.ProfessorUser.FirstName : null,
                ProfessorLastName = application.ProfessorUser != null ? application.ProfessorUser.LastName : null,
                ProfessorTitle = application.ProfessorUser != null ? application.ProfessorUser.Title : null,
                ProfessorCity = application.ProfessorUser != null ? application.ProfessorUser.City : null,
                ProfessorProvince = application.ProfessorUser != null ? application.ProfessorUser.Province : null,
                ProfessorCountry = application.ProfessorUser != null ? application.ProfessorUser.Country : null
            })
            .OrderByDescending(x => x.AppliedAt)
            .ToList();

        return Ok(filtered);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = nameof(UserRole.Professor) + "," + nameof(UserRole.Admin))]
    public async Task<ActionResult> Delete(int id)
    {
        var application = await _applicationRepository.GetByIdAsync(id);
        if (application is null)
            return NotFound("Postulación no encontrada.");

        var loggedUserId = ClaimsHelper.GetUserId(User);
        var loggedRole = ClaimsHelper.GetRole(User);

        if (loggedUserId is null)
            return Unauthorized();

        if (loggedRole != nameof(UserRole.Admin) && application.ProfessorUserId != loggedUserId.Value)
            return Forbid();

        await _applicationRepository.DeleteAsync(application);

        return NoContent();
    }

    [HttpPut("{id}/withdraw")]
    [Authorize(Roles = nameof(UserRole.Professor) + "," + nameof(UserRole.Admin))]
    public async Task<ActionResult> Withdraw(int id)
    {
        var application = await _applicationRepository.GetByIdAsync(id);
        if (application is null)
            return NotFound("Postulación no encontrada.");

        var loggedUserId = ClaimsHelper.GetUserId(User);
        var loggedRole = ClaimsHelper.GetRole(User);

        if (loggedUserId is null)
            return Unauthorized();

        if (loggedRole != nameof(UserRole.Admin) && application.ProfessorUserId != loggedUserId.Value)
            return Forbid();

        await _applicationRepository.DeleteAsync(application);

        return NoContent();
    }

    [HttpPut("{id}/accept")]
    [Authorize(Roles = nameof(UserRole.Institution) + "," + nameof(UserRole.Admin))]
    public async Task<ActionResult> Accept(int id)
    {
        var application = await _applicationRepository.GetByIdAsync(id);
        if (application is null)
            return NotFound("Postulación no encontrada.");

        var loggedUserId = ClaimsHelper.GetUserId(User);
        var loggedRole = ClaimsHelper.GetRole(User);

        if (loggedUserId is null)
            return Unauthorized();

        var jobPosting = await _jobPostingRepository.GetByIdAsync(application.JobPostingId);
        if (jobPosting is null)
            return NotFound("Vacante no encontrada.");

        if (loggedRole != nameof(UserRole.Admin) && jobPosting.InstitutionUserId != loggedUserId.Value)
            return Forbid();

        if (application.Status == ApplicationStatus.Aceptado)
            return BadRequest("La postulación ya fue aceptada.");

        if (application.Status == ApplicationStatus.Rechazado)
            return BadRequest("No se puede aceptar una postulación rechazada.");

        await _applicationRepository.AcceptAsync(application);

        await _notificationRepository.AddAsync(new Notification
        {
            UserId = application.ProfessorUserId,
            Title = "Postulación aceptada",
            Message = $"Tu postulación a la vacante '{jobPosting.Title}' fue aceptada.",
            Type = NotificationType.Verde
        });

        return NoContent();
    }

    [HttpPut("{id}/reject")]
    [Authorize(Roles = nameof(UserRole.Institution) + "," + nameof(UserRole.Admin))]
    public async Task<ActionResult> Reject(int id)
    {
        var application = await _applicationRepository.GetByIdAsync(id);
        if (application is null)
            return NotFound("Postulación no encontrada.");

        var loggedUserId = ClaimsHelper.GetUserId(User);
        var loggedRole = ClaimsHelper.GetRole(User);

        if (loggedUserId is null)
            return Unauthorized();

        var jobPosting = await _jobPostingRepository.GetByIdAsync(application.JobPostingId);
        if (jobPosting is null)
            return NotFound("Vacante no encontrada.");

        if (loggedRole != nameof(UserRole.Admin) && jobPosting.InstitutionUserId != loggedUserId.Value)
            return Forbid();

        if (application.Status == ApplicationStatus.Rechazado)
            return BadRequest("La postulación ya fue rechazada.");

        if (application.Status == ApplicationStatus.Aceptado)
            return BadRequest("No se puede rechazar una postulación aceptada.");

        await _applicationRepository.RejectAsync(application);

        await _notificationRepository.AddAsync(new Notification
        {
            UserId = application.ProfessorUserId,
            Title = "Postulación rechazada",
            Message = $"Tu postulación a la vacante '{jobPosting.Title}' fue rechazada.",
            Type = NotificationType.Rojo
        });

        return NoContent();
    }
}