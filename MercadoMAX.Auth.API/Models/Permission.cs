namespace MercadoMAX.Auth.API.Models;

public class Permission
{
    public int Id { get; set; }
    public string Module { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Status { get; set; }
}
