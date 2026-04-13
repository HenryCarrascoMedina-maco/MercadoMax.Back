using Dapper;
using MercadoMAX.Guias.API.DTOs;
using MercadoMAX.Shared.Data;
using MercadoMAX.Shared.DTOs;
using System.Data;

namespace MercadoMAX.Guias.API.Repositories;

// ── Guide ───────────────────────────────────────────────
public class GuideRepository : IGuideRepository
{
    private readonly DbConnectionFactory _db;
    public GuideRepository(DbConnectionFactory db) => _db = db;

    public async Task<GuideSpResult> CreateAsync(CreateGuideRequest r)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<GuideSpResult>("guide.SP_CREATE_GUIDE",
            new { r.SupplierId, r.CarrierId, r.TruckId, r.ShipmentDate, r.EstimatedArrivalDate, r.Observations, r.CreatedBy },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<GuideReadResponse?> GetByIdAsync(int id)
    {
        using var conn = _db.CreateConnection();
        using var multi = await conn.QueryMultipleAsync("guide.SP_READ_GUIDE",
            new { Id = id }, commandType: CommandType.StoredProcedure);

        var header = await multi.ReadFirstOrDefaultAsync<GuideHeaderResponse>();
        if (header == null) return null;

        var details = (await multi.ReadAsync<GuideDetailResponse>()).ToList();
        return new GuideReadResponse { Header = header, Details = details };
    }

    public async Task<(List<GuideListResponse> Items, int TotalRecords)> ListAsync(
        string? guideStatus, int? supplierId, int? carrierId,
        DateTime? dateFrom, DateTime? dateTo, string? search, int pageNumber, int pageSize)
    {
        using var conn = _db.CreateConnection();
        using var multi = await conn.QueryMultipleAsync("guide.SP_LIST_GUIDE",
            new { GuideStatus = guideStatus, SupplierId = supplierId, CarrierId = carrierId,
                  DateFrom = dateFrom, DateTo = dateTo, Search = search,
                  PageNumber = pageNumber, PageSize = pageSize },
            commandType: CommandType.StoredProcedure);

        var items = (await multi.ReadAsync<GuideListResponse>()).ToList();
        var total = await multi.ReadFirstAsync<int>();
        return (items, total);
    }

    public async Task<List<GuideListResponse>> ListBySupplierAsync(int supplierId, string? guideStatus)
    {
        using var conn = _db.CreateConnection();
        var result = await conn.QueryAsync<GuideListResponse>("guide.SP_LIST_GUIDE_BY_SUPPLIER",
            new { SupplierId = supplierId, GuideStatus = guideStatus },
            commandType: CommandType.StoredProcedure);
        return result.ToList();
    }

    public async Task<List<GuideListResponse>> ListByStallAsync(int stallId, string? guideStatus)
    {
        using var conn = _db.CreateConnection();
        var result = await conn.QueryAsync<GuideListResponse>("guide.SP_LIST_GUIDE_BY_STALL",
            new { StallId = stallId, GuideStatus = guideStatus },
            commandType: CommandType.StoredProcedure);
        return result.ToList();
    }

    public async Task<SpResult> UpdateStatusAsync(UpdateGuideStatusRequest r)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>("guide.SP_UPDATE_GUIDE_STATUS",
            new { r.Id, r.GuideStatus }, commandType: CommandType.StoredProcedure);
    }

    public async Task<SpResult> VoidAsync(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>("guide.SP_VOID_GUIDE",
            new { Id = id }, commandType: CommandType.StoredProcedure);
    }
}

// ── GuideDetail ─────────────────────────────────────────
public class GuideDetailRepository : IGuideDetailRepository
{
    private readonly DbConnectionFactory _db;
    public GuideDetailRepository(DbConnectionFactory db) => _db = db;

    public async Task<SpResult> CreateAsync(CreateGuideDetailRequest r)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>("guide.SP_CREATE_GUIDE_DETAIL",
            new { r.GuideId, r.DestinationStallId, r.ProductId, r.BrandId, r.ProductSizeId,
                  r.LogisticUnitId, r.Quantity, r.UnitPrice, r.TransportUnitPrice, r.Observations },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<SpResult> UpdateAsync(UpdateGuideDetailRequest r)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>("guide.SP_UPDATE_GUIDE_DETAIL",
            new { r.Id, r.DestinationStallId, r.ProductId, r.BrandId, r.ProductSizeId,
                  r.LogisticUnitId, r.Quantity, r.UnitPrice, r.TransportUnitPrice, r.Observations },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<SpResult> DeleteAsync(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>("guide.SP_DELETE_GUIDE_DETAIL",
            new { Id = id }, commandType: CommandType.StoredProcedure);
    }

    public async Task<List<GuideDetailResponse>> ListByGuideAsync(int guideId)
    {
        using var conn = _db.CreateConnection();
        var result = await conn.QueryAsync<GuideDetailResponse>("guide.SP_LIST_GUIDE_DETAIL_BY_GUIDE",
            new { GuideId = guideId }, commandType: CommandType.StoredProcedure);
        return result.ToList();
    }
}
