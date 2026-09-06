using System.Security.Claims;

namespace MercadoMAX.Shared.CrossCutting.Extensions;

/// <summary>
/// Accesores de claims. MercadoMAX emite el id de usuario en <see cref="ClaimTypes.NameIdentifier"/>.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    public static string? GetUserId(this ClaimsPrincipal? user) =>
        user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    public static string? GetEmail(this ClaimsPrincipal? user) =>
        user?.FindFirst(ClaimTypes.Email)?.Value;

    /// <summary>Permisos emitidos como claim "permission" = "Modulo:Accion".</summary>
    public static IReadOnlyList<string> GetPermissions(this ClaimsPrincipal? user) =>
        user?.FindAll("permission").Select(c => c.Value).Distinct().ToArray()
        ?? Array.Empty<string>();

    public static bool HasPermission(this ClaimsPrincipal? user, string permission) =>
        user?.GetPermissions().Contains(permission, StringComparer.OrdinalIgnoreCase) ?? false;
}
