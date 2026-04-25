using BuscoProfe.Api.DTOs.Sports;
using BuscoProfe.Api.Entities;
using BuscoProfe.Api.Enums;
using BuscoProfe.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuscoProfe.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SportsController : ControllerBase
{
    private readonly ISportRepository _sportRepository;

    public SportsController(ISportRepository sportRepository)
    {
        _sportRepository = sportRepository;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<List<Sport>>> GetAll()
    {
        var sports = await _sportRepository.GetAllAsync();
        return Ok(sports);
    }

    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<Sport>> Create(CreateSportDto dto)
    {
        var existing = await _sportRepository.GetByNameAsync(dto.Name.Trim());
        if (existing is not null)
            return BadRequest("Ya existe ese deporte.");

        var sport = new Sport
        {
            Name = dto.Name.Trim()
        };

        await _sportRepository.AddAsync(sport);
        return Ok(sport);
    }
}