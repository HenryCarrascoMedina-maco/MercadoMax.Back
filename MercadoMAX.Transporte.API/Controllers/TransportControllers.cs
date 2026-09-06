using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MercadoMAX.Transporte.API.DTOs;
using MercadoMAX.Transporte.API.Services;

namespace MercadoMAX.Transporte.API.Controllers;

// ── Carrier ─────────────────────────────────────────────
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CarrierController : ControllerBase
{
    private readonly ICarrierService _svc;
    public CarrierController(ICarrierService svc) => _svc = svc;

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] bool? status, [FromQuery] string? search)
        => Ok(await _svc.ListAsync(status, search));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
        => Ok(await _svc.GetByIdAsync(id));

    [HttpPost]
    [Authorize(Policy = "Transport:Create")]
    public async Task<IActionResult> Create([FromBody] CreateCarrierRequest r)
        => Ok(await _svc.CreateAsync(r));

    [HttpPut]
    [Authorize(Policy = "Transport:Update")]
    public async Task<IActionResult> Update([FromBody] UpdateCarrierRequest r)
        => Ok(await _svc.UpdateAsync(r));

    [HttpDelete("{id}")]
    [Authorize(Policy = "Transport:Delete")]
    public async Task<IActionResult> Delete(int id)
        => Ok(await _svc.DeleteAsync(id));

    [HttpPatch("{id}/toggle-status")]
    [Authorize(Policy = "Transport:Delete")]
    public async Task<IActionResult> ToggleStatus(int id)
        => Ok(await _svc.ToggleStatusAsync(id));
}

// ── Truck ───────────────────────────────────────────────
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TruckController : ControllerBase
{
    private readonly ITruckService _svc;
    public TruckController(ITruckService svc) => _svc = svc;

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] bool? status, [FromQuery] int? carrierId, [FromQuery] string? search)
        => Ok(await _svc.ListAsync(status, carrierId, search));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
        => Ok(await _svc.GetByIdAsync(id));

    [HttpGet("by-carrier/{carrierId}")]
    public async Task<IActionResult> ListByCarrier(int carrierId)
        => Ok(await _svc.ListByCarrierAsync(carrierId));

    [HttpPost]
    [Authorize(Policy = "Transport:Create")]
    public async Task<IActionResult> Create([FromBody] CreateTruckRequest r)
        => Ok(await _svc.CreateAsync(r));

    [HttpPut]
    [Authorize(Policy = "Transport:Update")]
    public async Task<IActionResult> Update([FromBody] UpdateTruckRequest r)
        => Ok(await _svc.UpdateAsync(r));

    [HttpDelete("{id}")]
    [Authorize(Policy = "Transport:Delete")]
    public async Task<IActionResult> Delete(int id)
        => Ok(await _svc.DeleteAsync(id));

    [HttpPatch("{id}/toggle-status")]
    [Authorize(Policy = "Transport:Delete")]
    public async Task<IActionResult> ToggleStatus(int id)
        => Ok(await _svc.ToggleStatusAsync(id));
}

// ── TransportRate ───────────────────────────────────────
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransportRateController : ControllerBase
{
    private readonly ITransportRateService _svc;
    public TransportRateController(ITransportRateService svc) => _svc = svc;

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] bool? status, [FromQuery] int? carrierId, [FromQuery] int? logisticUnitId)
        => Ok(await _svc.ListAsync(status, carrierId, logisticUnitId));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
        => Ok(await _svc.GetByIdAsync(id));

    [HttpGet("by-carrier/{carrierId}")]
    public async Task<IActionResult> ListByCarrier(int carrierId)
        => Ok(await _svc.ListByCarrierAsync(carrierId));

    [HttpPost]
    [Authorize(Policy = "Transport:Create")]
    public async Task<IActionResult> Create([FromBody] CreateTransportRateRequest r)
        => Ok(await _svc.CreateAsync(r));

    [HttpPut]
    [Authorize(Policy = "Transport:Update")]
    public async Task<IActionResult> Update([FromBody] UpdateTransportRateRequest r)
        => Ok(await _svc.UpdateAsync(r));

    [HttpDelete("{id}")]
    [Authorize(Policy = "Transport:Delete")]
    public async Task<IActionResult> Delete(int id)
        => Ok(await _svc.DeleteAsync(id));

    [HttpPatch("{id}/toggle-status")]
    [Authorize(Policy = "Transport:Delete")]
    public async Task<IActionResult> ToggleStatus(int id)
        => Ok(await _svc.ToggleStatusAsync(id));
}

// ── Settlement ──────────────────────────────────────────
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SettlementController : ControllerBase
{
    private readonly ISettlementService _svc;
    public SettlementController(ISettlementService svc) => _svc = svc;

    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] string? status, [FromQuery] int? carrierId,
        [FromQuery] DateTime? dateFrom, [FromQuery] DateTime? dateTo,
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        => Ok(await _svc.ListAsync(status, carrierId, dateFrom, dateTo, pageNumber, pageSize));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
        => Ok(await _svc.GetByIdAsync(id));

    [HttpPost]
    [Authorize(Policy = "Transport:Create")]
    public async Task<IActionResult> Create([FromBody] CreateSettlementRequest r)
        => Ok(await _svc.CreateAsync(r));

    [HttpPut("status")]
    [Authorize(Policy = "Transport:Update")]
    public async Task<IActionResult> UpdateStatus([FromBody] UpdateSettlementStatusRequest r)
        => Ok(await _svc.UpdateStatusAsync(r));

    /// <summary>
    /// Borra la liquidacion y sus lineas. Solo si sigue pendiente: una pagada
    /// es un registro contable y el SP la rechaza.
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Policy = "Transport:Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _svc.DeleteAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("detail")]
    [Authorize(Policy = "Transport:Create")]
    public async Task<IActionResult> CreateDetail([FromBody] CreateSettlementDetailRequest r)
        => Ok(await _svc.CreateDetailAsync(r));

    [HttpDelete("detail/{id}")]
    [Authorize(Policy = "Transport:Delete")]
    public async Task<IActionResult> DeleteDetail(int id)
        => Ok(await _svc.DeleteDetailAsync(id));
}
