using MercadoMAX.Maestros.API.DTOs;
using MercadoMAX.Maestros.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MercadoMAX.Maestros.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductCategoryController : ControllerBase
{
    private readonly IProductCategoryService _service;
    public ProductCategoryController(IProductCategoryService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] bool? status, [FromQuery] string? search)
    {
        var result = await _service.ListAsync(status, search);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    [Authorize(Policy = "Masters:Create")]
    public async Task<IActionResult> Create([FromBody] CreateProductCategoryRequest request)
    {
        var result = await _service.CreateAsync(request);
        return result.Success ? CreatedAtAction(nameof(GetById), new { id = result.Data }, result) : BadRequest(result);
    }

    [HttpPut]
    [Authorize(Policy = "Masters:Update")]
    public async Task<IActionResult> Update([FromBody] UpdateProductCategoryRequest request)
    {
        var result = await _service.UpdateAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "Masters:Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPatch("{id}/toggle-status")]
    [Authorize(Policy = "Masters:Delete")]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var result = await _service.ToggleStatusAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductController : ControllerBase
{
    private readonly IProductService _service;
    public ProductController(IProductService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] bool? status, [FromQuery] int? categoryId, [FromQuery] string? search)
    {
        var result = await _service.ListAsync(status, categoryId, search);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    [Authorize(Policy = "Masters:Create")]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
    {
        var result = await _service.CreateAsync(request);
        return result.Success ? CreatedAtAction(nameof(GetById), new { id = result.Data }, result) : BadRequest(result);
    }

    [HttpPut]
    [Authorize(Policy = "Masters:Update")]
    public async Task<IActionResult> Update([FromBody] UpdateProductRequest request)
    {
        var result = await _service.UpdateAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "Masters:Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPatch("{id}/toggle-status")]
    [Authorize(Policy = "Masters:Delete")]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var result = await _service.ToggleStatusAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductSizeController : ControllerBase
{
    private readonly IProductSizeService _service;
    public ProductSizeController(IProductSizeService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] bool? status, [FromQuery] int? productId)
    {
        var result = await _service.ListAsync(status, productId);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    [Authorize(Policy = "Masters:Create")]
    public async Task<IActionResult> Create([FromBody] CreateProductSizeRequest request)
    {
        var result = await _service.CreateAsync(request);
        return result.Success ? CreatedAtAction(nameof(GetById), new { id = result.Data }, result) : BadRequest(result);
    }

    [HttpPut]
    [Authorize(Policy = "Masters:Update")]
    public async Task<IActionResult> Update([FromBody] UpdateProductSizeRequest request)
    {
        var result = await _service.UpdateAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "Masters:Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPatch("{id}/toggle-status")]
    [Authorize(Policy = "Masters:Delete")]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var result = await _service.ToggleStatusAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BrandController : ControllerBase
{
    private readonly IBrandService _service;
    public BrandController(IBrandService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] bool? status, [FromQuery] int? supplierId, [FromQuery] int? productId, [FromQuery] string? search)
    {
        var result = await _service.ListAsync(status, supplierId, productId, search);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpGet("by-supplier/{supplierId}")]
    public async Task<IActionResult> ListBySupplier(int supplierId)
    {
        var result = await _service.ListBySupplierAsync(supplierId);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = "Masters:Create")]
    public async Task<IActionResult> Create([FromBody] CreateBrandRequest request)
    {
        var result = await _service.CreateAsync(request);
        return result.Success ? CreatedAtAction(nameof(GetById), new { id = result.Data }, result) : BadRequest(result);
    }

    [HttpPut]
    [Authorize(Policy = "Masters:Update")]
    public async Task<IActionResult> Update([FromBody] UpdateBrandRequest request)
    {
        var result = await _service.UpdateAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "Masters:Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPatch("{id}/toggle-status")]
    [Authorize(Policy = "Masters:Delete")]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var result = await _service.ToggleStatusAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SupplierController : ControllerBase
{
    private readonly ISupplierService _service;
    public SupplierController(ISupplierService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] bool? status, [FromQuery] string? search)
    {
        var result = await _service.ListAsync(status, search);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    [Authorize(Policy = "Masters:Create")]
    public async Task<IActionResult> Create([FromBody] CreateSupplierRequest request)
    {
        var result = await _service.CreateAsync(request);
        return result.Success ? CreatedAtAction(nameof(GetById), new { id = result.Data }, result) : BadRequest(result);
    }

    [HttpPut]
    [Authorize(Policy = "Masters:Update")]
    public async Task<IActionResult> Update([FromBody] UpdateSupplierRequest request)
    {
        var result = await _service.UpdateAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "Masters:Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPatch("{id}/toggle-status")]
    [Authorize(Policy = "Masters:Delete")]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var result = await _service.ToggleStatusAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LogisticUnitController : ControllerBase
{
    private readonly ILogisticUnitService _service;
    public LogisticUnitController(ILogisticUnitService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] bool? status)
    {
        var result = await _service.ListAsync(status);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    [Authorize(Policy = "Masters:Create")]
    public async Task<IActionResult> Create([FromBody] CreateLogisticUnitRequest request)
    {
        var result = await _service.CreateAsync(request);
        return result.Success ? CreatedAtAction(nameof(GetById), new { id = result.Data }, result) : BadRequest(result);
    }

    [HttpPut]
    [Authorize(Policy = "Masters:Update")]
    public async Task<IActionResult> Update([FromBody] UpdateLogisticUnitRequest request)
    {
        var result = await _service.UpdateAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "Masters:Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPatch("{id}/toggle-status")]
    [Authorize(Policy = "Masters:Delete")]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var result = await _service.ToggleStatusAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PavilionController : ControllerBase
{
    private readonly IPavilionService _service;
    public PavilionController(IPavilionService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] bool? status, [FromQuery] string? search)
    {
        var result = await _service.ListAsync(status, search);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    [Authorize(Policy = "Masters:Create")]
    public async Task<IActionResult> Create([FromBody] CreatePavilionRequest request)
    {
        var result = await _service.CreateAsync(request);
        return result.Success ? CreatedAtAction(nameof(GetById), new { id = result.Data }, result) : BadRequest(result);
    }

    [HttpPut]
    [Authorize(Policy = "Masters:Update")]
    public async Task<IActionResult> Update([FromBody] UpdatePavilionRequest request)
    {
        var result = await _service.UpdateAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "Masters:Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPatch("{id}/toggle-status")]
    [Authorize(Policy = "Masters:Delete")]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var result = await _service.ToggleStatusAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StallController : ControllerBase
{
    private readonly IStallService _service;
    public StallController(IStallService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] bool? status, [FromQuery] int? pavilionId, [FromQuery] string? search)
    {
        var result = await _service.ListAsync(status, pavilionId, search);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpGet("by-pavilion/{pavilionId}")]
    public async Task<IActionResult> ListByPavilion(int pavilionId)
    {
        var result = await _service.ListByPavilionAsync(pavilionId);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = "Masters:Create")]
    public async Task<IActionResult> Create([FromBody] CreateStallRequest request)
    {
        var result = await _service.CreateAsync(request);
        return result.Success ? CreatedAtAction(nameof(GetById), new { id = result.Data }, result) : BadRequest(result);
    }

    [HttpPut]
    [Authorize(Policy = "Masters:Update")]
    public async Task<IActionResult> Update([FromBody] UpdateStallRequest request)
    {
        var result = await _service.UpdateAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "Masters:Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPatch("{id}/toggle-status")]
    [Authorize(Policy = "Masters:Delete")]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var result = await _service.ToggleStatusAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
