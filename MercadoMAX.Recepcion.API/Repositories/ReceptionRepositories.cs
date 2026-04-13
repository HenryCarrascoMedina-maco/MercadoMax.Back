using Dapper;
using MercadoMAX.Recepcion.API.DTOs;
using MercadoMAX.Shared.Data;
using MercadoMAX.Shared.DTOs;
using System.Data;

namespace MercadoMAX.Recepcion.API.Repositories;

// ── Interfaces ──────────────────────────────────────────
public interface IReceptionRepository
{
    Task<SpResult> CreateAsync(CreateReceptionRequest r);
    Task<ReceptionReadResponse?> GetByIdAsync(int id);
    Task<(List<ReceptionListResponse> Items, int TotalRecords)> ListAsync(string? status, int? stallId, DateTime? dateFrom, DateTime? dateTo, int pageNumber, int pageSize);
    Task<SpResult> UpdateStatusAsync(UpdateReceptionStatusRequest r);
}

public interface IReceptionDetailRepository
{
    Task<SpResult> CreateAsync(CreateReceptionDetailRequest r);
    Task<SpResult> UpdateAsync(UpdateReceptionDetailRequest r);
    Task<List<ReceptionDetailResponse>> ListByReceptionAsync(int receptionId);
}

public interface IShortageRepository
{
    Task<SpResult> CreateAsync(CreateShortageRequest r);
    Task<(List<ShortageListResponse> Items, int TotalRecords)> ListAsync(string? claimStatus, int? receptionId, DateTime? dateFrom, DateTime? dateTo, int pageNumber, int pageSize);
    Task<SpResult> UpdateStatusAsync(UpdateShortageStatusRequest r);
    Task<List<ShortageResponse>> ListByReceptionAsync(int receptionId);
}

// ── Implementations ─────────────────────────────────────

public class ReceptionRepository : IReceptionRepository
{
    private readonly DbConnectionFactory _db;
    public ReceptionRepository(DbConnectionFactory db) => _db = db;

    public async Task<SpResult> CreateAsync(CreateReceptionRequest r)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>("reception.SP_CREATE_RECEPTION",
            new { r.GuideId, r.StallId, r.ReceptionDate, r.Observations, r.UserId },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<ReceptionReadResponse?> GetByIdAsync(int id)
    {
        using var conn = _db.CreateConnection();
        using var multi = await conn.QueryMultipleAsync("reception.SP_READ_RECEPTION",
            new { Id = id }, commandType: CommandType.StoredProcedure);

        var header = await multi.ReadFirstOrDefaultAsync<ReceptionHeaderResponse>();
        if (header == null) return null;

        var details = (await multi.ReadAsync<ReceptionDetailResponse>()).ToList();
        return new ReceptionReadResponse { Header = header, Details = details };
    }

    public async Task<(List<ReceptionListResponse> Items, int TotalRecords)> ListAsync(
        string? status, int? stallId, DateTime? dateFrom, DateTime? dateTo, int pageNumber, int pageSize)
    {
        using var conn = _db.CreateConnection();
        using var multi = await conn.QueryMultipleAsync("reception.SP_LIST_RECEPTION",
            new { ReceptionStatus = status, StallId = stallId, DateFrom = dateFrom, DateTo = dateTo,
                  PageNumber = pageNumber, PageSize = pageSize },
            commandType: CommandType.StoredProcedure);

        var items = (await multi.ReadAsync<ReceptionListResponse>()).ToList();
        var total = await multi.ReadFirstAsync<int>();
        return (items, total);
    }

    public async Task<SpResult> UpdateStatusAsync(UpdateReceptionStatusRequest r)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>("reception.SP_UPDATE_RECEPTION_STATUS",
            new { r.Id, r.ReceptionStatus }, commandType: CommandType.StoredProcedure);
    }
}

public class ReceptionDetailRepository : IReceptionDetailRepository
{
    private readonly DbConnectionFactory _db;
    public ReceptionDetailRepository(DbConnectionFactory db) => _db = db;

    public async Task<SpResult> CreateAsync(CreateReceptionDetailRequest r)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>("reception.SP_CREATE_RECEPTION_DETAIL",
            new { r.ReceptionId, r.ProductId, r.LogisticUnitId, r.ExpectedQuantity, r.ReceivedQuantity, r.Condition },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<SpResult> UpdateAsync(UpdateReceptionDetailRequest r)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>("reception.SP_UPDATE_RECEPTION_DETAIL",
            new { r.Id, r.ReceivedQuantity, r.Condition, r.ExpectedQuantity, r.ProductId },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<List<ReceptionDetailResponse>> ListByReceptionAsync(int receptionId)
    {
        using var conn = _db.CreateConnection();
        var result = await conn.QueryAsync<ReceptionDetailResponse>("reception.SP_LIST_RECEPTION_DETAIL_BY_RECEPTION",
            new { ReceptionId = receptionId }, commandType: CommandType.StoredProcedure);
        return result.ToList();
    }
}

public class ShortageRepository : IShortageRepository
{
    private readonly DbConnectionFactory _db;
    public ShortageRepository(DbConnectionFactory db) => _db = db;

    public async Task<SpResult> CreateAsync(CreateShortageRequest r)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>("reception.SP_CREATE_SHORTAGE",
            new { r.ReceptionDetailId, r.ShortageQuantity, r.Reason, r.Evidence, r.UserId },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<(List<ShortageListResponse> Items, int TotalRecords)> ListAsync(
        string? claimStatus, int? receptionId, DateTime? dateFrom, DateTime? dateTo, int pageNumber, int pageSize)
    {
        using var conn = _db.CreateConnection();
        using var multi = await conn.QueryMultipleAsync("reception.SP_LIST_SHORTAGE",
            new { ClaimStatus = claimStatus, ReceptionId = receptionId, DateFrom = dateFrom,
                  PageNumber = pageNumber, PageSize = pageSize },
            commandType: CommandType.StoredProcedure);

        var items = (await multi.ReadAsync<ShortageListResponse>()).ToList();
        var total = await multi.ReadFirstAsync<int>();
        return (items, total);
    }

    public async Task<SpResult> UpdateStatusAsync(UpdateShortageStatusRequest r)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>("reception.SP_UPDATE_SHORTAGE_STATUS",
            new { r.Id, r.ClaimStatus }, commandType: CommandType.StoredProcedure);
    }

    public async Task<List<ShortageResponse>> ListByReceptionAsync(int receptionId)
    {
        using var conn = _db.CreateConnection();
        var result = await conn.QueryAsync<ShortageResponse>("reception.SP_LIST_SHORTAGE_BY_RECEPTION",
            new { ReceptionId = receptionId }, commandType: CommandType.StoredProcedure);
        return result.ToList();
    }
}
