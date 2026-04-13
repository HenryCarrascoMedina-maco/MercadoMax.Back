namespace MercadoMAX.Recepcion.API.DTOs;

// ── Reception ───────────────────────────────────────────
public class CreateReceptionRequest
{
    public int GuideId { get; set; }
    public int StallId { get; set; }
    public DateTime ReceptionDate { get; set; }
    public string? Observations { get; set; }
    public int UserId { get; set; }
}

public class UpdateReceptionStatusRequest
{
    public int Id { get; set; }
    public string ReceptionStatus { get; set; } = string.Empty;
}

public class ReceptionHeaderResponse
{
    public int Id { get; set; }
    public int GuideId { get; set; }
    public string? GuideNumber { get; set; }
    public int StallId { get; set; }
    public string? StallCode { get; set; }
    public DateTime ReceptionDate { get; set; }
    public string ReceptionStatus { get; set; } = string.Empty;
    public string? Observations { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ReceptionReadResponse
{
    public ReceptionHeaderResponse Header { get; set; } = null!;
    public List<ReceptionDetailResponse> Details { get; set; } = [];
}

public class ReceptionListResponse
{
    public int Id { get; set; }
    public int GuideId { get; set; }
    public string? GuideNumber { get; set; }
    public int StallId { get; set; }
    public string? StallCode { get; set; }
    public DateTime ReceptionDate { get; set; }
    public string ReceptionStatus { get; set; } = string.Empty;
    public string? Observations { get; set; }
    public int DetailCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

// ── ReceptionDetail ─────────────────────────────────────
public class CreateReceptionDetailRequest
{
    public int ReceptionId { get; set; }
    public int ProductId { get; set; }
    public int LogisticUnitId { get; set; }
    public int ExpectedQuantity { get; set; }
    public int ReceivedQuantity { get; set; }
    public string? Condition { get; set; }
}

public class UpdateReceptionDetailRequest
{
    public int Id { get; set; }
    public int ReceivedQuantity { get; set; }
    public string? Condition { get; set; }
    public int ExpectedQuantity { get; set; }
    public int ProductId { get; set; }
}

public class ReceptionDetailResponse
{
    public int Id { get; set; }
    public int ReceptionId { get; set; }
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public int LogisticUnitId { get; set; }
    public string? LogisticUnitName { get; set; }
    public int ExpectedQuantity { get; set; }
    public int ReceivedQuantity { get; set; }
    public string? Condition { get; set; }
    public DateTime CreatedAt { get; set; }
}

// ── Shortage ────────────────────────────────────────────
public class CreateShortageRequest
{
    public int ReceptionDetailId { get; set; }
    public int ShortageQuantity { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Evidence { get; set; }
    public int UserId { get; set; }
}

public class UpdateShortageStatusRequest
{
    public int Id { get; set; }
    public string ClaimStatus { get; set; } = string.Empty;
}

public class ShortageResponse
{
    public int Id { get; set; }
    public int ReceptionDetailId { get; set; }
    public int ShortageQuantity { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string ClaimStatus { get; set; } = string.Empty;
    public string? Evidence { get; set; }
    public string? ProductName { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ShortageListResponse
{
    public int Id { get; set; }
    public int ReceptionDetailId { get; set; }
    public int ShortageQuantity { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string ClaimStatus { get; set; } = string.Empty;
    public string? Evidence { get; set; }
    public string? ProductName { get; set; }
    public int ReceptionId { get; set; }
    public DateTime CreatedAt { get; set; }
}
