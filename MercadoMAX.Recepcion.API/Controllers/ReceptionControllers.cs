using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MercadoMAX.Recepcion.API.DTOs;
using MercadoMAX.Recepcion.API.Services;

namespace MercadoMAX.Recepcion.API.Controllers;

// ── Reception ───────────────────────────────────────────
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReceptionController : ControllerBase
{
    private readonly IReceptionService _svc;
    public ReceptionController(IReceptionService svc) => _svc = svc;

    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] string? status, [FromQuery] int? stallId,
        [FromQuery] DateTime? dateFrom, [FromQuery] DateTime? dateTo,
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        => Ok(await _svc.ListAsync(status, stallId, dateFrom, dateTo, pageNumber, pageSize));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
        => Ok(await _svc.GetByIdAsync(id));

    [HttpPost]
    [Authorize(Policy = "Reception:Create")]
    public async Task<IActionResult> Create([FromBody] CreateReceptionRequest r)
        => Ok(await _svc.CreateAsync(r));

    [HttpPut("status")]
    [Authorize(Policy = "Reception:Update")]
    public async Task<IActionResult> UpdateStatus([FromBody] UpdateReceptionStatusRequest r)
        => Ok(await _svc.UpdateStatusAsync(r));
}

// ── ReceptionDetail ─────────────────────────────────────
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReceptionDetailController : ControllerBase
{
    private readonly IReceptionDetailService _svc;
    public ReceptionDetailController(IReceptionDetailService svc) => _svc = svc;

    [HttpGet("by-reception/{receptionId}")]
    public async Task<IActionResult> ListByReception(int receptionId)
        => Ok(await _svc.ListByReceptionAsync(receptionId));

    [HttpPost]
    [Authorize(Policy = "Reception:Create")]
    public async Task<IActionResult> Create([FromBody] CreateReceptionDetailRequest r)
        => Ok(await _svc.CreateAsync(r));

    [HttpPut]
    [Authorize(Policy = "Reception:Update")]
    public async Task<IActionResult> Update([FromBody] UpdateReceptionDetailRequest r)
        => Ok(await _svc.UpdateAsync(r));
}

// ── Shortage ────────────────────────────────────────────
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ShortageController : ControllerBase
{
    private readonly IShortageService _svc;
    public ShortageController(IShortageService svc) => _svc = svc;

    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] string? claimStatus, [FromQuery] int? receptionId,
        [FromQuery] DateTime? dateFrom, [FromQuery] DateTime? dateTo,
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        => Ok(await _svc.ListAsync(claimStatus, receptionId, dateFrom, dateTo, pageNumber, pageSize));

    [HttpGet("by-reception/{receptionId}")]
    public async Task<IActionResult> ListByReception(int receptionId)
        => Ok(await _svc.ListByReceptionAsync(receptionId));

    [HttpPost]
    [Authorize(Policy = "Reception:Create")]
    public async Task<IActionResult> Create([FromBody] CreateShortageRequest r)
        => Ok(await _svc.CreateAsync(r));

    [HttpPut("status")]
    [Authorize(Policy = "Reception:Update")]
    public async Task<IActionResult> UpdateStatus([FromBody] UpdateShortageStatusRequest r)
        => Ok(await _svc.UpdateStatusAsync(r));
}
