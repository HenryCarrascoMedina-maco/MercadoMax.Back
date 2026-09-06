using System.ComponentModel.DataAnnotations;

namespace MercadoMAX.Finanzas.API.DTOs;

// ──────────────────────────────────────────────────────────
// SALE
// ──────────────────────────────────────────────────────────

public class CreateSaleRequest
{
    public int StallId { get; set; }
    public string? CustomerName { get; set; }
    public string PaymentType { get; set; } = "Cash";   // Cash | Credit
    public int UserId { get; set; }
}

public class CreateSaleDetailRequest
{
    public int SaleId { get; set; }
    public int ProductId { get; set; }
    public int? BrandId { get; set; }
    public int? ProductSizeId { get; set; }
    public int LogisticUnitId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class VoidSaleRequest
{
    public int Id { get; set; }
}

public class SaleListResponse
{
    public int Id { get; set; }
    public int StallId { get; set; }
    public string StallNumber { get; set; } = string.Empty;
    public string? CustomerName { get; set; }
    public DateTime SaleDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string PaymentType { get; set; } = string.Empty;
    public string SaleStatus { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int DetailCount { get; set; }
}

public class SaleHeaderResponse
{
    public int Id { get; set; }
    public int StallId { get; set; }
    public string StallNumber { get; set; } = string.Empty;
    public string PavilionName { get; set; } = string.Empty;
    public string? CustomerName { get; set; }
    public DateTime SaleDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string PaymentType { get; set; } = string.Empty;
    public string SaleStatus { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class SaleDetailResponse
{
    public int Id { get; set; }
    public int SaleId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int? BrandId { get; set; }
    public string? BrandName { get; set; }
    public int? ProductSizeId { get; set; }
    public string? ProductSizeName { get; set; }
    public int LogisticUnitId { get; set; }
    public string LogisticUnitName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }
}

public class SaleReadResponse
{
    public SaleHeaderResponse Header { get; set; } = null!;
    public List<SaleDetailResponse> Details { get; set; } = [];
}

// ──────────────────────────────────────────────────────────
// ACCOUNT PAYABLE
// ──────────────────────────────────────────────────────────

public class CreateAccountPayableRequest
{
    public int StallId { get; set; }
    public int SupplierId { get; set; }
    public int GuideId { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime? DueDate { get; set; }
}

/// <summary>
/// Linea que la guia trae a un puesto, lista para ponerle precio. Cantidad,
/// producto y marca vienen de la guia: aqui solo se edita el precio unitario.
/// </summary>
public class GuideLineForPayableResponse
{
    public int GuideDetailId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int? BrandId { get; set; }
    public string? BrandName { get; set; }
    public int? ProductSizeId { get; set; }
    public string? ProductSizeName { get; set; }
    public int LogisticUnitId { get; set; }
    public string LogisticUnitName { get; set; } = string.Empty;
    public string? LogisticUnitAbbr { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }
}

public class PayableLinePriceRequest
{
    [Required] public int GuideDetailId { get; set; }
    public decimal UnitPrice { get; set; }
}

/// <summary>
/// Alta de una cuenta por pagar a partir de las lineas de la guia.
///
/// No lleva importe: el total lo calcula el SP como SUM(cantidad x precio) sobre
/// las lineas de esa guia y ese puesto, para que no pueda guardarse una cuenta
/// cuyo importe no cuadre con su detalle.
/// </summary>
public class CreateAccountPayableFromGuideRequest
{
    [Required] public int StallId { get; set; }
    [Required] public int SupplierId { get; set; }
    [Required] public int GuideId { get; set; }
    public DateTime? DueDate { get; set; }
    [Required] public List<PayableLinePriceRequest> Lines { get; set; } = [];
}

public class AccountPayableListResponse
{
    public int Id { get; set; }
    public int StallId { get; set; }
    public string StallNumber { get; set; } = string.Empty;
    public int SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public int GuideId { get; set; }
    public string GuideNumber { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal Balance { get; set; }
    public DateTime? DueDate { get; set; }
    public string AccountStatus { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class AccountPayableHeaderResponse
{
    public int Id { get; set; }
    public int StallId { get; set; }
    public string StallNumber { get; set; } = string.Empty;
    public int SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public int GuideId { get; set; }
    public string GuideNumber { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal Balance { get; set; }
    public DateTime? DueDate { get; set; }
    public string AccountStatus { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class PaymentResponse
{
    public int Id { get; set; }
    public int AccountPayableId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string? OperationNumber { get; set; }
    public DateTime PaymentDate { get; set; }
    public string? Observations { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
}

public class AccountPayableReadResponse
{
    public AccountPayableHeaderResponse Header { get; set; } = null!;
    public List<PaymentResponse> Payments { get; set; } = [];
}

public class CreatePaymentRequest
{
    public int AccountPayableId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;   // Cash | Transfer | Check
    public string? OperationNumber { get; set; }
    public string? Observations { get; set; }
    public int UserId { get; set; }
}

// ──────────────────────────────────────────────────────────
// REPORT
// ──────────────────────────────────────────────────────────

public class AccountStatementSummary
{
    public string StallNumber { get; set; } = string.Empty;
    public string PavilionName { get; set; } = string.Empty;
    public string OwnerName { get; set; } = string.Empty;
    public decimal TotalDebt { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal TotalBalance { get; set; }
    public int AccountCount { get; set; }
}

public class AccountStatementDetail
{
    public int Id { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public string GuideNumber { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal Balance { get; set; }
    public DateTime? DueDate { get; set; }
    public string AccountStatus { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class AccountStatementResponse
{
    public AccountStatementSummary? Summary { get; set; }
    public List<AccountStatementDetail> Details { get; set; } = [];
}
