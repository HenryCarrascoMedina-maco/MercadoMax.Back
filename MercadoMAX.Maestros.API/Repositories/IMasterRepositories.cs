using MercadoMAX.Maestros.API.DTOs;
using MercadoMAX.Shared.DTOs;

namespace MercadoMAX.Maestros.API.Repositories;

public interface IProductCategoryRepository
{
    Task<SpResult> CreateAsync(CreateProductCategoryRequest request);
    Task<ProductCategoryResponse?> GetByIdAsync(int id);
    Task<List<ProductCategoryResponse>> ListAsync(bool? status, string? search);
    Task<SpResult> UpdateAsync(UpdateProductCategoryRequest request);
    Task<SpResult> DeleteAsync(int id);
    Task<SpResult> ToggleStatusAsync(int id);
}

public interface IProductRepository
{
    Task<SpResult> CreateAsync(CreateProductRequest request);
    Task<ProductResponse?> GetByIdAsync(int id);
    Task<List<ProductResponse>> ListAsync(bool? status, int? categoryId, string? search);
    Task<SpResult> UpdateAsync(UpdateProductRequest request);
    Task<SpResult> DeleteAsync(int id);
    Task<SpResult> ToggleStatusAsync(int id);
}

public interface IProductSizeRepository
{
    Task<SpResult> CreateAsync(CreateProductSizeRequest request);
    Task<ProductSizeResponse?> GetByIdAsync(int id);
    Task<List<ProductSizeResponse>> ListAsync(bool? status, int? productId);
    Task<SpResult> UpdateAsync(UpdateProductSizeRequest request);
    Task<SpResult> DeleteAsync(int id);
    Task<SpResult> ToggleStatusAsync(int id);
}

public interface IBrandRepository
{
    Task<SpResult> CreateAsync(CreateBrandRequest request);
    Task<BrandResponse?> GetByIdAsync(int id);
    Task<List<BrandResponse>> ListAsync(bool? status, int? supplierId, int? productId, string? search);
    Task<List<BrandResponse>> ListBySupplierAsync(int supplierId);
    Task<SpResult> UpdateAsync(UpdateBrandRequest request);
    Task<SpResult> DeleteAsync(int id);
    Task<SpResult> ToggleStatusAsync(int id);
}

public interface ISupplierRepository
{
    Task<SpResult> CreateAsync(CreateSupplierRequest request);
    Task<SupplierResponse?> GetByIdAsync(int id);
    Task<List<SupplierResponse>> ListAsync(bool? status, string? search);
    Task<SpResult> UpdateAsync(UpdateSupplierRequest request);
    Task<SpResult> DeleteAsync(int id);
    Task<SpResult> ToggleStatusAsync(int id);
}

public interface ILogisticUnitRepository
{
    Task<SpResult> CreateAsync(CreateLogisticUnitRequest request);
    Task<LogisticUnitResponse?> GetByIdAsync(int id);
    Task<List<LogisticUnitResponse>> ListAsync(bool? status);
    Task<SpResult> UpdateAsync(UpdateLogisticUnitRequest request);
    Task<SpResult> DeleteAsync(int id);
    Task<SpResult> ToggleStatusAsync(int id);
}

public interface IPavilionRepository
{
    Task<SpResult> CreateAsync(CreatePavilionRequest request);
    Task<PavilionResponse?> GetByIdAsync(int id);
    Task<List<PavilionResponse>> ListAsync(bool? status, string? search);
    Task<SpResult> UpdateAsync(UpdatePavilionRequest request);
    Task<SpResult> DeleteAsync(int id);
    Task<SpResult> ToggleStatusAsync(int id);
}

public interface IStallRepository
{
    Task<SpResult> CreateAsync(CreateStallRequest request);
    Task<StallResponse?> GetByIdAsync(int id);
    Task<List<StallResponse>> ListAsync(bool? status, int? pavilionId, string? search);
    Task<List<StallResponse>> ListByPavilionAsync(int pavilionId);
    Task<SpResult> UpdateAsync(UpdateStallRequest request);
    Task<SpResult> DeleteAsync(int id);
    Task<SpResult> ToggleStatusAsync(int id);
}
