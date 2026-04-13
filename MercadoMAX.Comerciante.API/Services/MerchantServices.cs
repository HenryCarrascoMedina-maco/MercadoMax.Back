using MercadoMAX.Comerciante.API.DTOs;
using MercadoMAX.Comerciante.API.Repositories;
using MercadoMAX.Shared.DTOs;

namespace MercadoMAX.Comerciante.API.Services;

// ── Interfaces ──────────────────────────────────────────
public interface IReceptionConfirmationService
{
    Task<ApiResponse<int>> CreateAsync(CreateReceptionConfirmationRequest r);
    Task<ApiResponse<List<ReceptionConfirmationResponse>>> ListByStallAsync(int stallId, DateTime? dateFrom, DateTime? dateTo);
}

public interface IInventoryService
{
    Task<ApiResponse<int>> CreateAsync(CreateInventoryRequest r);
    Task<ApiResponse<InventoryResponse>> GetByIdAsync(int id);
    Task<ApiResponse<List<InventoryResponse>>> ListAsync(int? stallId, int? productId, bool? lowStock);
    Task<ApiResponse<List<InventoryResponse>>> ListByStallAsync(int stallId);
    Task<ApiResponse<string>> UpdateAsync(UpdateInventoryRequest r);
}

public interface IInventoryMovementService
{
    Task<ApiResponse<int>> CreateAsync(CreateInventoryMovementRequest r);
    Task<PagedResponse<InventoryMovementResponse>> ListByInventoryAsync(
        int inventoryId, DateTime? dateFrom, DateTime? dateTo, int pageNumber, int pageSize);
}

// ── Implementations ─────────────────────────────────────

public class ReceptionConfirmationService : IReceptionConfirmationService
{
    private readonly IReceptionConfirmationRepository _repo;
    public ReceptionConfirmationService(IReceptionConfirmationRepository repo) => _repo = repo;

    public async Task<ApiResponse<int>> CreateAsync(CreateReceptionConfirmationRequest r)
    {
        var sp = await _repo.CreateAsync(r);
        return sp.Success == 1 ? ApiResponse<int>.Ok(sp.Id, sp.Message) : ApiResponse<int>.Fail(sp.Message);
    }

    public async Task<ApiResponse<List<ReceptionConfirmationResponse>>> ListByStallAsync(
        int stallId, DateTime? dateFrom, DateTime? dateTo)
    {
        var items = await _repo.ListByStallAsync(stallId, dateFrom, dateTo);
        return ApiResponse<List<ReceptionConfirmationResponse>>.Ok(items);
    }
}

public class InventoryService : IInventoryService
{
    private readonly IInventoryRepository _repo;
    public InventoryService(IInventoryRepository repo) => _repo = repo;

    public async Task<ApiResponse<int>> CreateAsync(CreateInventoryRequest r)
    {
        var sp = await _repo.CreateAsync(r);
        return sp.Success == 1 ? ApiResponse<int>.Ok(sp.Id, sp.Message) : ApiResponse<int>.Fail(sp.Message);
    }

    public async Task<ApiResponse<InventoryResponse>> GetByIdAsync(int id)
    {
        var item = await _repo.GetByIdAsync(id);
        return item != null ? ApiResponse<InventoryResponse>.Ok(item) : ApiResponse<InventoryResponse>.Fail("Inventory not found");
    }

    public async Task<ApiResponse<List<InventoryResponse>>> ListAsync(int? stallId, int? productId, bool? lowStock)
    {
        var items = await _repo.ListAsync(stallId, productId, lowStock);
        return ApiResponse<List<InventoryResponse>>.Ok(items);
    }

    public async Task<ApiResponse<List<InventoryResponse>>> ListByStallAsync(int stallId)
    {
        var items = await _repo.ListByStallAsync(stallId);
        return ApiResponse<List<InventoryResponse>>.Ok(items);
    }

    public async Task<ApiResponse<string>> UpdateAsync(UpdateInventoryRequest r)
    {
        var sp = await _repo.UpdateAsync(r);
        return sp.Success == 1 ? ApiResponse<string>.Ok("OK", sp.Message) : ApiResponse<string>.Fail(sp.Message);
    }
}

public class InventoryMovementService : IInventoryMovementService
{
    private readonly IInventoryMovementRepository _repo;
    public InventoryMovementService(IInventoryMovementRepository repo) => _repo = repo;

    public async Task<ApiResponse<int>> CreateAsync(CreateInventoryMovementRequest r)
    {
        var sp = await _repo.CreateAsync(r);
        return sp.Success == 1 ? ApiResponse<int>.Ok(sp.Id, sp.Message) : ApiResponse<int>.Fail(sp.Message);
    }

    public async Task<PagedResponse<InventoryMovementResponse>> ListByInventoryAsync(
        int inventoryId, DateTime? dateFrom, DateTime? dateTo, int pageNumber, int pageSize)
    {
        var (items, total) = await _repo.ListByInventoryAsync(inventoryId, dateFrom, dateTo, pageNumber, pageSize);
        return new PagedResponse<InventoryMovementResponse>
        {
            Success = true,
            Data = items,
            TotalRecords = total,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
