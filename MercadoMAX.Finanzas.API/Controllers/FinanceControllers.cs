using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MercadoMAX.Finanzas.API.DTOs;
using MercadoMAX.Finanzas.API.Services;

namespace MercadoMAX.Finanzas.API.Controllers;

// ── Sale ────────────────────────────────────────────────
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SaleController : ControllerBase
{
    private readonly ISaleService _svc;
    public SaleController(ISaleService svc) => _svc = svc;

    [HttpGet]
    [Authorize(Policy = "Finance:List")]
    public async Task<IActionResult> List(
        [FromQuery] int? stallId,
        [FromQuery] string? paymentType,
        [FromQuery] string? saleStatus,
        [FromQuery] DateOnly? dateFrom,
        [FromQuery] DateOnly? dateTo,
        [FromQuery] string? search,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
        => Ok(await _svc.ListAsync(stallId, paymentType, saleStatus, dateFrom, dateTo, search, pageNumber, pageSize));

    [HttpGet("{id}")]
    [Authorize(Policy = "Finance:Read")]
    public async Task<IActionResult> GetById(int id)
        => Ok(await _svc.GetByIdAsync(id));

    [HttpPost]
    [Authorize(Policy = "Finance:Create")]
    public async Task<IActionResult> Create([FromBody] CreateSaleRequest r)
        => Ok(await _svc.CreateAsync(r));

    [HttpPost("detail")]
    [Authorize(Policy = "Finance:Create")]
    public async Task<IActionResult> CreateDetail([FromBody] CreateSaleDetailRequest r)
        => Ok(await _svc.CreateDetailAsync(r));

    [HttpPut("{id}/void")]
    [Authorize(Policy = "Finance:Update")]
    public async Task<IActionResult> Void(int id)
        => Ok(await _svc.VoidAsync(id));
}

// ── Account Payable ──────────────────────────────────────
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AccountPayableController : ControllerBase
{
    private readonly IAccountPayableService _svc;
    public AccountPayableController(IAccountPayableService svc) => _svc = svc;

    [HttpGet]
    [Authorize(Policy = "Finance:List")]
    public async Task<IActionResult> List(
        [FromQuery] int? stallId,
        [FromQuery] int? supplierId,
        [FromQuery] string? accountStatus,
        [FromQuery] DateOnly? dateFrom,
        [FromQuery] DateOnly? dateTo,
        [FromQuery] string? search,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
        => Ok(await _svc.ListAsync(stallId, supplierId, accountStatus, dateFrom, dateTo, search, pageNumber, pageSize));

    [HttpGet("{id}")]
    [Authorize(Policy = "Finance:Read")]
    public async Task<IActionResult> GetById(int id)
        => Ok(await _svc.GetByIdAsync(id));

    [HttpPost]
    [Authorize(Policy = "Finance:Create")]
    public async Task<IActionResult> Create([FromBody] CreateAccountPayableRequest r)
        => Ok(await _svc.CreateAsync(r));

    /// <summary>
    /// Lineas que la guia trae a ese puesto: cantidad, producto, marca y calibre
    /// ya resueltos. Es lo que el formulario pinta para poner solo los precios.
    /// </summary>
    [HttpGet("guide-lines")]
    [Authorize(Policy = "Finance:List")]
    public async Task<IActionResult> GuideLines([FromQuery] int guideId, [FromQuery] int stallId)
        => Ok(await _svc.ListGuideLinesAsync(guideId, stallId));

    /// <summary>
    /// Alta a partir de la guia. El cuerpo lleva los precios por linea, no el
    /// importe: el total lo calcula el servidor sobre las cantidades de la guia.
    /// </summary>
    [HttpPost("from-guide")]
    [Authorize(Policy = "Finance:Create")]
    public async Task<IActionResult> CreateFromGuide([FromBody] CreateAccountPayableFromGuideRequest r)
    {
        var result = await _svc.CreateFromGuideAsync(r);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("by-stall/{stallId}")]
    [Authorize(Policy = "Finance:List")]
    public async Task<IActionResult> ListByStall(int stallId, [FromQuery] string? accountStatus)
        => Ok(await _svc.ListByStallAsync(stallId, accountStatus));

    [HttpPost("payment")]
    [Authorize(Policy = "Finance:Create")]
    public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest r)
        => Ok(await _svc.CreatePaymentAsync(r));
}

// ── Report ────────────────────────────────────────────────
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportController(IReportService svc) : ControllerBase
{
    [HttpGet("account-statement/{stallId}")]
    [Authorize(Policy = "Finance:List")]
    public async Task<IActionResult> AccountStatement(
        int stallId,
        [FromQuery] DateOnly? dateFrom,
        [FromQuery] DateOnly? dateTo)
        => Ok(await svc.GetAccountStatementAsync(stallId, dateFrom, dateTo));
}
