using Dapper;
using MercadoMAX.Comerciante.API.DTOs;
using MercadoMAX.Shared.Data;
using MercadoMAX.Shared.DTOs;
using System.Data;

namespace MercadoMAX.Comerciante.API.Repositories;

// ── Interfaces ──────────────────────────────────────────
public interface IReceptionConfirmationRepository
{
    Task<SpResult> CreateAsync(CreateReceptionConfirmationRequest r);
    Task<List<ReceptionConfirmationResponse>> ListByStallAsync(int stallId, DateTime? dateFrom, DateTime? dateTo);
}

public interface IInventoryRepository
{
    Task<SpResult> CreateAsync(CreateInventoryRequest r);
    Task<InventoryResponse?> GetByIdAsync(int id);
    Task<List<InventoryResponse>> ListAsync(int? stallId, int? productId, bool? lowStock);
    Task<List<InventoryResponse>> ListByStallAsync(int stallId);
    Task<SpResult> UpdateAsync(UpdateInventoryRequest r);
}

public interface IInventoryMovementRepository
{
    Task<SpResult> CreateAsync(CreateInventoryMovementRequest r);
    Task<(List<InventoryMovementResponse> Items, int TotalRecords)> ListByInventoryAsync(
        int inventoryId, DateTime? dateFrom, DateTime? dateTo, int pageNumber, int pageSize);
}

// ── Implementations ─────────────────────────────────────

public class ReceptionConfirmationRepository : IReceptionConfirmationRepository
{
    private readonly DbConnectionFactory _db;
    public ReceptionConfirmationRepository(DbConnectionFactory db) => _db = db;

    public async Task<SpResult> CreateAsync(CreateReceptionConfirmationRequest r)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>("merchant.SP_CREATE_RECEPTION_CONFIRMATION",
            new { r.ReceptionId, r.StallId, r.ConfirmationDate, r.Observations, r.UserId },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<List<ReceptionConfirmationResponse>> ListByStallAsync(int stallId, DateTime? dateFrom, DateTime? dateTo)
    {
        using var conn = _db.CreateConnection();
        var result = await conn.QueryAsync<ReceptionConfirmationResponse>(
            "merchant.SP_LIST_RECEPTION_CONFIRMATION_BY_STALL",
            new { StallId = stallId, DateFrom = dateFrom, DateTo = dateTo },
            commandType: CommandType.StoredProcedure);
        return result.ToList();
    }
}

public class InventoryRepository : IInventoryRepository
{
    private readonly DbConnectionFactory _db;
    public InventoryRepository(DbConnectionFactory db) => _db = db;

    public async Task<SpResult> CreateAsync(CreateInventoryRequest r)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>("merchant.SP_CREATE_INVENTORY",
            new { r.StallId, r.ProductId, r.LogisticUnitId, r.CurrentStock, r.MinimumStock, r.AverageCost },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<InventoryResponse?> GetByIdAsync(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<InventoryResponse>("merchant.SP_READ_INVENTORY",
            new { Id = id }, commandType: CommandType.StoredProcedure);
    }

    public async Task<List<InventoryResponse>> ListAsync(int? stallId, int? productId, bool? lowStock)
    {
        using var conn = _db.CreateConnection();
        var result = await conn.QueryAsync<InventoryResponse>("merchant.SP_LIST_INVENTORY",
            new { StallId = stallId, ProductId = productId, LowStock = lowStock },
            commandType: CommandType.StoredProcedure);
        return result.ToList();
    }

    public async Task<List<InventoryResponse>> ListByStallAsync(int stallId)
    {
        using var conn = _db.CreateConnection();
        var result = await conn.QueryAsync<InventoryResponse>("merchant.SP_LIST_INVENTORY_BY_STALL",
            new { StallId = stallId }, commandType: CommandType.StoredProcedure);
        return result.ToList();
    }

    public async Task<SpResult> UpdateAsync(UpdateInventoryRequest r)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>("merchant.SP_UPDATE_INVENTORY",
            new { r.Id, r.MinimumStock }, commandType: CommandType.StoredProcedure);
    }
}

public class InventoryMovementRepository : IInventoryMovementRepository
{
    private readonly DbConnectionFactory _db;
    public InventoryMovementRepository(DbConnectionFactory db) => _db = db;

    public async Task<SpResult> CreateAsync(CreateInventoryMovementRequest r)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>("merchant.SP_CREATE_INVENTORY_MOVEMENT",
            new { r.InventoryId, r.MovementType, r.Quantity, r.ReferenceId, r.ReferenceType, r.Observations, r.UserId },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<(List<InventoryMovementResponse> Items, int TotalRecords)> ListByInventoryAsync(
        int inventoryId, DateTime? dateFrom, DateTime? dateTo, int pageNumber, int pageSize)
    {
        using var conn = _db.CreateConnection();
        using var multi = await conn.QueryMultipleAsync(
            "merchant.SP_LIST_INVENTORY_MOVEMENT_BY_INVENTORY",
            new { InventoryId = inventoryId, DateFrom = dateFrom, DateTo = dateTo,
                  PageNumber = pageNumber, PageSize = pageSize },
            commandType: CommandType.StoredProcedure);

        var items = (await multi.ReadAsync<InventoryMovementResponse>()).ToList();
        var total = await multi.ReadFirstAsync<int>();
        return (items, total);
    }
}
