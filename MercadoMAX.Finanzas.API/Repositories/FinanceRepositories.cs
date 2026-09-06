using System.Data;
using System.Text.Json;
using Dapper;
using MercadoMAX.Shared.Data;
using MercadoMAX.Shared.DTOs;
using MercadoMAX.Finanzas.API.DTOs;

namespace MercadoMAX.Finanzas.API.Repositories;

// ──────────────────────────────────────────────────────────
// SALE
// ──────────────────────────────────────────────────────────

public interface ISaleRepository
{
    Task<SpResult> CreateAsync(CreateSaleRequest req);
    Task<SaleReadResponse?> GetByIdAsync(int id);
    Task<(List<SaleListResponse> Items, int TotalRecords)> ListAsync(
        int? stallId, string? paymentType, string? saleStatus,
        DateOnly? dateFrom, DateOnly? dateTo, string? search,
        int pageNumber, int pageSize);
    Task<SpResult> CreateDetailAsync(CreateSaleDetailRequest req);
    Task<SpResult> VoidAsync(int id);
}

public class SaleRepository : ISaleRepository
{
    private readonly DbConnectionFactory _db;
    public SaleRepository(DbConnectionFactory db) => _db = db;

    public async Task<SpResult> CreateAsync(CreateSaleRequest req)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>(
            "finance.SP_CREATE_SALE",
            new { req.StallId, req.CustomerName, req.PaymentType, req.UserId },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<SaleReadResponse?> GetByIdAsync(int id)
    {
        using var conn = _db.CreateConnection();
        using var multi = await conn.QueryMultipleAsync(
            "finance.SP_READ_SALE",
            new { Id = id },
            commandType: CommandType.StoredProcedure);

        var header = await multi.ReadFirstOrDefaultAsync<SaleHeaderResponse>();
        if (header == null) return null;

        var details = (await multi.ReadAsync<SaleDetailResponse>()).ToList();
        return new SaleReadResponse { Header = header, Details = details };
    }

    public async Task<(List<SaleListResponse> Items, int TotalRecords)> ListAsync(
        int? stallId, string? paymentType, string? saleStatus,
        DateOnly? dateFrom, DateOnly? dateTo, string? search,
        int pageNumber, int pageSize)
    {
        using var conn = _db.CreateConnection();
        using var multi = await conn.QueryMultipleAsync(
            "finance.SP_LIST_SALE",
            new
            {
                StallId = stallId,
                PaymentType = paymentType,
                SaleStatus = saleStatus,
                DateFrom = dateFrom.HasValue ? (DateTime?)dateFrom.Value.ToDateTime(TimeOnly.MinValue) : null,
                DateTo = dateTo.HasValue ? (DateTime?)dateTo.Value.ToDateTime(TimeOnly.MinValue) : null,
                Search = search,
                PageNumber = pageNumber,
                PageSize = pageSize
            },
            commandType: CommandType.StoredProcedure);

        var items = (await multi.ReadAsync<SaleListResponse>()).ToList();
        var total = await multi.ReadFirstAsync<int>();
        return (items, total);
    }

    public async Task<SpResult> CreateDetailAsync(CreateSaleDetailRequest req)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>(
            "finance.SP_CREATE_SALE_DETAIL",
            new
            {
                req.SaleId, req.ProductId, req.BrandId, req.ProductSizeId,
                req.LogisticUnitId, req.Quantity, req.UnitPrice
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<SpResult> VoidAsync(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>(
            "finance.SP_VOID_SALE",
            new { Id = id },
            commandType: CommandType.StoredProcedure);
    }
}

// ──────────────────────────────────────────────────────────
// ACCOUNT PAYABLE
// ──────────────────────────────────────────────────────────

public interface IAccountPayableRepository
{
    Task<SpResult> CreateAsync(CreateAccountPayableRequest req);
    Task<AccountPayableReadResponse?> GetByIdAsync(int id);
    Task<(List<AccountPayableListResponse> Items, int TotalRecords)> ListAsync(
        int? stallId, int? supplierId, string? accountStatus,
        DateOnly? dateFrom, DateOnly? dateTo, string? search,
        int pageNumber, int pageSize);
    Task<List<AccountPayableListResponse>> ListByStallAsync(int stallId, string? accountStatus);
    Task<List<GuideLineForPayableResponse>> ListGuideLinesAsync(int guideId, int stallId);
    Task<SpResult> CreateFromGuideAsync(CreateAccountPayableFromGuideRequest req);
    Task<SpResult> CreatePaymentAsync(CreatePaymentRequest req);
}

public class AccountPayableRepository : IAccountPayableRepository
{
    private readonly DbConnectionFactory _db;
    public AccountPayableRepository(DbConnectionFactory db) => _db = db;

    public async Task<SpResult> CreateAsync(CreateAccountPayableRequest req)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>(
            "finance.SP_CREATE_ACCOUNT_PAYABLE",
            new { req.StallId, req.SupplierId, req.GuideId, req.TotalAmount, req.DueDate },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<List<GuideLineForPayableResponse>> ListGuideLinesAsync(int guideId, int stallId)
    {
        using var conn = _db.CreateConnection();
        var rows = await conn.QueryAsync<GuideLineForPayableResponse>(
            "finance.SP_LIST_GUIDE_DETAIL_BY_STALL",
            new { GuideId = guideId, StallId = stallId },
            commandType: CommandType.StoredProcedure);
        return rows.ToList();
    }

    /// <summary>
    /// Las lineas viajan como JSON porque el SP las lee con OPENJSON: evita un
    /// tipo tabla en base de datos y mantiene el alta en una sola transaccion.
    /// </summary>
    public async Task<SpResult> CreateFromGuideAsync(CreateAccountPayableFromGuideRequest req)
    {
        using var conn = _db.CreateConnection();
        var lines = JsonSerializer.Serialize(req.Lines.Select(l => new
        {
            guideDetailId = l.GuideDetailId,
            unitPrice = l.UnitPrice
        }));

        return await conn.QueryFirstAsync<SpResult>(
            "finance.SP_CREATE_ACCOUNT_PAYABLE_FROM_GUIDE",
            new { req.StallId, req.SupplierId, req.GuideId, req.DueDate, Lines = lines },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<AccountPayableReadResponse?> GetByIdAsync(int id)
    {
        using var conn = _db.CreateConnection();
        using var multi = await conn.QueryMultipleAsync(
            "finance.SP_READ_ACCOUNT_PAYABLE",
            new { Id = id },
            commandType: CommandType.StoredProcedure);

        var header = await multi.ReadFirstOrDefaultAsync<AccountPayableHeaderResponse>();
        if (header == null) return null;

        var payments = (await multi.ReadAsync<PaymentResponse>()).ToList();
        return new AccountPayableReadResponse { Header = header, Payments = payments };
    }

    public async Task<(List<AccountPayableListResponse> Items, int TotalRecords)> ListAsync(
        int? stallId, int? supplierId, string? accountStatus,
        DateOnly? dateFrom, DateOnly? dateTo, string? search,
        int pageNumber, int pageSize)
    {
        using var conn = _db.CreateConnection();
        using var multi = await conn.QueryMultipleAsync(
            "finance.SP_LIST_ACCOUNT_PAYABLE",
            new
            {
                StallId = stallId,
                SupplierId = supplierId,
                AccountStatus = accountStatus,
                DateFrom = dateFrom.HasValue ? (DateTime?)dateFrom.Value.ToDateTime(TimeOnly.MinValue) : null,
                DateTo = dateTo.HasValue ? (DateTime?)dateTo.Value.ToDateTime(TimeOnly.MinValue) : null,
                Search = search,
                PageNumber = pageNumber,
                PageSize = pageSize
            },
            commandType: CommandType.StoredProcedure);

        var items = (await multi.ReadAsync<AccountPayableListResponse>()).ToList();
        var total = await multi.ReadFirstAsync<int>();
        return (items, total);
    }

    public async Task<List<AccountPayableListResponse>> ListByStallAsync(int stallId, string? accountStatus)
    {
        using var conn = _db.CreateConnection();
        var result = await conn.QueryAsync<AccountPayableListResponse>(
            "finance.SP_LIST_ACCOUNT_PAYABLE_BY_STALL",
            new { StallId = stallId, AccountStatus = accountStatus },
            commandType: CommandType.StoredProcedure);
        return result.ToList();
    }

    public async Task<SpResult> CreatePaymentAsync(CreatePaymentRequest req)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>(
            "finance.SP_CREATE_PAYMENT",
            new
            {
                req.AccountPayableId, req.Amount, req.PaymentMethod,
                req.OperationNumber, req.Observations, req.UserId
            },
            commandType: CommandType.StoredProcedure);
    }
}

// ──────────────────────────────────────────────────────────
// REPORT
// ──────────────────────────────────────────────────────────

public interface IReportRepository
{
    Task<AccountStatementResponse> GetAccountStatementAsync(int stallId, DateOnly? dateFrom, DateOnly? dateTo);
}

public class ReportRepository : IReportRepository
{
    private readonly DbConnectionFactory _db;
    public ReportRepository(DbConnectionFactory db) => _db = db;

    public async Task<AccountStatementResponse> GetAccountStatementAsync(int stallId, DateOnly? dateFrom, DateOnly? dateTo)
    {
        using var conn = _db.CreateConnection();
        using var multi = await conn.QueryMultipleAsync(
            "finance.SP_REPORT_ACCOUNT_STATEMENT",
            new { StallId = stallId,
                  DateFrom = dateFrom.HasValue ? (DateTime?)dateFrom.Value.ToDateTime(TimeOnly.MinValue) : null,
                  DateTo = dateTo.HasValue ? (DateTime?)dateTo.Value.ToDateTime(TimeOnly.MinValue) : null },
            commandType: CommandType.StoredProcedure);

        var summary = await multi.ReadFirstOrDefaultAsync<AccountStatementSummary>();
        var details = (await multi.ReadAsync<AccountStatementDetail>()).ToList();
        return new AccountStatementResponse { Summary = summary, Details = details };
    }
}
