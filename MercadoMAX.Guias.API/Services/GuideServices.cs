using MercadoMAX.Guias.API.DTOs;
using MercadoMAX.Guias.API.Repositories;
using MercadoMAX.Shared.DTOs;

namespace MercadoMAX.Guias.API.Services;

// ── Interfaces ──────────────────────────────────────────
public interface IGuideService
{
    Task<ApiResponse<GuideSpResult>> CreateAsync(CreateGuideRequest request);
    Task<ApiResponse<GuideReadResponse>> GetByIdAsync(int id);
    Task<PagedResponse<GuideListResponse>> ListAsync(string? guideStatus, int? supplierId, int? carrierId, DateTime? dateFrom, DateTime? dateTo, string? search, int pageNumber, int pageSize);
    Task<ApiResponse<List<GuideListResponse>>> ListBySupplierAsync(int supplierId, string? guideStatus);
    Task<ApiResponse<List<GuideListResponse>>> ListByStallAsync(int stallId, string? guideStatus);
    Task<ApiResponse<string>> UpdateStatusAsync(UpdateGuideStatusRequest request);
    Task<ApiResponse<string>> VoidAsync(int id);
}

public interface IGuideDetailService
{
    Task<ApiResponse<int>> CreateAsync(CreateGuideDetailRequest request);
    Task<ApiResponse<string>> UpdateAsync(UpdateGuideDetailRequest request);
    Task<ApiResponse<string>> DeleteAsync(int id);
    Task<ApiResponse<List<GuideDetailResponse>>> ListByGuideAsync(int guideId);
}

// ── Implementations ─────────────────────────────────────

public class GuideService : IGuideService
{
    private readonly IGuideRepository _repo;
    public GuideService(IGuideRepository repo) => _repo = repo;

    public async Task<ApiResponse<GuideSpResult>> CreateAsync(CreateGuideRequest request)
    {
        var sp = await _repo.CreateAsync(request);
        return sp.Success == 1
            ? ApiResponse<GuideSpResult>.Ok(sp, sp.Message)
            : ApiResponse<GuideSpResult>.Fail(sp.Message);
    }

    public async Task<ApiResponse<GuideReadResponse>> GetByIdAsync(int id)
    {
        var guide = await _repo.GetByIdAsync(id);
        return guide != null
            ? ApiResponse<GuideReadResponse>.Ok(guide)
            : ApiResponse<GuideReadResponse>.Fail("Guide not found");
    }

    public async Task<PagedResponse<GuideListResponse>> ListAsync(
        string? guideStatus, int? supplierId, int? carrierId,
        DateTime? dateFrom, DateTime? dateTo, string? search, int pageNumber, int pageSize)
    {
        var (items, total) = await _repo.ListAsync(guideStatus, supplierId, carrierId, dateFrom, dateTo, search, pageNumber, pageSize);
        return new PagedResponse<GuideListResponse>
        {
            Data = items,
            TotalRecords = total,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<ApiResponse<List<GuideListResponse>>> ListBySupplierAsync(int supplierId, string? guideStatus)
    {
        var list = await _repo.ListBySupplierAsync(supplierId, guideStatus);
        return ApiResponse<List<GuideListResponse>>.Ok(list);
    }

    public async Task<ApiResponse<List<GuideListResponse>>> ListByStallAsync(int stallId, string? guideStatus)
    {
        var list = await _repo.ListByStallAsync(stallId, guideStatus);
        return ApiResponse<List<GuideListResponse>>.Ok(list);
    }

    public async Task<ApiResponse<string>> UpdateStatusAsync(UpdateGuideStatusRequest request)
    {
        var sp = await _repo.UpdateStatusAsync(request);
        return sp.Success == 1
            ? ApiResponse<string>.Ok(sp.Message)
            : ApiResponse<string>.Fail(sp.Message);
    }

    public async Task<ApiResponse<string>> VoidAsync(int id)
    {
        var sp = await _repo.VoidAsync(id);
        return sp.Success == 1
            ? ApiResponse<string>.Ok(sp.Message)
            : ApiResponse<string>.Fail(sp.Message);
    }
}

public class GuideDetailService : IGuideDetailService
{
    private readonly IGuideDetailRepository _repo;
    public GuideDetailService(IGuideDetailRepository repo) => _repo = repo;

    public async Task<ApiResponse<int>> CreateAsync(CreateGuideDetailRequest request)
    {
        var sp = await _repo.CreateAsync(request);
        return sp.Success == 1
            ? ApiResponse<int>.Ok(sp.Id, sp.Message)
            : ApiResponse<int>.Fail(sp.Message);
    }

    public async Task<ApiResponse<string>> UpdateAsync(UpdateGuideDetailRequest request)
    {
        var sp = await _repo.UpdateAsync(request);
        return sp.Success == 1
            ? ApiResponse<string>.Ok(sp.Message)
            : ApiResponse<string>.Fail(sp.Message);
    }

    public async Task<ApiResponse<string>> DeleteAsync(int id)
    {
        var sp = await _repo.DeleteAsync(id);
        return sp.Success == 1
            ? ApiResponse<string>.Ok(sp.Message)
            : ApiResponse<string>.Fail(sp.Message);
    }

    public async Task<ApiResponse<List<GuideDetailResponse>>> ListByGuideAsync(int guideId)
    {
        var list = await _repo.ListByGuideAsync(guideId);
        return ApiResponse<List<GuideDetailResponse>>.Ok(list);
    }
}
