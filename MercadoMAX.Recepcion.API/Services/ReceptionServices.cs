using MercadoMAX.Recepcion.API.DTOs;
using MercadoMAX.Recepcion.API.Repositories;
using MercadoMAX.Shared.DTOs;

namespace MercadoMAX.Recepcion.API.Services;

// ── Interfaces ──────────────────────────────────────────
public interface IReceptionService
{
    Task<ApiResponse<int>> CreateAsync(CreateReceptionRequest r);
    Task<ApiResponse<ReceptionReadResponse>> GetByIdAsync(int id);
    Task<PagedResponse<ReceptionListResponse>> ListAsync(string? status, int? stallId, DateTime? dateFrom, DateTime? dateTo, int pageNumber, int pageSize);
    Task<ApiResponse<string>> UpdateStatusAsync(UpdateReceptionStatusRequest r);
}

public interface IReceptionDetailService
{
    Task<ApiResponse<int>> CreateAsync(CreateReceptionDetailRequest r);
    Task<ApiResponse<string>> UpdateAsync(UpdateReceptionDetailRequest r);
    Task<ApiResponse<List<ReceptionDetailResponse>>> ListByReceptionAsync(int receptionId);
}

public interface IShortageService
{
    Task<ApiResponse<int>> CreateAsync(CreateShortageRequest r);
    Task<PagedResponse<ShortageListResponse>> ListAsync(string? claimStatus, int? receptionId, DateTime? dateFrom, DateTime? dateTo, int pageNumber, int pageSize);
    Task<ApiResponse<string>> UpdateStatusAsync(UpdateShortageStatusRequest r);
    Task<ApiResponse<List<ShortageResponse>>> ListByReceptionAsync(int receptionId);
}

// ── Implementations ─────────────────────────────────────

public class ReceptionService : IReceptionService
{
    private readonly IReceptionRepository _repo;
    public ReceptionService(IReceptionRepository repo) => _repo = repo;

    public async Task<ApiResponse<int>> CreateAsync(CreateReceptionRequest r)
    {
        var sp = await _repo.CreateAsync(r);
        return sp.Success == 1 ? ApiResponse<int>.Ok(sp.Id, sp.Message) : ApiResponse<int>.Fail(sp.Message);
    }

    public async Task<ApiResponse<ReceptionReadResponse>> GetByIdAsync(int id)
    {
        var item = await _repo.GetByIdAsync(id);
        return item != null ? ApiResponse<ReceptionReadResponse>.Ok(item) : ApiResponse<ReceptionReadResponse>.Fail("Reception not found");
    }

    public async Task<PagedResponse<ReceptionListResponse>> ListAsync(
        string? status, int? stallId, DateTime? dateFrom, DateTime? dateTo, int pageNumber, int pageSize)
    {
        var (items, total) = await _repo.ListAsync(status, stallId, dateFrom, dateTo, pageNumber, pageSize);
        return new PagedResponse<ReceptionListResponse>
        {
            Success = true,
            Data = items,
            TotalRecords = total,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<ApiResponse<string>> UpdateStatusAsync(UpdateReceptionStatusRequest r)
    {
        var sp = await _repo.UpdateStatusAsync(r);
        return sp.Success == 1 ? ApiResponse<string>.Ok("OK", sp.Message) : ApiResponse<string>.Fail(sp.Message);
    }
}

public class ReceptionDetailService : IReceptionDetailService
{
    private readonly IReceptionDetailRepository _repo;
    public ReceptionDetailService(IReceptionDetailRepository repo) => _repo = repo;

    public async Task<ApiResponse<int>> CreateAsync(CreateReceptionDetailRequest r)
    {
        var sp = await _repo.CreateAsync(r);
        return sp.Success == 1 ? ApiResponse<int>.Ok(sp.Id, sp.Message) : ApiResponse<int>.Fail(sp.Message);
    }

    public async Task<ApiResponse<string>> UpdateAsync(UpdateReceptionDetailRequest r)
    {
        var sp = await _repo.UpdateAsync(r);
        return sp.Success == 1 ? ApiResponse<string>.Ok("OK", sp.Message) : ApiResponse<string>.Fail(sp.Message);
    }

    public async Task<ApiResponse<List<ReceptionDetailResponse>>> ListByReceptionAsync(int receptionId)
    {
        var items = await _repo.ListByReceptionAsync(receptionId);
        return ApiResponse<List<ReceptionDetailResponse>>.Ok(items);
    }
}

public class ShortageService : IShortageService
{
    private readonly IShortageRepository _repo;
    public ShortageService(IShortageRepository repo) => _repo = repo;

    public async Task<ApiResponse<int>> CreateAsync(CreateShortageRequest r)
    {
        var sp = await _repo.CreateAsync(r);
        return sp.Success == 1 ? ApiResponse<int>.Ok(sp.Id, sp.Message) : ApiResponse<int>.Fail(sp.Message);
    }

    public async Task<PagedResponse<ShortageListResponse>> ListAsync(
        string? claimStatus, int? receptionId, DateTime? dateFrom, DateTime? dateTo, int pageNumber, int pageSize)
    {
        var (items, total) = await _repo.ListAsync(claimStatus, receptionId, dateFrom, dateTo, pageNumber, pageSize);
        return new PagedResponse<ShortageListResponse>
        {
            Success = true,
            Data = items,
            TotalRecords = total,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<ApiResponse<string>> UpdateStatusAsync(UpdateShortageStatusRequest r)
    {
        var sp = await _repo.UpdateStatusAsync(r);
        return sp.Success == 1 ? ApiResponse<string>.Ok("OK", sp.Message) : ApiResponse<string>.Fail(sp.Message);
    }

    public async Task<ApiResponse<List<ShortageResponse>>> ListByReceptionAsync(int receptionId)
    {
        var items = await _repo.ListByReceptionAsync(receptionId);
        return ApiResponse<List<ShortageResponse>>.Ok(items);
    }
}
