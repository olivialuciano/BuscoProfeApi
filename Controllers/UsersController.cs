using BuscoProfe.Api.DTOs.Users;
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
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IProfessorExperienceRepository _experienceRepository;
    private readonly IProfessorEducationRepository _educationRepository;
    private readonly IProfessorCertificationRepository _certificationRepository;
    private readonly IProfessorSkillRepository _skillRepository;
    private readonly IJobPostingRepository _jobPostingRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IEmailService _emailService;

    public UsersController(
        IUserRepository userRepository,
        IProfessorExperienceRepository experienceRepository,
        IProfessorEducationRepository educationRepository,
        IProfessorCertificationRepository certificationRepository,
        IProfessorSkillRepository skillRepository,
        IJobPostingRepository jobPostingRepository,
        INotificationRepository notificationRepository,
        IEmailService emailService)
    {
        _userRepository = userRepository;
        _experienceRepository = experienceRepository;
        _educationRepository = educationRepository;
        _certificationRepository = certificationRepository;
        _skillRepository = skillRepository;
        _jobPostingRepository = jobPostingRepository;
        _notificationRepository = notificationRepository;
        _emailService = emailService;
    }

    [HttpGet]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<List<UserResponseDto>>> GetAll()
    {
        var users = await _userRepository.GetAllAsync();

        var response = users.Select(u => new UserResponseDto
        {
            Id = u.Id,
            Email = u.Email,
            Role = u.Role,
            IsActive = u.IsActive,
            FirstName = u.FirstName,
            LastName = u.LastName,
            LegalName = u.LegalName,
            TradeName = u.TradeName,
            ValidationStatus = u.ValidationStatus,
            Title = u.Title,
            AboutMe = u.AboutMe
        }).ToList();

        return Ok(response);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<User>> GetById(int id)
    {
        var loggedUserId = ClaimsHelper.GetUserId(User);
        var loggedRole = ClaimsHelper.GetRole(User);

        if (loggedUserId is null)
            return Unauthorized();

        if (loggedRole != nameof(UserRole.Admin) && loggedUserId.Value != id)
            return Forbid();

        var user = await _userRepository.GetByIdAsync(id);
        if (user is null) return NotFound();

        return Ok(user);
    }

    [HttpGet("professors/{id}")]
    [AllowAnonymous]
    public async Task<ActionResult> GetProfessorById(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user is null) return NotFound();

        if (user.Role != UserRole.Professor)
            return BadRequest("El usuario indicado no es profesor.");

        var experiences = await _experienceRepository.GetByUserIdAsync(id);
        var educations = await _educationRepository.GetByUserIdAsync(id);
        var certifications = await _certificationRepository.GetByUserIdAsync(id);
        var skills = await _skillRepository.GetByUserIdAsync(id);

        var response = new
        {
            user.Id,
            user.Email,
            user.Role,
            user.IsActive,
            user.FirstName,
            user.LastName,

            user.WhatsApp1,
            user.WhatsApp2,
            user.WhatsApp3,

            user.Title,
            user.AboutMe,
            user.Languages,
            user.PreferredZone,
            user.Availability,
            user.WorkModePreference,
            user.ContractPreference,
            user.Country,
            user.Province,
            user.City,
            user.Address,
            user.ProfileImageUrl,
            user.CoverImageUrl,
            user.IsPublic,

            Experiences = experiences.Select(x => new
            {
                x.Id,
                x.UserId,
                x.Position,
                x.Description,
                x.StartDate,
                x.EndDate,
            }),

            Educations = educations.Select(x => new
            {
                x.Id,
                x.UserId,
                x.InstitutionName,
                x.Title,
                x.Status,
                x.StartDate,
                x.EndDate,
                x.Description
            }),

            Certifications = certifications.Select(x => new
            {
                x.Id,
                x.UserId,
                x.Name,
                x.Issuer,
                x.IssueDate,
                x.CredentialUrl
            }),

            Skills = skills.Select(x => new
            {
                x.Id,
                x.UserId,
                x.Name
            })
        };

        return Ok(response);
    }

    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<UserResponseDto>> Create(CreateUserDto dto)
    {
        var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
        if (existingUser is not null)
            return BadRequest("Ya existe un usuario con ese email.");

        var verificationCode = Random.Shared.Next(100000, 999999).ToString();

        var user = new User
        {
            Email = dto.Email,
            PasswordHash = PasswordHelper.HashPassword(dto.Password),
            Role = dto.Role,

            EmailConfirmed = false,
            EmailVerificationCode = verificationCode,
            EmailVerificationCodeExpiresAt = DateTime.UtcNow.AddMinutes(10),
            EmailVerificationCodeLastSentAt = DateTime.UtcNow,

            FirstName = dto.FirstName,
            LastName = dto.LastName,
            WhatsApp1 = dto.WhatsApp1,
            WhatsApp2 = dto.WhatsApp2,
            WhatsApp3 = dto.WhatsApp3,
            Country = dto.Country,
            Province = dto.Province,
            City = dto.City,
            Address = dto.Address,
            LegalName = dto.LegalName,
            TradeName = dto.TradeName,
            InstitutionType = dto.InstitutionType,
            ShortDescription = dto.ShortDescription,
            Description = dto.Description,
            Website = dto.Website,
            InstagramUrl = dto.InstagramUrl,
            FacebookUrl = dto.FacebookUrl,
            LinkedInUrl = dto.LinkedInUrl,
            BenefitsText = dto.BenefitsText,
            ValuesText = dto.ValuesText,
            HiringInfoText = dto.HiringInfoText,
            Title = dto.Title,
            AboutMe = dto.AboutMe,
            Languages = dto.Languages,
            PreferredZone = dto.PreferredZone,
            Availability = dto.Availability,
            WorkModePreference = dto.WorkModePreference,
            ContractPreference = dto.ContractPreference,
            SalaryExpectationText = dto.SalaryExpectationText,
            CvUrl = dto.CvUrl,
            IsPublic = dto.IsPublic
        };

        if (dto.Role == UserRole.Institution)
        {
            user.IsActive = false;
            user.ValidationStatus = ValidationStatus.Pendiente;
        }
        else
        {
            user.IsActive = true;
        }

        await _userRepository.AddAsync(user);

        await _emailService.SendEmailVerificationCodeAsync(user.Email, verificationCode);

        var response = new UserResponseDto
        {
            Id = user.Id,
            Email = user.Email,
            Role = user.Role,
            IsActive = user.IsActive,
            FirstName = user.FirstName,
            LastName = user.LastName,
            LegalName = user.LegalName,
            TradeName = user.TradeName,
            ValidationStatus = user.ValidationStatus,
            Title = user.Title,
            AboutMe = user.AboutMe
        };

        return CreatedAtAction(nameof(GetById), new { id = user.Id }, response);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult> Update(int id, UpdateUserDto dto)
    {
        var loggedUserId = ClaimsHelper.GetUserId(User);
        var loggedRole = ClaimsHelper.GetRole(User);

        if (loggedUserId is null)
            return Unauthorized();

        if (loggedRole != nameof(UserRole.Admin) && loggedUserId.Value != id)
            return Forbid();

        var user = await _userRepository.GetByIdAsync(id);
        if (user is null) return NotFound();

        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.WhatsApp1 = dto.WhatsApp1;
        user.WhatsApp2 = dto.WhatsApp2;
        user.WhatsApp3 = dto.WhatsApp3;
        user.Country = dto.Country;
        user.Province = dto.Province;
        user.City = dto.City;
        user.Address = dto.Address;
        user.ProfileImageUrl = dto.ProfileImageUrl;
        user.CoverImageUrl = dto.CoverImageUrl;
        user.LegalName = dto.LegalName;
        user.TradeName = dto.TradeName;
        user.InstitutionType = dto.InstitutionType;
        user.ShortDescription = dto.ShortDescription;
        user.Description = dto.Description;
        user.Website = dto.Website;
        user.InstagramUrl = dto.InstagramUrl;
        user.FacebookUrl = dto.FacebookUrl;
        user.LinkedInUrl = dto.LinkedInUrl;
        user.BenefitsText = dto.BenefitsText;
        user.ValuesText = dto.ValuesText;
        user.HiringInfoText = dto.HiringInfoText;
        user.Title = dto.Title;
        user.AboutMe = dto.AboutMe;
        user.Languages = dto.Languages;
        user.PreferredZone = dto.PreferredZone;
        user.Availability = dto.Availability;
        user.WorkModePreference = dto.WorkModePreference;
        user.ContractPreference = dto.ContractPreference;
        user.SalaryExpectationText = dto.SalaryExpectationText;
        user.CvUrl = dto.CvUrl;
        user.IsPublic = dto.IsPublic;
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);
        return NoContent();
    }

    [HttpPut("{id}/activate")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult> Activate(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user is null) return NotFound();

        user.IsActive = true;
        user.UpdatedAt = DateTime.UtcNow;

        if (user.Role == UserRole.Institution)
            user.ValidationStatus = ValidationStatus.Aprobado;

        await _userRepository.UpdateAsync(user);
        return NoContent();
    }

    [HttpPut("{id}/inactivate")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult> Inactivate(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user is null) return NotFound();

        user.IsActive = false;
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult> Delete(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user is null) return NotFound();

        if (user.Role == UserRole.Institution)
        {
            var hasJobPostings = await _jobPostingRepository.InstitutionHasAnyAsync(id);
            if (hasJobPostings)
            {
                return BadRequest("No se puede eliminar la institución porque tiene vacantes asociadas. Podés inactivarla, pero no borrarla.");
            }
        }

        await _userRepository.DeleteAsync(user);
        return NoContent();
    }

    [HttpPut("{id}/activate-institution")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult> ActivateInstitution(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user is null) return NotFound();

        if (user.Role != UserRole.Institution)
            return BadRequest("El usuario no es una institución.");

        user.IsActive = true;
        user.ValidationStatus = ValidationStatus.Aprobado;
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);

        await _notificationRepository.AddAsync(new Notification
        {
            UserId = user.Id,
            Title = "Institución aprobada",
            Message = "Tu institución fue aprobada y ya puede publicar vacantes.",
            Type = NotificationType.Verde
        });

        return NoContent();
    }
    [HttpGet("institutions")]
    [AllowAnonymous]
    public async Task<ActionResult> GetAllInstitutions()
    {
        var institutions = await _userRepository.GetAllInstitutionsAsync();

        var response = institutions.Select(u => new
        {
            u.Id,
            u.Email,
            u.Role,
            u.IsActive,
            u.LegalName,
            u.TradeName,
            u.InstitutionType,
            u.ShortDescription,
            u.Description,
            u.Website,
            u.InstagramUrl,
            u.FacebookUrl,
            u.LinkedInUrl,
            u.Country,
            u.Province,
            u.City,
            u.Address,
            u.ProfileImageUrl,
            u.CoverImageUrl,
            u.ValidationStatus
        }).ToList();

        return Ok(response);
    }

    [HttpGet("professors")]
    [AllowAnonymous]
    public async Task<ActionResult> GetAllProfessors()
    {
        var professors = await _userRepository.GetAllProfessorsAsync();

        var response = professors.Select(u => new
        {
            u.Id,
            u.Email,
            u.Role,
            u.IsActive,
            u.FirstName,
            u.LastName,
            u.Title,
            u.AboutMe,
            u.Languages,
            u.PreferredZone,
            u.Availability,
            u.WorkModePreference,
            u.ContractPreference,
            u.SalaryExpectationText,
            u.Country,
            u.Province,
            u.City,
            u.Address,
            u.ProfileImageUrl,
            u.CoverImageUrl,
            u.CvUrl,
            u.IsPublic
        }).ToList();

        return Ok(response);
    }
}