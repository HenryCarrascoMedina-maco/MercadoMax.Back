using MercadoMAX.Transporte.API.DTOs;
using MercadoMAX.Transporte.API.Repositories;
using MercadoMAX.Shared.DTOs;

namespace MercadoMAX.Transporte.API.Services;

// ── Interfaces ──────────────────────────────────────────
public interface ICarrierService
{
    Task<ApiResponse<int>> CreateAsync(CreateCarrierRequest r);
    Task<ApiResponse<CarrierResponse>> GetByIdAsync(int id);
    Task<ApiResponse<List<CarrierResponse>>> ListAsync(bool? status, string? search);
    Task<ApiResponse<string>> UpdateAsync(UpdateCarrierRequest r);
    Task<ApiResponse<string>> DeleteAsync(int id);
    Task<ApiResponse<string>> ToggleStatusAsync(int id);
}

public interface ITruckService
{
    Task<ApiResponse<int>> CreateAsync(CreateTruckRequest r);
    Task<ApiResponse<TruckResponse>> GetByIdAsync(int id);
    Task<ApiResponse<List<TruckResponse>>> ListAsync(bool? status, int? carrierId, string? search);
    Task<ApiResponse<List<TruckResponse>>> ListByCarrierAsync(int carrierId);
    Task<ApiResponse<string>> UpdateAsync(UpdateTruckRequest r);
    Task<ApiResponse<string>> DeleteAsync(int id);
    Task<ApiResponse<string>> ToggleStatusAsync(int id);
}

public interface ITransportRateService
{
    Task<ApiResponse<int>> CreateAsync(CreateTransportRateRequest r);
    Task<ApiResponse<TransportRateResponse>> GetByIdAsync(int id);
    Task<ApiResponse<List<TransportRateResponse>>> ListAsync(bool? status, int? carrierId, int? logisticUnitId);
    Task<ApiResponse<List<TransportRateResponse>>> ListByCarrierAsync(int carrierId);
    Task<ApiResponse<string>> UpdateAsync(UpdateTransportRateRequest r);
    Task<ApiResponse<string>> DeleteAsync(int id);
    Task<ApiResponse<string>> ToggleStatusAsync(int id);
}

public interface ISettlementService
{
    Task<ApiResponse<int>> CreateAsync(CreateSettlementRequest r);
    Task<ApiResponse<SettlementReadResponse>> GetByIdAsync(int id);
    Task<PagedResponse<SettlementListResponse>> ListAsync(string? status, int? carrierId, DateTime? dateFrom, DateTime? dateTo, int pageNumber, int pageSize);
    Task<ApiResponse<string>> UpdateStatusAsync(UpdateSettlementStatusRequest r);
    Task<ApiResponse<string>> DeleteAsync(int id);
    Task<ApiResponse<int>> CreateDetailAsync(CreateSettlementDetailRequest r);
    Task<ApiResponse<string>> DeleteDetailAsync(int id);
}

// ── Implementations ─────────────────────────────────────

public class CarrierService : ICarrierService
{
    private readonly ICarrierRepository _repo;
    public CarrierService(ICarrierRepository repo) => _repo = repo;

    public async Task<ApiResponse<int>> CreateAsync(CreateCarrierRequest r)
    {
        var sp = await _repo.CreateAsync(r);
        return sp.Success == 1 ? ApiResponse<int>.Ok(sp.Id, sp.Message) : ApiResponse<int>.Fail(sp.Message);
    }

    public async Task<ApiResponse<CarrierResponse>> GetByIdAsync(int id)
    {
        var item = await _repo.GetByIdAsync(id);
        return item != null ? ApiResponse<CarrierResponse>.Ok(item) : ApiResponse<CarrierResponse>.Fail("Carrier not found");
    }

    public async Task<ApiResponse<List<CarrierResponse>>> ListAsync(bool? status, string? search)
    {
        var items = await _repo.ListAsync(status, search);
        return ApiResponse<List<CarrierResponse>>.Ok(items);
    }

    public async Task<ApiResponse<string>> UpdateAsync(UpdateCarrierRequest r)
    {
        var sp = await _repo.UpdateAsync(r);
        return sp.Success == 1 ? ApiResponse<string>.Ok("OK", sp.Message) : ApiResponse<string>.Fail(sp.Message);
    }

    public async Task<ApiResponse<string>> DeleteAsync(int id)
    {
        var sp = await _repo.DeleteAsync(id);
        return sp.Success == 1 ? ApiResponse<string>.Ok("OK", sp.Message) : ApiResponse<string>.Fail(sp.Message);
    }

    public async Task<ApiResponse<string>> ToggleStatusAsync(int id)
    {
        var sp = await _repo.ToggleStatusAsync(id);
        return sp.Success == 1 ? ApiResponse<string>.Ok("OK", sp.Message) : ApiResponse<string>.Fail(sp.Message);
    }
}

public class TruckService : ITruckService
{
    private readonly ITruckRepository _repo;
    public TruckService(ITruckRepository repo) => _repo = repo;

    public async Task<ApiResponse<int>> CreateAsync(CreateTruckRequest r)
    {
        var sp = await _repo.CreateAsync(r);
        return sp.Success == 1 ? ApiResponse<int>.Ok(sp.Id, sp.Message) : ApiResponse<int>.Fail(sp.Message);
    }

    public async Task<ApiResponse<TruckResponse>> GetByIdAsync(int id)
    {
        var item = await _repo.GetByIdAsync(id);
        return item != null ? ApiResponse<TruckResponse>.Ok(item) : ApiResponse<TruckResponse>.Fail("Truck not found");
    }

    public async Task<ApiResponse<List<TruckResponse>>> ListAsync(bool? status, int? carrierId, string? search)
    {
        var items = await _repo.ListAsync(status, carrierId, search);
        return ApiResponse<List<TruckResponse>>.Ok(items);
    }

    public async Task<ApiResponse<List<TruckResponse>>> ListByCarrierAsync(int carrierId)
    {
        var items = await _repo.ListByCarrierAsync(carrierId);
        return ApiResponse<List<TruckResponse>>.Ok(items);
    }

    public async Task<ApiResponse<string>> UpdateAsync(UpdateTruckRequest r)
    {
        var sp = await _repo.UpdateAsync(r);
        return sp.Success == 1 ? ApiResponse<string>.Ok("OK", sp.Message) : ApiResponse<string>.Fail(sp.Message);
    }

    public async Task<ApiResponse<string>> DeleteAsync(int id)
    {
        var sp = await _repo.DeleteAsync(id);
        return sp.Success == 1 ? ApiResponse<string>.Ok("OK", sp.Message) : ApiResponse<string>.Fail(sp.Message);
    }

    public async Task<ApiResponse<string>> ToggleStatusAsync(int id)
    {
        var sp = await _repo.ToggleStatusAsync(id);
        return sp.Success == 1 ? ApiResponse<string>.Ok("OK", sp.Message) : ApiResponse<string>.Fail(sp.Message);
    }
}

public class TransportRateService : ITransportRateService
{
    private readonly ITransportRateRepository _repo;
    public TransportRateService(ITransportRateRepository repo) => _repo = repo;

    public async Task<ApiResponse<int>> CreateAsync(CreateTransportRateRequest r)
    {
        var sp = await _repo.CreateAsync(r);
        return sp.Success == 1 ? ApiResponse<int>.Ok(sp.Id, sp.Message) : ApiResponse<int>.Fail(sp.Message);
    }

    public async Task<ApiResponse<TransportRateResponse>> GetByIdAsync(int id)
    {
        var item = await _repo.GetByIdAsync(id);
        return item != null ? ApiResponse<TransportRateResponse>.Ok(item) : ApiResponse<TransportRateResponse>.Fail("Transport rate not found");
    }

    public async Task<ApiResponse<List<TransportRateResponse>>> ListAsync(bool? status, int? carrierId, int? logisticUnitId)
    {
        var items = await _repo.ListAsync(status, carrierId, logisticUnitId);
        return ApiResponse<List<TransportRateResponse>>.Ok(items);
    }

    public async Task<ApiResponse<List<TransportRateResponse>>> ListByCarrierAsync(int carrierId)
    {
        var items = await _repo.ListByCarrierAsync(carrierId);
        return ApiResponse<List<TransportRateResponse>>.Ok(items);
    }

    public async Task<ApiResponse<string>> UpdateAsync(UpdateTransportRateRequest r)
    {
        var sp = await _repo.UpdateAsync(r);
        return sp.Success == 1 ? ApiResponse<string>.Ok("OK", sp.Message) : ApiResponse<string>.Fail(sp.Message);
    }

    public async Task<ApiResponse<string>> DeleteAsync(int id)
    {
        var sp = await _repo.DeleteAsync(id);
        return sp.Success == 1 ? ApiResponse<string>.Ok("OK", sp.Message) : ApiResponse<string>.Fail(sp.Message);
    }

    public async Task<ApiResponse<string>> ToggleStatusAsync(int id)
    {
        var sp = await _repo.ToggleStatusAsync(id);
        return sp.Success == 1 ? ApiResponse<string>.Ok("OK", sp.Message) : ApiResponse<string>.Fail(sp.Message);
    }
}

public class SettlementService : ISettlementService
{
    private readonly ISettlementRepository _repo;
    public SettlementService(ISettlementRepository repo) => _repo = repo;

    public async Task<ApiResponse<int>> CreateAsync(CreateSettlementRequest r)
    {
        var sp = await _repo.CreateAsync(r);
        return sp.Success == 1 ? ApiResponse<int>.Ok(sp.Id, sp.Message) : ApiResponse<int>.Fail(sp.Message);
    }

    public async Task<ApiResponse<SettlementReadResponse>> GetByIdAsync(int id)
    {
        var item = await _repo.GetByIdAsync(id);
        return item != null ? ApiResponse<SettlementReadResponse>.Ok(item) : ApiResponse<SettlementReadResponse>.Fail("Settlement not found");
    }

    public async Task<PagedResponse<SettlementListResponse>> ListAsync(
        string? status, int? carrierId, DateTime? dateFrom, DateTime? dateTo, int pageNumber, int pageSize)
    {
        var (items, total) = await _repo.ListAsync(status, carrierId, dateFrom, dateTo, pageNumber, pageSize);
        return new PagedResponse<SettlementListResponse>
        {
            Success = true,
            Data = items,
            TotalRecords = total,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<ApiResponse<string>> UpdateStatusAsync(UpdateSettlementStatusRequest r)
    {
        var sp = await _repo.UpdateStatusAsync(r);
        return sp.Success == 1 ? ApiResponse<string>.Ok("OK", sp.Message) : ApiResponse<string>.Fail(sp.Message);
    }

    public async Task<ApiResponse<int>> CreateDetailAsync(CreateSettlementDetailRequest r)
    {
        var sp = await _repo.CreateDetailAsync(r);
        return sp.Success == 1 ? ApiResponse<int>.Ok(sp.Id, sp.Message) : ApiResponse<int>.Fail(sp.Message);
    }

    /// <summary>
    /// Borra la liquidacion y sus lineas. Solo procede si sigue pendiente: una
    /// pagada es un registro contable y el SP la rechaza.
    /// </summary>
    public async Task<ApiResponse<string>> DeleteAsync(int id)
    {
        var sp = await _repo.DeleteAsync(id);
        return sp.Success == 1 ? ApiResponse<string>.Ok("OK", sp.Message) : ApiResponse<string>.Fail(sp.Message);
    }

    public async Task<ApiResponse<string>> DeleteDetailAsync(int id)
    {
        var sp = await _repo.DeleteDetailAsync(id);
        return sp.Success == 1 ? ApiResponse<string>.Ok("OK", sp.Message) : ApiResponse<string>.Fail(sp.Message);
    }
}
