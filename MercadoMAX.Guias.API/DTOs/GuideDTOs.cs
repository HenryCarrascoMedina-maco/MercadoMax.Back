using System.ComponentModel.DataAnnotations;

namespace MercadoMAX.Guias.API.DTOs;

// ── Guide ───────────────────────────────────────────────
public class CreateGuideRequest
{
    [Required] public int SupplierId { get; set; }
    public int? CarrierId { get; set; }
    public int? TruckId { get; set; }
    [Required] public DateTime ShipmentDate { get; set; }
    public DateTime? EstimatedArrivalDate { get; set; }
    [MaxLength(500)]
    public string? Observations { get; set; }
    [Required] public int CreatedBy { get; set; }
}

public class UpdateGuideStatusRequest
{
    [Required] public int Id { get; set; }
    [Required, MaxLength(20)]
    public string GuideStatus { get; set; } = string.Empty;
}

public class GuideListResponse
{
    public int Id { get; set; }
    public string GuideNumber { get; set; } = string.Empty;
    public int SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public int? CarrierId { get; set; }
    public string? CarrierName { get; set; }
    public int? TruckId { get; set; }
    public string? TruckLicensePlate { get; set; }
    public DateTime ShipmentDate { get; set; }
    public DateTime? EstimatedArrivalDate { get; set; }
    public string GuideStatus { get; set; } = string.Empty;
    public decimal TotalTransportCost { get; set; }
    public DateTime CreatedAt { get; set; }
    public int DetailCount { get; set; }
}

public class GuideHeaderResponse
{
    public int Id { get; set; }
    public string GuideNumber { get; set; } = string.Empty;
    public int SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public string SupplierTaxId { get; set; } = string.Empty;
    public int? CarrierId { get; set; }
    public string? CarrierName { get; set; }
    public string? CarrierDocumentId { get; set; }
    public int? TruckId { get; set; }
    public string? TruckLicensePlate { get; set; }
    public string? TruckBrand { get; set; }
    public string? TruckModel { get; set; }
    public DateTime ShipmentDate { get; set; }
    public DateTime? EstimatedArrivalDate { get; set; }
    public string? Observations { get; set; }
    public string GuideStatus { get; set; } = string.Empty;
    public decimal TotalTransportCost { get; set; }
    public int CreatedBy { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class GuideReadResponse
{
    public GuideHeaderResponse Header { get; set; } = null!;
    public List<GuideDetailResponse> Details { get; set; } = [];
}

// ── GuideDetail ─────────────────────────────────────────
public class CreateGuideDetailRequest
{
    [Required] public int GuideId { get; set; }
    [Required] public int DestinationStallId { get; set; }
    [Required] public int ProductId { get; set; }
    public int? BrandId { get; set; }
    public int? ProductSizeId { get; set; }
    [Required] public int LogisticUnitId { get; set; }
    [Required] public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TransportUnitPrice { get; set; }
    [MaxLength(300)]
    public string? Observations { get; set; }
}

public class UpdateGuideDetailRequest
{
    [Required] public int Id { get; set; }
    [Required] public int DestinationStallId { get; set; }
    [Required] public int ProductId { get; set; }
    public int? BrandId { get; set; }
    public int? ProductSizeId { get; set; }
    [Required] public int LogisticUnitId { get; set; }
    [Required] public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TransportUnitPrice { get; set; }
    [MaxLength(300)]
    public string? Observations { get; set; }
}

public class GuideDetailResponse
{
    public int Id { get; set; }
    public int GuideId { get; set; }
    public int DestinationStallId { get; set; }
    public string StallNumber { get; set; } = string.Empty;
    public string PavilionName { get; set; } = string.Empty;
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? CategoryName { get; set; }
    public int? BrandId { get; set; }
    public string? BrandName { get; set; }
    public int? ProductSizeId { get; set; }
    public string? ProductSizeName { get; set; }
    public int LogisticUnitId { get; set; }
    public string LogisticUnitName { get; set; } = string.Empty;
    public string? LogisticUnitAbbr { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TransportUnitPrice { get; set; }
    public decimal TransportSubtotal { get; set; }
    public string? Observations { get; set; }
    public bool Status { get; set; }
}

// ── Guide SP Result with number ─────────────────────────
public class GuideSpResult
{
    public int Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int Id { get; set; }
    public string GuideNumber { get; set; } = string.Empty;
}
