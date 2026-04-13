namespace MercadoMAX.Transporte.API.Models;

public class Carrier
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
}

public class Truck
{
    public int Id { get; set; }
    public string LicensePlate { get; set; } = string.Empty;
    public int CarrierId { get; set; }
    public string? Capacity { get; set; }
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public bool Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class TransportRate
{
    public int Id { get; set; }
    public int CarrierId { get; set; }
    public int LogisticUnitId { get; set; }
    public string RouteOrigin { get; set; } = string.Empty;
    public string RouteDestination { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public DateTime EffectiveDate { get; set; }
    public bool Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class TransportSettlement
{
    public int Id { get; set; }
    public int CarrierId { get; set; }
    public int? TruckId { get; set; }
    public DateTime TripDate { get; set; }
    public decimal TotalUnits { get; set; }
    public decimal TotalAmount { get; set; }
    public string SettlementStatus { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class SettlementDetail
{
    public int Id { get; set; }
    public int SettlementId { get; set; }
    public int GuideId { get; set; }
    public int LogisticUnitId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }
}
