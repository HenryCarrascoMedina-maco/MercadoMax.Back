using MercadoMAX.Guias.API.DTOs;
using MercadoMAX.Guias.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MercadoMAX.Guias.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GuideController : ControllerBase
{
    private readonly IGuideService _service;
    public GuideController(IGuideService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] string? guideStatus, [FromQuery] int? supplierId, [FromQuery] int? carrierId,
        [FromQuery] DateTime? dateFrom, [FromQuery] DateTime? dateTo, [FromQuery] string? search,
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _service.ListAsync(guideStatus, supplierId, carrierId, dateFrom, dateTo, search, pageNumber, pageSize);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpGet("by-supplier/{supplierId}")]
    public async Task<IActionResult> ListBySupplier(int supplierId, [FromQuery] string? guideStatus)
    {
        var result = await _service.ListBySupplierAsync(supplierId, guideStatus);
        return Ok(result);
    }

    [HttpGet("by-stall/{stallId}")]
    public async Task<IActionResult> ListByStall(int stallId, [FromQuery] string? guideStatus)
    {
        var result = await _service.ListByStallAsync(stallId, guideStatus);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = "Guides:Create")]
    public async Task<IActionResult> Create([FromBody] CreateGuideRequest request)
    {
        var result = await _service.CreateAsync(request);
        return result.Success ? CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result) : BadRequest(result);
    }

    [HttpPut("status")]
    [Authorize(Policy = "Guides:Update")]
    public async Task<IActionResult> UpdateStatus([FromBody] UpdateGuideStatusRequest request)
    {
        var result = await _service.UpdateStatusAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "Guides:Delete")]
    public async Task<IActionResult> Void(int id)
    {
        var result = await _service.VoidAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GuideDetailController : ControllerBase
{
    private readonly IGuideDetailService _service;
    public GuideDetailController(IGuideDetailService service) => _service = service;

    [HttpGet("by-guide/{guideId}")]
    public async Task<IActionResult> ListByGuide(int guideId)
    {
        var result = await _service.ListByGuideAsync(guideId);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = "Guides:Create")]
    public async Task<IActionResult> Create([FromBody] CreateGuideDetailRequest request)
    {
        var result = await _service.CreateAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut]
    [Authorize(Policy = "Guides:Update")]
    public async Task<IActionResult> Update([FromBody] UpdateGuideDetailRequest request)
    {
        var result = await _service.UpdateAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "Guides:Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
