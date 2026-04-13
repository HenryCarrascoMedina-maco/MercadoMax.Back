namespace MercadoMAX.Guias.API.Models;

public class Guide
{
    public int Id { get; set; }
    public string GuideNumber { get; set; } = string.Empty;
    public int SupplierId { get; set; }
    public int? CarrierId { get; set; }
    public int? TruckId { get; set; }
    public DateTime ShipmentDate { get; set; }
    public DateTime? EstimatedArrivalDate { get; set; }
    public string? Observations { get; set; }
    public string GuideStatus { get; set; } = "Pending";
    public decimal TotalTransportCost { get; set; }
    public int CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class GuideDetail
{
    public int Id { get; set; }
    public int GuideId { get; set; }
    public int DestinationStallId { get; set; }
    public int ProductId { get; set; }
    public int? BrandId { get; set; }
    public int? ProductSizeId { get; set; }
    public int LogisticUnitId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TransportUnitPrice { get; set; }
    public decimal TransportSubtotal { get; set; }
    public string? Observations { get; set; }
    public bool Status { get; set; }
}
