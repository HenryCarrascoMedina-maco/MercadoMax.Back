using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MercadoMAX.Comerciante.API.DTOs;
using MercadoMAX.Comerciante.API.Services;

namespace MercadoMAX.Comerciante.API.Controllers;

// ── ReceptionConfirmation ───────────────────────────────
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReceptionConfirmationController : ControllerBase
{
    private readonly IReceptionConfirmationService _svc;
    public ReceptionConfirmationController(IReceptionConfirmationService svc) => _svc = svc;

    [HttpGet("by-stall/{stallId}")]
    public async Task<IActionResult> ListByStall(int stallId, [FromQuery] DateTime? dateFrom, [FromQuery] DateTime? dateTo)
        => Ok(await _svc.ListByStallAsync(stallId, dateFrom, dateTo));

    [HttpPost]
    [Authorize(Policy = "Inventory:Update")]
    public async Task<IActionResult> Create([FromBody] CreateReceptionConfirmationRequest r)
        => Ok(await _svc.CreateAsync(r));
}

// ── Inventory ───────────────────────────────────────────
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _svc;
    public InventoryController(IInventoryService svc) => _svc = svc;

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int? stallId, [FromQuery] int? productId, [FromQuery] bool? lowStock)
        => Ok(await _svc.ListAsync(stallId, productId, lowStock));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
        => Ok(await _svc.GetByIdAsync(id));

    [HttpGet("by-stall/{stallId}")]
    public async Task<IActionResult> ListByStall(int stallId)
        => Ok(await _svc.ListByStallAsync(stallId));

    [HttpPost]
    [Authorize(Policy = "Inventory:Update")]
    public async Task<IActionResult> Create([FromBody] CreateInventoryRequest r)
        => Ok(await _svc.CreateAsync(r));

    [HttpPut]
    [Authorize(Policy = "Inventory:Update")]
    public async Task<IActionResult> Update([FromBody] UpdateInventoryRequest r)
        => Ok(await _svc.UpdateAsync(r));
}

// ── InventoryMovement ───────────────────────────────────
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InventoryMovementController : ControllerBase
{
    private readonly IInventoryMovementService _svc;
    public InventoryMovementController(IInventoryMovementService svc) => _svc = svc;

    [HttpGet("by-inventory/{inventoryId}")]
    public async Task<IActionResult> ListByInventory(
        int inventoryId,
        [FromQuery] DateTime? dateFrom, [FromQuery] DateTime? dateTo,
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        => Ok(await _svc.ListByInventoryAsync(inventoryId, dateFrom, dateTo, pageNumber, pageSize));

    [HttpPost]
    [Authorize(Policy = "Inventory:Update")]
    public async Task<IActionResult> Create([FromBody] CreateInventoryMovementRequest r)
        => Ok(await _svc.CreateAsync(r));
}
