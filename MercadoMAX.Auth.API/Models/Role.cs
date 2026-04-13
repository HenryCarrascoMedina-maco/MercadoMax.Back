namespace MercadoMAX.Auth.API.Models;

public class Role
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
