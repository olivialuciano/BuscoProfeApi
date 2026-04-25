using BuscoProfe.Api.DTOs.Test;
using BuscoProfe.Api.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuscoProfe.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpPost("hash-password")]
    [AllowAnonymous]
    public ActionResult<HashPasswordResponseDto> HashPassword(HashPasswordDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Password))
            return BadRequest("La contraseña es obligatoria.");

        return Ok(new HashPasswordResponseDto
        {
            Password = dto.Password,
            Hash = PasswordHelper.HashPassword(dto.Password)
        });
    }
}