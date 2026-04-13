using System.ComponentModel.DataAnnotations;

namespace MercadoMAX.Maestros.API.DTOs;

// ── ProductCategory ─────────────────────────────────────
public class CreateProductCategoryRequest
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    [MaxLength(200)]
    public string? Description { get; set; }
}

public class UpdateProductCategoryRequest
{
    [Required] public int Id { get; set; }
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    [MaxLength(200)]
    public string? Description { get; set; }
    public bool Status { get; set; }
}

public class ProductCategoryResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

// ── Product ─────────────────────────────────────────────
public class CreateProductRequest
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    [Required] public int CategoryId { get; set; }
    [MaxLength(200)]
    public string? Description { get; set; }
}

public class UpdateProductRequest
{
    [Required] public int Id { get; set; }
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    [Required] public int CategoryId { get; set; }
    [MaxLength(200)]
    public string? Description { get; set; }
    public bool Status { get; set; }
}

public class ProductResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

// ── ProductSize ─────────────────────────────────────────
public class CreateProductSizeRequest
{
    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    [Required] public int ProductId { get; set; }
}

public class UpdateProductSizeRequest
{
    [Required] public int Id { get; set; }
    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    [Required] public int ProductId { get; set; }
    public bool Status { get; set; }
}

public class ProductSizeResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public bool Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

// ── Brand ───────────────────────────────────────────────
public class CreateBrandRequest
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    [Required] public int SupplierId { get; set; }
    [Required] public int ProductId { get; set; }
}

public class UpdateBrandRequest
{
    [Required] public int Id { get; set; }
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    [Required] public int SupplierId { get; set; }
    [Required] public int ProductId { get; set; }
    public bool Status { get; set; }
}

public class BrandResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public bool Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

// ── Supplier ────────────────────────────────────────────
public class CreateSupplierRequest
{
    [Required, MaxLength(200)]
    public string BusinessName { get; set; } = string.Empty;
    [Required, MaxLength(20)]
    public string TaxId { get; set; } = string.Empty;
    [MaxLength(20)]
    public string? Phone { get; set; }
    [MaxLength(300)]
    public string? Address { get; set; }
    [MaxLength(100)]
    public string? Province { get; set; }
    [MaxLength(100)]
    public string? Department { get; set; }
    [MaxLength(150)]
    public string? ContactName { get; set; }
    [MaxLength(20)]
    public string? ContactPhone { get; set; }
    public int? UserId { get; set; }
}

public class UpdateSupplierRequest
{
    [Required] public int Id { get; set; }
    [Required, MaxLength(200)]
    public string BusinessName { get; set; } = string.Empty;
    [Required, MaxLength(20)]
    public string TaxId { get; set; } = string.Empty;
    [MaxLength(20)]
    public string? Phone { get; set; }
    [MaxLength(300)]
    public string? Address { get; set; }
    [MaxLength(100)]
    public string? Province { get; set; }
    [MaxLength(100)]
    public string? Department { get; set; }
    [MaxLength(150)]
    public string? ContactName { get; set; }
    [MaxLength(20)]
    public string? ContactPhone { get; set; }
    public int? UserId { get; set; }
    public bool Status { get; set; }
}

public class SupplierResponse
{
    public int Id { get; set; }
    public string BusinessName { get; set; } = string.Empty;
    public string TaxId { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? Province { get; set; }
    public string? Department { get; set; }
    public string? ContactName { get; set; }
    public string? ContactPhone { get; set; }
    public int? UserId { get; set; }
    public bool Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

// ── LogisticUnit ────────────────────────────────────────
public class CreateLogisticUnitRequest
{
    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    [Required, MaxLength(10)]
    public string Abbreviation { get; set; } = string.Empty;
}

public class UpdateLogisticUnitRequest
{
    [Required] public int Id { get; set; }
    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    [Required, MaxLength(10)]
    public string Abbreviation { get; set; } = string.Empty;
    public bool Status { get; set; }
}

public class LogisticUnitResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Abbreviation { get; set; } = string.Empty;
    public bool Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

// ── Pavilion ────────────────────────────────────────────
public class CreatePavilionRequest
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    [Required, MaxLength(50)]
    public string Category { get; set; } = string.Empty;
    [MaxLength(200)]
    public string? Location { get; set; }
}

public class UpdatePavilionRequest
{
    [Required] public int Id { get; set; }
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    [Required, MaxLength(50)]
    public string Category { get; set; } = string.Empty;
    [MaxLength(200)]
    public string? Location { get; set; }
    public bool Status { get; set; }
}

public class PavilionResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? Location { get; set; }
    public bool Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

// ── Stall ───────────────────────────────────────────────
public class CreateStallRequest
{
    [Required, MaxLength(20)]
    public string Number { get; set; } = string.Empty;
    [Required] public int PavilionId { get; set; }
    public int? UserId { get; set; }
}

public class UpdateStallRequest
{
    [Required] public int Id { get; set; }
    [Required, MaxLength(20)]
    public string Number { get; set; } = string.Empty;
    [Required] public int PavilionId { get; set; }
    public int? UserId { get; set; }
    public bool Status { get; set; }
}

public class StallResponse
{
    public int Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public int PavilionId { get; set; }
    public string PavilionName { get; set; } = string.Empty;
    public int? UserId { get; set; }
    public string? OwnerName { get; set; }
    public bool Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
