using Dapper;
using MercadoMAX.Transporte.API.DTOs;
using MercadoMAX.Shared.Data;
using MercadoMAX.Shared.DTOs;
using System.Data;

namespace MercadoMAX.Transporte.API.Repositories;

// ── Interfaces ──────────────────────────────────────────
public interface ICarrierRepository
{
    Task<SpResult> CreateAsync(CreateCarrierRequest r);
    Task<CarrierResponse?> GetByIdAsync(int id);
    Task<List<CarrierResponse>> ListAsync(bool? status, string? search);
    Task<SpResult> UpdateAsync(UpdateCarrierRequest r);
    Task<SpResult> DeleteAsync(int id);
    Task<SpResult> ToggleStatusAsync(int id);
}

public interface ITruckRepository
{
    Task<SpResult> CreateAsync(CreateTruckRequest r);
    Task<TruckResponse?> GetByIdAsync(int id);
    Task<List<TruckResponse>> ListAsync(bool? status, int? carrierId, string? search);
    Task<List<TruckResponse>> ListByCarrierAsync(int carrierId);
    Task<SpResult> UpdateAsync(UpdateTruckRequest r);
    Task<SpResult> DeleteAsync(int id);
    Task<SpResult> ToggleStatusAsync(int id);
}

public interface ITransportRateRepository
{
    Task<SpResult> CreateAsync(CreateTransportRateRequest r);
    Task<TransportRateResponse?> GetByIdAsync(int id);
    Task<List<TransportRateResponse>> ListAsync(bool? status, int? carrierId, int? logisticUnitId);
    Task<List<TransportRateResponse>> ListByCarrierAsync(int carrierId);
    Task<SpResult> UpdateAsync(UpdateTransportRateRequest r);
    Task<SpResult> DeleteAsync(int id);
    Task<SpResult> ToggleStatusAsync(int id);
}

public interface ISettlementRepository
{
    Task<SpResult> CreateAsync(CreateSettlementRequest r);
    Task<SettlementReadResponse?> GetByIdAsync(int id);
    Task<(List<SettlementListResponse> Items, int TotalRecords)> ListAsync(string? status, int? carrierId, DateTime? dateFrom, DateTime? dateTo, int pageNumber, int pageSize);
    Task<SpResult> UpdateStatusAsync(UpdateSettlementStatusRequest r);
    Task<SpResult> CreateDetailAsync(CreateSettlementDetailRequest r);
    Task<SpResult> DeleteDetailAsync(int id);
}

// ── Implementations ─────────────────────────────────────

public class CarrierRepository : ICarrierRepository
{
    private readonly DbConnectionFactory _db;
    public CarrierRepository(DbConnectionFactory db) => _db = db;

    public async Task<SpResult> CreateAsync(CreateCarrierRequest r)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>("transport.SP_CREATE_CARRIER",
            new { r.FirstName, r.LastName, r.DocumentId, r.Phone, r.LicenseNumber, r.UserId },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<CarrierResponse?> GetByIdAsync(int id)
    {
        using var conn = _db.CreateConnection();
        using var multi = await conn.QueryMultipleAsync("transport.SP_READ_CARRIER",
            new { Id = id }, commandType: CommandType.StoredProcedure);
        return await multi.ReadFirstOrDefaultAsync<CarrierResponse>();
    }

    public async Task<List<CarrierResponse>> ListAsync(bool? status, string? search)
    {
        using var conn = _db.CreateConnection();
        var result = await conn.QueryAsync<CarrierResponse>("transport.SP_LIST_CARRIER",
            new { Status = status, Search = search }, commandType: CommandType.StoredProcedure);
        return result.ToList();
    }

    public async Task<SpResult> UpdateAsync(UpdateCarrierRequest r)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>("transport.SP_UPDATE_CARRIER",
            new { r.Id, r.FirstName, r.LastName, r.DocumentId, r.Phone, r.LicenseNumber, r.UserId, r.Status },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<SpResult> DeleteAsync(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>("transport.SP_DELETE_CARRIER",
            new { Id = id }, commandType: CommandType.StoredProcedure);
    }

    public async Task<SpResult> ToggleStatusAsync(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>("transport.SP_TOGGLE_STATUS_CARRIER",
            new { Id = id }, commandType: CommandType.StoredProcedure);
    }
}

public class TruckRepository : ITruckRepository
{
    private readonly DbConnectionFactory _db;
    public TruckRepository(DbConnectionFactory db) => _db = db;

    public async Task<SpResult> CreateAsync(CreateTruckRequest r)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>("transport.SP_CREATE_TRUCK",
            new { r.LicensePlate, r.CarrierId, r.Capacity, r.Brand, r.Model },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<TruckResponse?> GetByIdAsync(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<TruckResponse>("transport.SP_READ_TRUCK",
            new { Id = id }, commandType: CommandType.StoredProcedure);
    }

    public async Task<List<TruckResponse>> ListAsync(bool? status, int? carrierId, string? search)
    {
        using var conn = _db.CreateConnection();
        var result = await conn.QueryAsync<TruckResponse>("transport.SP_LIST_TRUCK",
            new { Status = status, CarrierId = carrierId, Search = search },
            commandType: CommandType.StoredProcedure);
        return result.ToList();
    }

    public async Task<List<TruckResponse>> ListByCarrierAsync(int carrierId)
    {
        using var conn = _db.CreateConnection();
        var result = await conn.QueryAsync<TruckResponse>("transport.SP_LIST_TRUCK_BY_CARRIER",
            new { CarrierId = carrierId }, commandType: CommandType.StoredProcedure);
        return result.ToList();
    }

    public async Task<SpResult> UpdateAsync(UpdateTruckRequest r)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>("transport.SP_UPDATE_TRUCK",
            new { r.Id, r.LicensePlate, r.CarrierId, r.Capacity, r.Brand, r.Model, r.Status },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<SpResult> DeleteAsync(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>("transport.SP_DELETE_TRUCK",
            new { Id = id }, commandType: CommandType.StoredProcedure);
    }

    public async Task<SpResult> ToggleStatusAsync(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>("transport.SP_TOGGLE_STATUS_TRUCK",
            new { Id = id }, commandType: CommandType.StoredProcedure);
    }
}

public class TransportRateRepository : ITransportRateRepository
{
    private readonly DbConnectionFactory _db;
    public TransportRateRepository(DbConnectionFactory db) => _db = db;

    public async Task<SpResult> CreateAsync(CreateTransportRateRequest r)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>("transport.SP_CREATE_TRANSPORT_RATE",
            new { r.CarrierId, r.LogisticUnitId, r.RouteOrigin, r.RouteDestination, r.UnitPrice, r.EffectiveDate },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<TransportRateResponse?> GetByIdAsync(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<TransportRateResponse>("transport.SP_READ_TRANSPORT_RATE",
            new { Id = id }, commandType: CommandType.StoredProcedure);
    }

    public async Task<List<TransportRateResponse>> ListAsync(bool? status, int? carrierId, int? logisticUnitId)
    {
        using var conn = _db.CreateConnection();
        var result = await conn.QueryAsync<TransportRateResponse>("transport.SP_LIST_TRANSPORT_RATE",
            new { Status = status, CarrierId = carrierId, LogisticUnitId = logisticUnitId },
            commandType: CommandType.StoredProcedure);
        return result.ToList();
    }

    public async Task<List<TransportRateResponse>> ListByCarrierAsync(int carrierId)
    {
        using var conn = _db.CreateConnection();
        var result = await conn.QueryAsync<TransportRateResponse>("transport.SP_LIST_TRANSPORT_RATE_BY_CARRIER",
            new { CarrierId = carrierId }, commandType: CommandType.StoredProcedure);
        return result.ToList();
    }

    public async Task<SpResult> UpdateAsync(UpdateTransportRateRequest r)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>("transport.SP_UPDATE_TRANSPORT_RATE",
            new { r.Id, r.CarrierId, r.LogisticUnitId, r.RouteOrigin, r.RouteDestination, r.UnitPrice, r.EffectiveDate, r.Status },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<SpResult> DeleteAsync(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>("transport.SP_DELETE_TRANSPORT_RATE",
            new { Id = id }, commandType: CommandType.StoredProcedure);
    }

    public async Task<SpResult> ToggleStatusAsync(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>("transport.SP_TOGGLE_STATUS_TRANSPORT_RATE",
            new { Id = id }, commandType: CommandType.StoredProcedure);
    }
}

public class SettlementRepository : ISettlementRepository
{
    private readonly DbConnectionFactory _db;
    public SettlementRepository(DbConnectionFactory db) => _db = db;

    public async Task<SpResult> CreateAsync(CreateSettlementRequest r)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>("transport.SP_CREATE_TRANSPORT_SETTLEMENT",
            new { r.CarrierId, r.TruckId, r.TripDate },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<SettlementReadResponse?> GetByIdAsync(int id)
    {
        using var conn = _db.CreateConnection();
        using var multi = await conn.QueryMultipleAsync("transport.SP_READ_TRANSPORT_SETTLEMENT",
            new { Id = id }, commandType: CommandType.StoredProcedure);

        var header = await multi.ReadFirstOrDefaultAsync<SettlementHeaderResponse>();
        if (header == null) return null;

        var details = (await multi.ReadAsync<SettlementDetailResponse>()).ToList();
        return new SettlementReadResponse { Header = header, Details = details };
    }

    public async Task<(List<SettlementListResponse> Items, int TotalRecords)> ListAsync(
        string? status, int? carrierId, DateTime? dateFrom, DateTime? dateTo, int pageNumber, int pageSize)
    {
        using var conn = _db.CreateConnection();
        using var multi = await conn.QueryMultipleAsync("transport.SP_LIST_TRANSPORT_SETTLEMENT",
            new { SettlementStatus = status, CarrierId = carrierId, DateFrom = dateFrom, DateTo = dateTo,
                  PageNumber = pageNumber, PageSize = pageSize },
            commandType: CommandType.StoredProcedure);

        var items = (await multi.ReadAsync<SettlementListResponse>()).ToList();
        var total = await multi.ReadFirstAsync<int>();
        return (items, total);
    }

    public async Task<SpResult> UpdateStatusAsync(UpdateSettlementStatusRequest r)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>("transport.SP_UPDATE_TRANSPORT_SETTLEMENT_STATUS",
            new { r.Id, r.SettlementStatus }, commandType: CommandType.StoredProcedure);
    }

    public async Task<SpResult> CreateDetailAsync(CreateSettlementDetailRequest r)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>("transport.SP_CREATE_SETTLEMENT_DETAIL",
            new { r.SettlementId, r.GuideId, r.LogisticUnitId, r.Quantity, r.UnitPrice },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<SpResult> DeleteDetailAsync(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>("transport.SP_DELETE_SETTLEMENT_DETAIL",
            new { Id = id }, commandType: CommandType.StoredProcedure);
    }
}
