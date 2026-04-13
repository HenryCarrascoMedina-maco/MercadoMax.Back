namespace MercadoMAX.Recepcion.API.Models;

public class Reception
{
    public int Id { get; set; }
    public int GuideId { get; set; }
    public int StallId { get; set; }
    public DateTime ReceptionDate { get; set; }
    public string ReceptionStatus { get; set; } = string.Empty;
    public string? Observations { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ReceptionDetail
{
    public int Id { get; set; }
    public int ReceptionId { get; set; }
    public int ProductId { get; set; }
    public int LogisticUnitId { get; set; }
    public int ExpectedQuantity { get; set; }
    public int ReceivedQuantity { get; set; }
    public string? Condition { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class Shortage
{
    public int Id { get; set; }
    public int ReceptionDetailId { get; set; }
    public int ShortageQuantity { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string ClaimStatus { get; set; } = string.Empty;
    public string? Evidence { get; set; }
    public DateTime CreatedAt { get; set; }
}
