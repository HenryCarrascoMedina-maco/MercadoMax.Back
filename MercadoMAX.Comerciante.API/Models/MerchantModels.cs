namespace MercadoMAX.Comerciante.API.Models;

public class ReceptionConfirmation
{
    public int Id { get; set; }
    public int ReceptionId { get; set; }
    public int StallId { get; set; }
    public DateTime ConfirmationDate { get; set; }
    public string? Observations { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class Inventory
{
    public int Id { get; set; }
    public int StallId { get; set; }
    public int ProductId { get; set; }
    public int LogisticUnitId { get; set; }
    public int CurrentStock { get; set; }
    public int MinimumStock { get; set; }
    public decimal AverageCost { get; set; }
    public bool Status { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class InventoryMovement
{
    public int Id { get; set; }
    public int InventoryId { get; set; }
    public string MovementType { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public string? ReferenceDocument { get; set; }
    public int? ReferenceId { get; set; }
    public DateTime MovementDate { get; set; }
}
