using MercadoMAX.Maestros.API.DTOs;
using MercadoMAX.Maestros.API.Repositories;
using MercadoMAX.Shared.DTOs;

namespace MercadoMAX.Maestros.API.Services;

// ── Interfaces ──────────────────────────────────────────
public interface IProductCategoryService
{
    Task<ApiResponse<int>> CreateAsync(CreateProductCategoryRequest request);
    Task<ApiResponse<ProductCategoryResponse>> GetByIdAsync(int id);
    Task<ApiResponse<List<ProductCategoryResponse>>> ListAsync(bool? status, string? search);
    Task<ApiResponse<string>> UpdateAsync(UpdateProductCategoryRequest request);
    Task<ApiResponse<string>> DeleteAsync(int id);
    Task<ApiResponse<string>> ToggleStatusAsync(int id);
}

public interface IProductService
{
    Task<ApiResponse<int>> CreateAsync(CreateProductRequest request);
    Task<ApiResponse<ProductResponse>> GetByIdAsync(int id);
    Task<ApiResponse<List<ProductResponse>>> ListAsync(bool? status, int? categoryId, string? search);
    Task<ApiResponse<string>> UpdateAsync(UpdateProductRequest request);
    Task<ApiResponse<string>> DeleteAsync(int id);
    Task<ApiResponse<string>> ToggleStatusAsync(int id);
}

public interface IProductSizeService
{
    Task<ApiResponse<int>> CreateAsync(CreateProductSizeRequest request);
    Task<ApiResponse<ProductSizeResponse>> GetByIdAsync(int id);
    Task<ApiResponse<List<ProductSizeResponse>>> ListAsync(bool? status, int? productId);
    Task<ApiResponse<string>> UpdateAsync(UpdateProductSizeRequest request);
    Task<ApiResponse<string>> DeleteAsync(int id);
    Task<ApiResponse<string>> ToggleStatusAsync(int id);
}

public interface IBrandService
{
    Task<ApiResponse<int>> CreateAsync(CreateBrandRequest request);
    Task<ApiResponse<BrandResponse>> GetByIdAsync(int id);
    Task<ApiResponse<List<BrandResponse>>> ListAsync(bool? status, int? supplierId, int? productId, string? search);
    Task<ApiResponse<List<BrandResponse>>> ListBySupplierAsync(int supplierId);
    Task<ApiResponse<string>> UpdateAsync(UpdateBrandRequest request);
    Task<ApiResponse<string>> DeleteAsync(int id);
    Task<ApiResponse<string>> ToggleStatusAsync(int id);
}

public interface ISupplierService
{
    Task<ApiResponse<int>> CreateAsync(CreateSupplierRequest request);
    Task<ApiResponse<SupplierResponse>> GetByIdAsync(int id);
    Task<ApiResponse<List<SupplierResponse>>> ListAsync(bool? status, string? search);
    Task<ApiResponse<string>> UpdateAsync(UpdateSupplierRequest request);
    Task<ApiResponse<string>> DeleteAsync(int id);
    Task<ApiResponse<string>> ToggleStatusAsync(int id);
}

public interface ILogisticUnitService
{
    Task<ApiResponse<int>> CreateAsync(CreateLogisticUnitRequest request);
    Task<ApiResponse<LogisticUnitResponse>> GetByIdAsync(int id);
    Task<ApiResponse<List<LogisticUnitResponse>>> ListAsync(bool? status);
    Task<ApiResponse<string>> UpdateAsync(UpdateLogisticUnitRequest request);
    Task<ApiResponse<string>> DeleteAsync(int id);
    Task<ApiResponse<string>> ToggleStatusAsync(int id);
}

public interface IPavilionService
{
    Task<ApiResponse<int>> CreateAsync(CreatePavilionRequest request);
    Task<ApiResponse<PavilionResponse>> GetByIdAsync(int id);
    Task<ApiResponse<List<PavilionResponse>>> ListAsync(bool? status, string? search);
    Task<ApiResponse<string>> UpdateAsync(UpdatePavilionRequest request);
    Task<ApiResponse<string>> DeleteAsync(int id);
    Task<ApiResponse<string>> ToggleStatusAsync(int id);
}

public interface IStallService
{
    Task<ApiResponse<int>> CreateAsync(CreateStallRequest request);
    Task<ApiResponse<StallResponse>> GetByIdAsync(int id);
    Task<ApiResponse<List<StallResponse>>> ListAsync(bool? status, int? pavilionId, string? search);
    Task<ApiResponse<List<StallResponse>>> ListByPavilionAsync(int pavilionId);
    Task<ApiResponse<string>> UpdateAsync(UpdateStallRequest request);
    Task<ApiResponse<string>> DeleteAsync(int id);
    Task<ApiResponse<string>> ToggleStatusAsync(int id);
}

// ── Implementations ─────────────────────────────────────
public class ProductCategoryService : IProductCategoryService
{
    private readonly IProductCategoryRepository _repo;
    public ProductCategoryService(IProductCategoryRepository repo) => _repo = repo;

    public async Task<ApiResponse<int>> CreateAsync(CreateProductCategoryRequest request)
    {
        var r = await _repo.CreateAsync(request);
        return r.Success == 1 ? ApiResponse<int>.Ok(r.Id, r.Message) : ApiResponse<int>.Fail(r.Message);
    }

    public async Task<ApiResponse<ProductCategoryResponse>> GetByIdAsync(int id)
    {
        var item = await _repo.GetByIdAsync(id);
        return item != null ? ApiResponse<ProductCategoryResponse>.Ok(item) : ApiResponse<ProductCategoryResponse>.Fail("Not found");
    }

    public async Task<ApiResponse<List<ProductCategoryResponse>>> ListAsync(bool? status, string? search)
        => ApiResponse<List<ProductCategoryResponse>>.Ok(await _repo.ListAsync(status, search));

    public async Task<ApiResponse<string>> UpdateAsync(UpdateProductCategoryRequest request)
    {
        var r = await _repo.UpdateAsync(request);
        return r.Success == 1 ? ApiResponse<string>.Ok(r.Message) : ApiResponse<string>.Fail(r.Message);
    }

    public async Task<ApiResponse<string>> DeleteAsync(int id)
    {
        var r = await _repo.DeleteAsync(id);
        return r.Success == 1 ? ApiResponse<string>.Ok(r.Message) : ApiResponse<string>.Fail(r.Message);
    }

    public async Task<ApiResponse<string>> ToggleStatusAsync(int id)
    {
        var r = await _repo.ToggleStatusAsync(id);
        return r.Success == 1 ? ApiResponse<string>.Ok(r.Message) : ApiResponse<string>.Fail(r.Message);
    }
}

public class ProductService : IProductService
{
    private readonly IProductRepository _repo;
    public ProductService(IProductRepository repo) => _repo = repo;

    public async Task<ApiResponse<int>> CreateAsync(CreateProductRequest request)
    {
        var r = await _repo.CreateAsync(request);
        return r.Success == 1 ? ApiResponse<int>.Ok(r.Id, r.Message) : ApiResponse<int>.Fail(r.Message);
    }

    public async Task<ApiResponse<ProductResponse>> GetByIdAsync(int id)
    {
        var item = await _repo.GetByIdAsync(id);
        return item != null ? ApiResponse<ProductResponse>.Ok(item) : ApiResponse<ProductResponse>.Fail("Not found");
    }

    public async Task<ApiResponse<List<ProductResponse>>> ListAsync(bool? status, int? categoryId, string? search)
        => ApiResponse<List<ProductResponse>>.Ok(await _repo.ListAsync(status, categoryId, search));

    public async Task<ApiResponse<string>> UpdateAsync(UpdateProductRequest request)
    {
        var r = await _repo.UpdateAsync(request);
        return r.Success == 1 ? ApiResponse<string>.Ok(r.Message) : ApiResponse<string>.Fail(r.Message);
    }

    public async Task<ApiResponse<string>> DeleteAsync(int id)
    {
        var r = await _repo.DeleteAsync(id);
        return r.Success == 1 ? ApiResponse<string>.Ok(r.Message) : ApiResponse<string>.Fail(r.Message);
    }

    public async Task<ApiResponse<string>> ToggleStatusAsync(int id)
    {
        var r = await _repo.ToggleStatusAsync(id);
        return r.Success == 1 ? ApiResponse<string>.Ok(r.Message) : ApiResponse<string>.Fail(r.Message);
    }
}

public class ProductSizeService : IProductSizeService
{
    private readonly IProductSizeRepository _repo;
    public ProductSizeService(IProductSizeRepository repo) => _repo = repo;

    public async Task<ApiResponse<int>> CreateAsync(CreateProductSizeRequest request)
    {
        var r = await _repo.CreateAsync(request);
        return r.Success == 1 ? ApiResponse<int>.Ok(r.Id, r.Message) : ApiResponse<int>.Fail(r.Message);
    }

    public async Task<ApiResponse<ProductSizeResponse>> GetByIdAsync(int id)
    {
        var item = await _repo.GetByIdAsync(id);
        return item != null ? ApiResponse<ProductSizeResponse>.Ok(item) : ApiResponse<ProductSizeResponse>.Fail("Not found");
    }

    public async Task<ApiResponse<List<ProductSizeResponse>>> ListAsync(bool? status, int? productId)
        => ApiResponse<List<ProductSizeResponse>>.Ok(await _repo.ListAsync(status, productId));

    public async Task<ApiResponse<string>> UpdateAsync(UpdateProductSizeRequest request)
    {
        var r = await _repo.UpdateAsync(request);
        return r.Success == 1 ? ApiResponse<string>.Ok(r.Message) : ApiResponse<string>.Fail(r.Message);
    }

    public async Task<ApiResponse<string>> DeleteAsync(int id)
    {
        var r = await _repo.DeleteAsync(id);
        return r.Success == 1 ? ApiResponse<string>.Ok(r.Message) : ApiResponse<string>.Fail(r.Message);
    }

    public async Task<ApiResponse<string>> ToggleStatusAsync(int id)
    {
        var r = await _repo.ToggleStatusAsync(id);
        return r.Success == 1 ? ApiResponse<string>.Ok(r.Message) : ApiResponse<string>.Fail(r.Message);
    }
}

public class BrandService : IBrandService
{
    private readonly IBrandRepository _repo;
    public BrandService(IBrandRepository repo) => _repo = repo;

    public async Task<ApiResponse<int>> CreateAsync(CreateBrandRequest request)
    {
        var r = await _repo.CreateAsync(request);
        return r.Success == 1 ? ApiResponse<int>.Ok(r.Id, r.Message) : ApiResponse<int>.Fail(r.Message);
    }

    public async Task<ApiResponse<BrandResponse>> GetByIdAsync(int id)
    {
        var item = await _repo.GetByIdAsync(id);
        return item != null ? ApiResponse<BrandResponse>.Ok(item) : ApiResponse<BrandResponse>.Fail("Not found");
    }

    public async Task<ApiResponse<List<BrandResponse>>> ListAsync(bool? status, int? supplierId, int? productId, string? search)
        => ApiResponse<List<BrandResponse>>.Ok(await _repo.ListAsync(status, supplierId, productId, search));

    public async Task<ApiResponse<List<BrandResponse>>> ListBySupplierAsync(int supplierId)
        => ApiResponse<List<BrandResponse>>.Ok(await _repo.ListBySupplierAsync(supplierId));

    public async Task<ApiResponse<string>> UpdateAsync(UpdateBrandRequest request)
    {
        var r = await _repo.UpdateAsync(request);
        return r.Success == 1 ? ApiResponse<string>.Ok(r.Message) : ApiResponse<string>.Fail(r.Message);
    }

    public async Task<ApiResponse<string>> DeleteAsync(int id)
    {
        var r = await _repo.DeleteAsync(id);
        return r.Success == 1 ? ApiResponse<string>.Ok(r.Message) : ApiResponse<string>.Fail(r.Message);
    }

    public async Task<ApiResponse<string>> ToggleStatusAsync(int id)
    {
        var r = await _repo.ToggleStatusAsync(id);
        return r.Success == 1 ? ApiResponse<string>.Ok(r.Message) : ApiResponse<string>.Fail(r.Message);
    }
}

public class SupplierService : ISupplierService
{
    private readonly ISupplierRepository _repo;
    public SupplierService(ISupplierRepository repo) => _repo = repo;

    public async Task<ApiResponse<int>> CreateAsync(CreateSupplierRequest request)
    {
        var r = await _repo.CreateAsync(request);
        return r.Success == 1 ? ApiResponse<int>.Ok(r.Id, r.Message) : ApiResponse<int>.Fail(r.Message);
    }

    public async Task<ApiResponse<SupplierResponse>> GetByIdAsync(int id)
    {
        var item = await _repo.GetByIdAsync(id);
        return item != null ? ApiResponse<SupplierResponse>.Ok(item) : ApiResponse<SupplierResponse>.Fail("Not found");
    }

    public async Task<ApiResponse<List<SupplierResponse>>> ListAsync(bool? status, string? search)
        => ApiResponse<List<SupplierResponse>>.Ok(await _repo.ListAsync(status, search));

    public async Task<ApiResponse<string>> UpdateAsync(UpdateSupplierRequest request)
    {
        var r = await _repo.UpdateAsync(request);
        return r.Success == 1 ? ApiResponse<string>.Ok(r.Message) : ApiResponse<string>.Fail(r.Message);
    }

    public async Task<ApiResponse<string>> DeleteAsync(int id)
    {
        var r = await _repo.DeleteAsync(id);
        return r.Success == 1 ? ApiResponse<string>.Ok(r.Message) : ApiResponse<string>.Fail(r.Message);
    }

    public async Task<ApiResponse<string>> ToggleStatusAsync(int id)
    {
        var r = await _repo.ToggleStatusAsync(id);
        return r.Success == 1 ? ApiResponse<string>.Ok(r.Message) : ApiResponse<string>.Fail(r.Message);
    }
}

public class LogisticUnitService : ILogisticUnitService
{
    private readonly ILogisticUnitRepository _repo;
    public LogisticUnitService(ILogisticUnitRepository repo) => _repo = repo;

    public async Task<ApiResponse<int>> CreateAsync(CreateLogisticUnitRequest request)
    {
        var r = await _repo.CreateAsync(request);
        return r.Success == 1 ? ApiResponse<int>.Ok(r.Id, r.Message) : ApiResponse<int>.Fail(r.Message);
    }

    public async Task<ApiResponse<LogisticUnitResponse>> GetByIdAsync(int id)
    {
        var item = await _repo.GetByIdAsync(id);
        return item != null ? ApiResponse<LogisticUnitResponse>.Ok(item) : ApiResponse<LogisticUnitResponse>.Fail("Not found");
    }

    public async Task<ApiResponse<List<LogisticUnitResponse>>> ListAsync(bool? status)
        => ApiResponse<List<LogisticUnitResponse>>.Ok(await _repo.ListAsync(status));

    public async Task<ApiResponse<string>> UpdateAsync(UpdateLogisticUnitRequest request)
    {
        var r = await _repo.UpdateAsync(request);
        return r.Success == 1 ? ApiResponse<string>.Ok(r.Message) : ApiResponse<string>.Fail(r.Message);
    }

    public async Task<ApiResponse<string>> DeleteAsync(int id)
    {
        var r = await _repo.DeleteAsync(id);
        return r.Success == 1 ? ApiResponse<string>.Ok(r.Message) : ApiResponse<string>.Fail(r.Message);
    }

    public async Task<ApiResponse<string>> ToggleStatusAsync(int id)
    {
        var r = await _repo.ToggleStatusAsync(id);
        return r.Success == 1 ? ApiResponse<string>.Ok(r.Message) : ApiResponse<string>.Fail(r.Message);
    }
}

public class PavilionService : IPavilionService
{
    private readonly IPavilionRepository _repo;
    public PavilionService(IPavilionRepository repo) => _repo = repo;

    public async Task<ApiResponse<int>> CreateAsync(CreatePavilionRequest request)
    {
        var r = await _repo.CreateAsync(request);
        return r.Success == 1 ? ApiResponse<int>.Ok(r.Id, r.Message) : ApiResponse<int>.Fail(r.Message);
    }

    public async Task<ApiResponse<PavilionResponse>> GetByIdAsync(int id)
    {
        var item = await _repo.GetByIdAsync(id);
        return item != null ? ApiResponse<PavilionResponse>.Ok(item) : ApiResponse<PavilionResponse>.Fail("Not found");
    }

    public async Task<ApiResponse<List<PavilionResponse>>> ListAsync(bool? status, string? search)
        => ApiResponse<List<PavilionResponse>>.Ok(await _repo.ListAsync(status, search));

    public async Task<ApiResponse<string>> UpdateAsync(UpdatePavilionRequest request)
    {
        var r = await _repo.UpdateAsync(request);
        return r.Success == 1 ? ApiResponse<string>.Ok(r.Message) : ApiResponse<string>.Fail(r.Message);
    }

    public async Task<ApiResponse<string>> DeleteAsync(int id)
    {
        var r = await _repo.DeleteAsync(id);
        return r.Success == 1 ? ApiResponse<string>.Ok(r.Message) : ApiResponse<string>.Fail(r.Message);
    }

    public async Task<ApiResponse<string>> ToggleStatusAsync(int id)
    {
        var r = await _repo.ToggleStatusAsync(id);
        return r.Success == 1 ? ApiResponse<string>.Ok(r.Message) : ApiResponse<string>.Fail(r.Message);
    }
}

public class StallService : IStallService
{
    private readonly IStallRepository _repo;
    public StallService(IStallRepository repo) => _repo = repo;

    public async Task<ApiResponse<int>> CreateAsync(CreateStallRequest request)
    {
        var r = await _repo.CreateAsync(request);
        return r.Success == 1 ? ApiResponse<int>.Ok(r.Id, r.Message) : ApiResponse<int>.Fail(r.Message);
    }

    public async Task<ApiResponse<StallResponse>> GetByIdAsync(int id)
    {
        var item = await _repo.GetByIdAsync(id);
        return item != null ? ApiResponse<StallResponse>.Ok(item) : ApiResponse<StallResponse>.Fail("Not found");
    }

    public async Task<ApiResponse<List<StallResponse>>> ListAsync(bool? status, int? pavilionId, string? search)
        => ApiResponse<List<StallResponse>>.Ok(await _repo.ListAsync(status, pavilionId, search));

    public async Task<ApiResponse<List<StallResponse>>> ListByPavilionAsync(int pavilionId)
        => ApiResponse<List<StallResponse>>.Ok(await _repo.ListByPavilionAsync(pavilionId));

    public async Task<ApiResponse<string>> UpdateAsync(UpdateStallRequest request)
    {
        var r = await _repo.UpdateAsync(request);
        return r.Success == 1 ? ApiResponse<string>.Ok(r.Message) : ApiResponse<string>.Fail(r.Message);
    }

    public async Task<ApiResponse<string>> DeleteAsync(int id)
    {
        var r = await _repo.DeleteAsync(id);
        return r.Success == 1 ? ApiResponse<string>.Ok(r.Message) : ApiResponse<string>.Fail(r.Message);
    }

    public async Task<ApiResponse<string>> ToggleStatusAsync(int id)
    {
        var r = await _repo.ToggleStatusAsync(id);
        return r.Success == 1 ? ApiResponse<string>.Ok(r.Message) : ApiResponse<string>.Fail(r.Message);
    }
}
