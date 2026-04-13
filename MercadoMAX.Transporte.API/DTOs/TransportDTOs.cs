using System.ComponentModel.DataAnnotations;

namespace MercadoMAX.Transporte.API.DTOs;

// ── Carrier ─────────────────────────────────────────────
public class CreateCarrierRequest
{
    [Required, MaxLength(100)] public string FirstName { get; set; } = string.Empty;
    [Required, MaxLength(100)] public string LastName { get; set; } = string.Empty;
    [Required, MaxLength(20)] public string DocumentId { get; set; } = string.Empty;
    [MaxLength(20)] public string? Phone { get; set; }
    [MaxLength(30)] public string? LicenseNumber { get; set; }
    public int? UserId { get; set; }
}

public class UpdateCarrierRequest
{
    [Required] public int Id { get; set; }
    [Required, MaxLength(100)] public string FirstName { get; set; } = string.Empty;
    [Required, MaxLength(100)] public string LastName { get; set; } = string.Empty;
    [Required, MaxLength(20)] public string DocumentId { get; set; } = string.Empty;
    [MaxLength(20)] public string? Phone { get; set; }
    [MaxLength(30)] public string? LicenseNumber { get; set; }
    public int? UserId { get; set; }
    public bool Status { get; set; }
}

public class CarrierResponse
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string DocumentId { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? LicenseNumber { get; set; }
    public int? UserId { get; set; }
    public bool Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int TruckCount { get; set; }
}

// ── Truck ───────────────────────────────────────────────
public class CreateTruckRequest
{
    [Required, MaxLength(15)] public string LicensePlate { get; set; } = string.Empty;
    [Required] public int CarrierId { get; set; }
    [MaxLength(50)] public string? Capacity { get; set; }
    [MaxLength(50)] public string? Brand { get; set; }
    [MaxLength(50)] public string? Model { get; set; }
}

public class UpdateTruckRequest
{
    [Required] public int Id { get; set; }
    [Required, MaxLength(15)] public string LicensePlate { get; set; } = string.Empty;
    [Required] public int CarrierId { get; set; }
    [MaxLength(50)] public string? Capacity { get; set; }
    [MaxLength(50)] public string? Brand { get; set; }
    [MaxLength(50)] public string? Model { get; set; }
    public bool Status { get; set; }
}

public class TruckResponse
{
    public int Id { get; set; }
    public string LicensePlate { get; set; } = string.Empty;
    public int CarrierId { get; set; }
    public string CarrierName { get; set; } = string.Empty;
    public string? Capacity { get; set; }
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public bool Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

// ── TransportRate ───────────────────────────────────────
public class CreateTransportRateRequest
{
    [Required] public int CarrierId { get; set; }
    [Required] public int LogisticUnitId { get; set; }
    [Required, MaxLength(150)] public string RouteOrigin { get; set; } = string.Empty;
    [Required, MaxLength(150)] public string RouteDestination { get; set; } = string.Empty;
    [Required] public decimal UnitPrice { get; set; }
    [Required] public DateTime EffectiveDate { get; set; }
}

public class UpdateTransportRateRequest
{
    [Required] public int Id { get; set; }
    [Required] public int CarrierId { get; set; }
    [Required] public int LogisticUnitId { get; set; }
    [Required, MaxLength(150)] public string RouteOrigin { get; set; } = string.Empty;
    [Required, MaxLength(150)] public string RouteDestination { get; set; } = string.Empty;
    [Required] public decimal UnitPrice { get; set; }
    [Required] public DateTime EffectiveDate { get; set; }
    public bool Status { get; set; }
}

public class TransportRateResponse
{
    public int Id { get; set; }
    public int CarrierId { get; set; }
    public string CarrierName { get; set; } = string.Empty;
    public int LogisticUnitId { get; set; }
    public string LogisticUnitName { get; set; } = string.Empty;
    public string? LogisticUnitAbbr { get; set; }
    public string RouteOrigin { get; set; } = string.Empty;
    public string RouteDestination { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public DateTime EffectiveDate { get; set; }
    public bool Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

// ── Settlement ──────────────────────────────────────────
public class CreateSettlementRequest
{
    [Required] public int CarrierId { get; set; }
    public int? TruckId { get; set; }
    [Required] public DateTime TripDate { get; set; }
}

public class UpdateSettlementStatusRequest
{
    [Required] public int Id { get; set; }
    [Required, MaxLength(20)] public string SettlementStatus { get; set; } = string.Empty;
}

public class SettlementListResponse
{
    public int Id { get; set; }
    public int CarrierId { get; set; }
    public string CarrierName { get; set; } = string.Empty;
    public int? TruckId { get; set; }
    public string? LicensePlate { get; set; }
    public DateTime TripDate { get; set; }
    public decimal TotalUnits { get; set; }
    public decimal TotalAmount { get; set; }
    public string SettlementStatus { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int DetailCount { get; set; }
}

public class SettlementHeaderResponse
{
    public int Id { get; set; }
    public int CarrierId { get; set; }
    public string CarrierName { get; set; } = string.Empty;
    public string? CarrierDocumentId { get; set; }
    public int? TruckId { get; set; }
    public string? LicensePlate { get; set; }
    public DateTime TripDate { get; set; }
    public decimal TotalUnits { get; set; }
    public decimal TotalAmount { get; set; }
    public string SettlementStatus { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class SettlementDetailResponse
{
    public int Id { get; set; }
    public int SettlementId { get; set; }
    public int GuideId { get; set; }
    public string GuideNumber { get; set; } = string.Empty;
    public int LogisticUnitId { get; set; }
    public string LogisticUnitName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }
}

public class SettlementReadResponse
{
    public SettlementHeaderResponse Header { get; set; } = null!;
    public List<SettlementDetailResponse> Details { get; set; } = [];
}

// ── Settlement Detail Create ────────────────────────────
public class CreateSettlementDetailRequest
{
    [Required] public int SettlementId { get; set; }
    [Required] public int GuideId { get; set; }
    [Required] public int LogisticUnitId { get; set; }
    [Required] public decimal Quantity { get; set; }
    [Required] public decimal UnitPrice { get; set; }
}
