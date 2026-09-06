using Dapper;
using MercadoMAX.Maestros.API.DTOs;
using MercadoMAX.Shared.Data;
using MercadoMAX.Shared.DTOs;
using MercadoMAX.Shared.CrossCutting.Data;
using System.Data;

namespace MercadoMAX.Maestros.API.Repositories;

// ── ProductCategory (migrado a ISpExecutor — piloto Fase 1) ──
// Mismos SPs, mismos parámetros; solo cambia el medio de ejecución (centralizado en SpExecutor).
public class ProductCategoryRepository : IProductCategoryRepository
{
    private readonly ISpExecutor _sp;
    public ProductCategoryRepository(ISpExecutor sp) => _sp = sp;

    public Task<SpResult> CreateAsync(CreateProductCategoryRequest r)
        => _sp.ExecSpResultAsync("master.SP_CREATE_PRODUCT_CATEGORY", new { r.Name, r.Description });

    public Task<ProductCategoryResponse?> GetByIdAsync(int id)
        => _sp.QuerySingleSpAsync<ProductCategoryResponse>("master.SP_READ_PRODUCT_CATEGORY", new { Id = id });

    public Task<List<ProductCategoryResponse>> ListAsync(bool? status, string? search)
        => _sp.QueryListSpAsync<ProductCategoryResponse>("master.SP_LIST_PRODUCT_CATEGORY",
            new { Status = status, Search = search });

    public Task<SpResult> UpdateAsync(UpdateProductCategoryRequest r)
        => _sp.ExecSpResultAsync("master.SP_UPDATE_PRODUCT_CATEGORY",
            new { r.Id, r.Name, r.Description, r.Status });

    public Task<SpResult> DeleteAsync(int id)
        => _sp.ExecSpResultAsync("master.SP_DELETE_PRODUCT_CATEGORY", new { Id = id });

    public Task<SpResult> ToggleStatusAsync(int id)
        => _sp.ExecSpResultAsync("master.SP_TOGGLE_STATUS_PRODUCT_CATEGORY", new { Id = id });
}

// ── Product ─────────────────────────────────────────────
public class ProductRepository : IProductRepository
{
    private readonly DbConnectionFactory _db;
    public ProductRepository(DbConnectionFactory db) => _db = db;

    public async Task<SpResult> CreateAsync(CreateProductRequest r)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>("master.SP_CREATE_PRODUCT",
            new { r.Name, r.CategoryId, r.Description }, commandType: CommandType.StoredProcedure);
    }

    public async Task<ProductResponse?> GetByIdAsync(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<ProductResponse>("master.SP_READ_PRODUCT",
            new { Id = id }, commandType: CommandType.StoredProcedure);
    }

    public async Task<List<ProductResponse>> ListAsync(bool? status, int? categoryId, string? search)
    {
        using var conn = _db.CreateConnection();
        var result = await conn.QueryAsync<ProductResponse>("master.SP_LIST_PRODUCT",
            new { Status = status, CategoryId = categoryId, Search = search }, commandType: CommandType.StoredProcedure);
        return result.ToList();
    }

    public async Task<SpResult> UpdateAsync(UpdateProductRequest r)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>("master.SP_UPDATE_PRODUCT",
            new { r.Id, r.Name, r.CategoryId, r.Description, r.Status }, commandType: CommandType.StoredProcedure);
    }

    public async Task<SpResult> DeleteAsync(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>("master.SP_DELETE_PRODUCT",
            new { Id = id }, commandType: CommandType.StoredProcedure);
    }

    public async Task<SpResult> ToggleStatusAsync(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>("master.SP_TOGGLE_STATUS_PRODUCT",
            new { Id = id }, commandType: CommandType.StoredProcedure);
    }
}

// ── ProductSize ─────────────────────────────────────────
public class ProductSizeRepository : IProductSizeRepository
{
    private readonly DbConnectionFactory _db;
    public ProductSizeRepository(DbConnectionFactory db) => _db = db;

    public async Task<SpResult> CreateAsync(CreateProductSizeRequest r)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>("master.SP_CREATE_PRODUCT_SIZE",
            new { r.Name, r.ProductId }, commandType: CommandType.StoredProcedure);
    }

    public async Task<ProductSizeResponse?> GetByIdAsync(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<ProductSizeResponse>("master.SP_READ_PRODUCT_SIZE",
            new { Id = id }, commandType: CommandType.StoredProcedure);
    }

    public async Task<List<ProductSizeResponse>> ListAsync(bool? status, int? productId)
    {
        using var conn = _db.CreateConnection();
        var result = await conn.QueryAsync<ProductSizeResponse>("master.SP_LIST_PRODUCT_SIZE",
            new { Status = status, ProductId = productId }, commandType: CommandType.StoredProcedure);
        return result.ToList();
    }

    public async Task<SpResult> UpdateAsync(UpdateProductSizeRequest r)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>("master.SP_UPDATE_PRODUCT_SIZE",
            new { r.Id, r.Name, r.ProductId, r.Status }, commandType: CommandType.StoredProcedure);
    }

    public async Task<SpResult> DeleteAsync(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>("master.SP_DELETE_PRODUCT_SIZE",
            new { Id = id }, commandType: CommandType.StoredProcedure);
    }

    public async Task<SpResult> ToggleStatusAsync(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>("master.SP_TOGGLE_STATUS_PRODUCT_SIZE",
            new { Id = id }, commandType: CommandType.StoredProcedure);
    }
}

// ── Brand ───────────────────────────────────────────────
public class BrandRepository : IBrandRepository
{
    private readonly DbConnectionFactory _db;
    public BrandRepository(DbConnectionFactory db) => _db = db;

    public async Task<SpResult> CreateAsync(CreateBrandRequest r)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>("master.SP_CREATE_BRAND",
            new { r.Name, r.SupplierId, r.ProductId }, commandType: CommandType.StoredProcedure);
    }

    public async Task<BrandResponse?> GetByIdAsync(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<BrandResponse>("master.SP_READ_BRAND",
            new { Id = id }, commandType: CommandType.StoredProcedure);
    }

    public async Task<List<BrandResponse>> ListAsync(bool? status, int? supplierId, int? productId, string? search)
    {
        using var conn = _db.CreateConnection();
        var result = await conn.QueryAsync<BrandResponse>("master.SP_LIST_BRAND",
            new { Status = status, SupplierId = supplierId, ProductId = productId, Search = search },
            commandType: CommandType.StoredProcedure);
        return result.ToList();
    }

    public async Task<List<BrandResponse>> ListBySupplierAsync(int supplierId)
    {
        using var conn = _db.CreateConnection();
        var result = await conn.QueryAsync<BrandResponse>("master.SP_LIST_BRAND_BY_SUPPLIER",
            new { SupplierId = supplierId }, commandType: CommandType.StoredProcedure);
        return result.ToList();
    }

    public async Task<SpResult> UpdateAsync(UpdateBrandRequest r)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>("master.SP_UPDATE_BRAND",
            new { r.Id, r.Name, r.SupplierId, r.ProductId, r.Status }, commandType: CommandType.StoredProcedure);
    }

    public async Task<SpResult> DeleteAsync(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>("master.SP_DELETE_BRAND",
            new { Id = id }, commandType: CommandType.StoredProcedure);
    }

    public async Task<SpResult> ToggleStatusAsync(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>("master.SP_TOGGLE_STATUS_BRAND",
            new { Id = id }, commandType: CommandType.StoredProcedure);
    }
}

// ── Supplier ────────────────────────────────────────────
public class SupplierRepository : ISupplierRepository
{
    private readonly DbConnectionFactory _db;
    public SupplierRepository(DbConnectionFactory db) => _db = db;

    public async Task<SpResult> CreateAsync(CreateSupplierRequest r)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>("master.SP_CREATE_SUPPLIER",
            new { r.BusinessName, r.TaxId, r.Phone, r.Address, r.Province, r.Department, r.ContactName, r.ContactPhone, r.UserId },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<SupplierResponse?> GetByIdAsync(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<SupplierResponse>("master.SP_READ_SUPPLIER",
            new { Id = id }, commandType: CommandType.StoredProcedure);
    }

    public async Task<List<SupplierResponse>> ListAsync(bool? status, string? search)
    {
        using var conn = _db.CreateConnection();
        var result = await conn.QueryAsync<SupplierResponse>("master.SP_LIST_SUPPLIER",
            new { Status = status, Search = search }, commandType: CommandType.StoredProcedure);
        return result.ToList();
    }

    public async Task<SpResult> UpdateAsync(UpdateSupplierRequest r)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>("master.SP_UPDATE_SUPPLIER",
            new { r.Id, r.BusinessName, r.TaxId, r.Phone, r.Address, r.Province, r.Department, r.ContactName, r.ContactPhone, r.UserId, r.Status },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<SpResult> DeleteAsync(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>("master.SP_DELETE_SUPPLIER",
            new { Id = id }, commandType: CommandType.StoredProcedure);
    }

    public async Task<SpResult> ToggleStatusAsync(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>("master.SP_TOGGLE_STATUS_SUPPLIER",
            new { Id = id }, commandType: CommandType.StoredProcedure);
    }
}

// ── LogisticUnit ────────────────────────────────────────
public class LogisticUnitRepository : ILogisticUnitRepository
{
    private readonly DbConnectionFactory _db;
    public LogisticUnitRepository(DbConnectionFactory db) => _db = db;

    public async Task<SpResult> CreateAsync(CreateLogisticUnitRequest r)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>("master.SP_CREATE_LOGISTIC_UNIT",
            new { r.Name, r.Abbreviation }, commandType: CommandType.StoredProcedure);
    }

    public async Task<LogisticUnitResponse?> GetByIdAsync(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<LogisticUnitResponse>("master.SP_READ_LOGISTIC_UNIT",
            new { Id = id }, commandType: CommandType.StoredProcedure);
    }

    public async Task<List<LogisticUnitResponse>> ListAsync(bool? status)
    {
        using var conn = _db.CreateConnection();
        var result = await conn.QueryAsync<LogisticUnitResponse>("master.SP_LIST_LOGISTIC_UNIT",
            new { Status = status }, commandType: CommandType.StoredProcedure);
        return result.ToList();
    }

    public async Task<SpResult> UpdateAsync(UpdateLogisticUnitRequest r)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>("master.SP_UPDATE_LOGISTIC_UNIT",
            new { r.Id, r.Name, r.Abbreviation, r.Status }, commandType: CommandType.StoredProcedure);
    }

    public async Task<SpResult> DeleteAsync(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>("master.SP_DELETE_LOGISTIC_UNIT",
            new { Id = id }, commandType: CommandType.StoredProcedure);
    }

    public async Task<SpResult> ToggleStatusAsync(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>("master.SP_TOGGLE_STATUS_LOGISTIC_UNIT",
            new { Id = id }, commandType: CommandType.StoredProcedure);
    }
}

// ── Pavilion ────────────────────────────────────────────
public class PavilionRepository : IPavilionRepository
{
    private readonly DbConnectionFactory _db;
    public PavilionRepository(DbConnectionFactory db) => _db = db;

    public async Task<SpResult> CreateAsync(CreatePavilionRequest r)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>("master.SP_CREATE_PAVILION",
            new { r.Name, r.Category, r.Location }, commandType: CommandType.StoredProcedure);
    }

    public async Task<PavilionResponse?> GetByIdAsync(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<PavilionResponse>("master.SP_READ_PAVILION",
            new { Id = id }, commandType: CommandType.StoredProcedure);
    }

    public async Task<List<PavilionResponse>> ListAsync(bool? status, string? search)
    {
        using var conn = _db.CreateConnection();
        var result = await conn.QueryAsync<PavilionResponse>("master.SP_LIST_PAVILION",
            new { Status = status, Search = search }, commandType: CommandType.StoredProcedure);
        return result.ToList();
    }

    public async Task<SpResult> UpdateAsync(UpdatePavilionRequest r)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>("master.SP_UPDATE_PAVILION",
            new { r.Id, r.Name, r.Category, r.Location, r.Status }, commandType: CommandType.StoredProcedure);
    }

    public async Task<SpResult> DeleteAsync(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>("master.SP_DELETE_PAVILION",
            new { Id = id }, commandType: CommandType.StoredProcedure);
    }

    public async Task<SpResult> ToggleStatusAsync(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>("master.SP_TOGGLE_STATUS_PAVILION",
            new { Id = id }, commandType: CommandType.StoredProcedure);
    }
}

// ── Stall ───────────────────────────────────────────────
public class StallRepository : IStallRepository
{
    private readonly DbConnectionFactory _db;
    public StallRepository(DbConnectionFactory db) => _db = db;

    public async Task<SpResult> CreateAsync(CreateStallRequest r)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>("master.SP_CREATE_STALL",
            new { r.Number, r.PavilionId, r.UserId }, commandType: CommandType.StoredProcedure);
    }

    public async Task<StallResponse?> GetByIdAsync(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<StallResponse>("master.SP_READ_STALL",
            new { Id = id }, commandType: CommandType.StoredProcedure);
    }

    public async Task<List<StallResponse>> ListAsync(bool? status, int? pavilionId, string? search)
    {
        using var conn = _db.CreateConnection();
        var result = await conn.QueryAsync<StallResponse>("master.SP_LIST_STALL",
            new { Status = status, PavilionId = pavilionId, Search = search },
            commandType: CommandType.StoredProcedure);
        return result.ToList();
    }

    public async Task<List<StallResponse>> ListByPavilionAsync(int pavilionId)
    {
        using var conn = _db.CreateConnection();
        var result = await conn.QueryAsync<StallResponse>("master.SP_LIST_STALL_BY_PAVILION",
            new { PavilionId = pavilionId }, commandType: CommandType.StoredProcedure);
        return result.ToList();
    }

    public async Task<SpResult> UpdateAsync(UpdateStallRequest r)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>("master.SP_UPDATE_STALL",
            new { r.Id, r.Number, r.PavilionId, r.UserId, r.Status }, commandType: CommandType.StoredProcedure);
    }

    public async Task<SpResult> DeleteAsync(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>("master.SP_DELETE_STALL",
            new { Id = id }, commandType: CommandType.StoredProcedure);
    }

    public async Task<SpResult> ToggleStatusAsync(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>("master.SP_TOGGLE_STATUS_STALL",
            new { Id = id }, commandType: CommandType.StoredProcedure);
    }
}
