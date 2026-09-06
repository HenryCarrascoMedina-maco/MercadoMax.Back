using MercadoMAX.Shared.DTOs;
using MercadoMAX.Finanzas.API.DTOs;
using MercadoMAX.Finanzas.API.Repositories;

namespace MercadoMAX.Finanzas.API.Services;

// ──────────────────────────────────────────────────────────
// SALE
// ──────────────────────────────────────────────────────────

public interface ISaleService
{
    Task<ApiResponse<int>> CreateAsync(CreateSaleRequest req);
    Task<ApiResponse<SaleReadResponse>> GetByIdAsync(int id);
    Task<PagedResponse<SaleListResponse>> ListAsync(
        int? stallId, string? paymentType, string? saleStatus,
        DateOnly? dateFrom, DateOnly? dateTo, string? search,
        int pageNumber, int pageSize);
    Task<ApiResponse<int>> CreateDetailAsync(CreateSaleDetailRequest req);
    Task<ApiResponse<bool>> VoidAsync(int id);
}

public class SaleService : ISaleService
{
    private readonly ISaleRepository _repo;
    public SaleService(ISaleRepository repo) => _repo = repo;

    public async Task<ApiResponse<int>> CreateAsync(CreateSaleRequest req)
    {
        var result = await _repo.CreateAsync(req);
        return result.Success == 1
            ? ApiResponse<int>.Ok((int)result.Id, result.Message)
            : ApiResponse<int>.Fail(result.Message);
    }

    public async Task<ApiResponse<SaleReadResponse>> GetByIdAsync(int id)
    {
        var data = await _repo.GetByIdAsync(id);
        return data is null
            ? ApiResponse<SaleReadResponse>.Fail("Sale not found")
            : ApiResponse<SaleReadResponse>.Ok(data);
    }

    public async Task<PagedResponse<SaleListResponse>> ListAsync(
        int? stallId, string? paymentType, string? saleStatus,
        DateOnly? dateFrom, DateOnly? dateTo, string? search,
        int pageNumber, int pageSize)
    {
        var (items, total) = await _repo.ListAsync(
            stallId, paymentType, saleStatus, dateFrom, dateTo, search, pageNumber, pageSize);

        return new PagedResponse<SaleListResponse>
        {
            Data = items,
            TotalRecords = total,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<ApiResponse<int>> CreateDetailAsync(CreateSaleDetailRequest req)
    {
        var result = await _repo.CreateDetailAsync(req);
        return result.Success == 1
            ? ApiResponse<int>.Ok((int)result.Id, result.Message)
            : ApiResponse<int>.Fail(result.Message);
    }

    public async Task<ApiResponse<bool>> VoidAsync(int id)
    {
        var result = await _repo.VoidAsync(id);
        return result.Success == 1
            ? ApiResponse<bool>.Ok(true, result.Message)
            : ApiResponse<bool>.Fail(result.Message);
    }
}

// ──────────────────────────────────────────────────────────
// ACCOUNT PAYABLE
// ──────────────────────────────────────────────────────────

public interface IAccountPayableService
{
    Task<ApiResponse<int>> CreateAsync(CreateAccountPayableRequest req);
    Task<ApiResponse<AccountPayableReadResponse>> GetByIdAsync(int id);
    Task<PagedResponse<AccountPayableListResponse>> ListAsync(
        int? stallId, int? supplierId, string? accountStatus,
        DateOnly? dateFrom, DateOnly? dateTo, string? search,
        int pageNumber, int pageSize);
    Task<ApiResponse<List<AccountPayableListResponse>>> ListByStallAsync(int stallId, string? accountStatus);
    Task<ApiResponse<List<GuideLineForPayableResponse>>> ListGuideLinesAsync(int guideId, int stallId);
    Task<ApiResponse<int>> CreateFromGuideAsync(CreateAccountPayableFromGuideRequest req);
    Task<ApiResponse<int>> CreatePaymentAsync(CreatePaymentRequest req);
}

public class AccountPayableService : IAccountPayableService
{
    private readonly IAccountPayableRepository _repo;
    public AccountPayableService(IAccountPayableRepository repo) => _repo = repo;

    public async Task<ApiResponse<int>> CreateAsync(CreateAccountPayableRequest req)
    {
        var result = await _repo.CreateAsync(req);
        return result.Success == 1
            ? ApiResponse<int>.Ok((int)result.Id, result.Message)
            : ApiResponse<int>.Fail(result.Message);
    }

    public async Task<ApiResponse<AccountPayableReadResponse>> GetByIdAsync(int id)
    {
        var data = await _repo.GetByIdAsync(id);
        return data is null
            ? ApiResponse<AccountPayableReadResponse>.Fail("Account payable not found")
            : ApiResponse<AccountPayableReadResponse>.Ok(data);
    }

    /// <summary>Lineas que la guia trae a ese puesto, para ponerles precio.</summary>
    public async Task<ApiResponse<List<GuideLineForPayableResponse>>> ListGuideLinesAsync(int guideId, int stallId)
        => ApiResponse<List<GuideLineForPayableResponse>>.Ok(await _repo.ListGuideLinesAsync(guideId, stallId));

    /// <summary>
    /// Alta desde la guia: guarda el precio de cada linea y crea la cuenta con
    /// el total que calcula el SP. El importe no se acepta del cliente.
    /// </summary>
    public async Task<ApiResponse<int>> CreateFromGuideAsync(CreateAccountPayableFromGuideRequest req)
    {
        var result = await _repo.CreateFromGuideAsync(req);
        return result.Success == 1
            ? ApiResponse<int>.Ok((int)result.Id, result.Message)
            : ApiResponse<int>.Fail(result.Message);
    }

    public async Task<PagedResponse<AccountPayableListResponse>> ListAsync(
        int? stallId, int? supplierId, string? accountStatus,
        DateOnly? dateFrom, DateOnly? dateTo, string? search,
        int pageNumber, int pageSize)
    {
        var (items, total) = await _repo.ListAsync(
            stallId, supplierId, accountStatus, dateFrom, dateTo, search, pageNumber, pageSize);

        return new PagedResponse<AccountPayableListResponse>
        {
            Data = items,
            TotalRecords = total,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<ApiResponse<List<AccountPayableListResponse>>> ListByStallAsync(int stallId, string? accountStatus)
    {
        var data = await _repo.ListByStallAsync(stallId, accountStatus);
        return ApiResponse<List<AccountPayableListResponse>>.Ok(data);
    }

    public async Task<ApiResponse<int>> CreatePaymentAsync(CreatePaymentRequest req)
    {
        var result = await _repo.CreatePaymentAsync(req);
        return result.Success == 1
            ? ApiResponse<int>.Ok((int)result.Id, result.Message)
            : ApiResponse<int>.Fail(result.Message);
    }
}

// ──────────────────────────────────────────────────────────
// REPORT
// ──────────────────────────────────────────────────────────

public interface IReportService
{
    Task<ApiResponse<AccountStatementResponse>> GetAccountStatementAsync(int stallId, DateOnly? dateFrom, DateOnly? dateTo);
}

public class ReportService : IReportService
{
    private readonly IReportRepository _repo;
    public ReportService(IReportRepository repo) => _repo = repo;

    public async Task<ApiResponse<AccountStatementResponse>> GetAccountStatementAsync(int stallId, DateOnly? dateFrom, DateOnly? dateTo)
    {
        var data = await _repo.GetAccountStatementAsync(stallId, dateFrom, dateTo);
        return data?.Summary is null
            ? ApiResponse<AccountStatementResponse>.Fail("No data found for this stall")
            : ApiResponse<AccountStatementResponse>.Ok(data);
    }
}
