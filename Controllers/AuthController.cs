using BuscoProfe.Api.Data;
using BuscoProfe.Api.DTOs.Auth;
using BuscoProfe.Api.Entities;
using BuscoProfe.Api.Enums;
using BuscoProfe.Api.Helpers;
using BuscoProfe.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BuscoProfe.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IEmailService _emailService;
    private readonly AppDbContext _context;

    public AuthController(
        IUserRepository userRepository,
        IJwtService jwtService,
        IEmailService emailService,
        AppDbContext context)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _emailService = emailService;
        _context = context;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginDto dto)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email);
        if (user is null)
            return Unauthorized("Credenciales inválidas.");

        if (!PasswordHelper.VerifyPassword(dto.Password, user.PasswordHash))
            return Unauthorized("Credenciales inválidas.");

        if (!user.IsActive)
            return Unauthorized("El usuario está inactivo.");

        if (!user.EmailConfirmed)
            return Unauthorized("Tenés que verificar tu email antes de iniciar sesión.");

        var token = _jwtService.GenerateToken(user);

        return Ok(new LoginResponseDto
        {
            Token = token,
            ExpiresAt = _jwtService.GetExpirationUtc(),
            UserId = user.Id,
            Email = user.Email,
            Role = user.Role
        });
    }

    [HttpPost("start-registration")]
    [AllowAnonymous]
    public async Task<ActionResult> StartRegistration(StartRegistrationDto dto)
    {
        var email = dto.Email.Trim().ToLower();

        if (string.IsNullOrWhiteSpace(email))
            return BadRequest("El email es obligatorio.");

        if (string.IsNullOrWhiteSpace(dto.Password))
            return BadRequest("La contraseña es obligatoria.");

        var existingUser = await _userRepository.GetByEmailAsync(email);

        if (existingUser is not null)
            return BadRequest("Ya existe un usuario con ese email.");

        var existingPendingRegistration = await _context.PendingUserRegistrations
            .FirstOrDefaultAsync(x => x.Email == email);

        if (existingPendingRegistration is not null)
        {
            _context.PendingUserRegistrations.Remove(existingPendingRegistration);
            await _context.SaveChangesAsync();
        }

        var verificationCode = Random.Shared.Next(100000, 1000000).ToString();

        var pendingRegistration = new PendingUserRegistration
        {
            Email = email,
            PasswordHash = PasswordHelper.HashPassword(dto.Password),
            Role = dto.Role,

            FirstName = dto.FirstName,
            LastName = dto.LastName,
            LegalName = dto.LegalName,
            TradeName = dto.TradeName,
            InstitutionType = dto.InstitutionType,
            City = dto.City,
            Province = dto.Province,
            Country = dto.Country,

            EmailVerificationCode = verificationCode,
            EmailVerificationCodeExpiresAt = DateTime.UtcNow.AddMinutes(10),
            EmailVerificationCodeLastSentAt = DateTime.UtcNow
        };

        _context.PendingUserRegistrations.Add(pendingRegistration);
        await _context.SaveChangesAsync();

        await _emailService.SendEmailVerificationCodeAsync(email, verificationCode);

        return Ok("Te enviamos un código de verificación a tu email.");
    }

    [HttpPost("verify-email-code")]
    [AllowAnonymous]
    public async Task<ActionResult> VerifyEmailCode(VerifyEmailCodeDto dto)
    {
        var email = dto.Email.Trim().ToLower();
        var code = dto.Code.Trim();

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(code))
            return BadRequest("Email y código son obligatorios.");

        if (code.Length != 6 || !code.All(char.IsDigit))
            return BadRequest("El código debe tener 6 dígitos.");

        var existingUser = await _userRepository.GetByEmailAsync(email);

        if (existingUser is not null)
            return BadRequest("Ya existe un usuario registrado con ese email.");

        var pendingRegistration = await _context.PendingUserRegistrations
            .FirstOrDefaultAsync(x => x.Email == email);

        if (pendingRegistration is null)
            return BadRequest("No encontramos una registración pendiente para ese email.");

        if (pendingRegistration.EmailVerificationCode != code)
            return BadRequest("El código ingresado no es válido.");

        if (pendingRegistration.EmailVerificationCodeExpiresAt < DateTime.UtcNow)
            return BadRequest("El código expiró. Solicitá uno nuevo.");

        var user = new User
        {
            Email = pendingRegistration.Email,
            PasswordHash = pendingRegistration.PasswordHash,
            Role = pendingRegistration.Role,

            EmailConfirmed = true,
            EmailVerificationCode = null,
            EmailVerificationCodeExpiresAt = null,
            EmailVerificationCodeLastSentAt = null,

            FirstName = pendingRegistration.FirstName,
            LastName = pendingRegistration.LastName,
            LegalName = pendingRegistration.LegalName,
            TradeName = pendingRegistration.TradeName,
            InstitutionType = pendingRegistration.InstitutionType,
            City = pendingRegistration.City,
            Province = pendingRegistration.Province,
            Country = pendingRegistration.Country
        };

        if (pendingRegistration.Role == UserRole.Institution)
        {
            user.IsActive = false;
            user.ValidationStatus = ValidationStatus.Pendiente;
        }
        else
        {
            user.IsActive = true;
        }

        await _userRepository.AddAsync(user);

        _context.PendingUserRegistrations.Remove(pendingRegistration);
        await _context.SaveChangesAsync();

        return Ok("Email verificado correctamente. Tu cuenta fue creada.");
    }

    [HttpPost("resend-email-verification-code")]
    [AllowAnonymous]
    public async Task<ActionResult> ResendEmailVerificationCode(ResendEmailVerificationCodeDto dto)
    {
        var email = dto.Email.Trim().ToLower();

        if (string.IsNullOrWhiteSpace(email))
            return BadRequest("El email es obligatorio.");

        var existingUser = await _userRepository.GetByEmailAsync(email);

        if (existingUser is not null)
            return BadRequest("Ya existe un usuario registrado con ese email.");

        var pendingRegistration = await _context.PendingUserRegistrations
            .FirstOrDefaultAsync(x => x.Email == email);

        if (pendingRegistration is null)
            return BadRequest("No encontramos una registración pendiente para ese email.");

        if (pendingRegistration.EmailVerificationCodeLastSentAt.AddMinutes(1) > DateTime.UtcNow)
            return BadRequest("Tenés que esperar un minuto antes de solicitar otro código.");

        var verificationCode = Random.Shared.Next(100000, 1000000).ToString();

        pendingRegistration.EmailVerificationCode = verificationCode;
        pendingRegistration.EmailVerificationCodeExpiresAt = DateTime.UtcNow.AddMinutes(10);
        pendingRegistration.EmailVerificationCodeLastSentAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await _emailService.SendEmailVerificationCodeAsync(email, verificationCode);

        return Ok("Te enviamos un nuevo código de verificación.");
    }
}