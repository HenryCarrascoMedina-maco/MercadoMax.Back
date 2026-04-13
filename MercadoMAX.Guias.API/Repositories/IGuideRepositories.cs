using MercadoMAX.Guias.API.DTOs;
using MercadoMAX.Shared.DTOs;

namespace MercadoMAX.Guias.API.Repositories;

public interface IGuideRepository
{
    Task<GuideSpResult> CreateAsync(CreateGuideRequest request);
    Task<GuideReadResponse?> GetByIdAsync(int id);
    Task<(List<GuideListResponse> Items, int TotalRecords)> ListAsync(string? guideStatus, int? supplierId, int? carrierId, DateTime? dateFrom, DateTime? dateTo, string? search, int pageNumber, int pageSize);
    Task<List<GuideListResponse>> ListBySupplierAsync(int supplierId, string? guideStatus);
    Task<List<GuideListResponse>> ListByStallAsync(int stallId, string? guideStatus);
    Task<SpResult> UpdateStatusAsync(UpdateGuideStatusRequest request);
    Task<SpResult> VoidAsync(int id);
}

public interface IGuideDetailRepository
{
    Task<SpResult> CreateAsync(CreateGuideDetailRequest request);
    Task<SpResult> UpdateAsync(UpdateGuideDetailRequest request);
    Task<SpResult> DeleteAsync(int id);
    Task<List<GuideDetailResponse>> ListByGuideAsync(int guideId);
}
