using MercadoMAX.Auth.API.DTOs;
using MercadoMAX.Shared.DTOs;

namespace MercadoMAX.Auth.API.Repositories;

public interface IRoleRepository
{
    Task<List<RoleResponse>> ListAsync();
    Task<RoleResponse?> GetByIdAsync(int id);
    Task<List<PermissionResponse>> GetPermissionsByRoleAsync(int roleId);
    Task<List<PermissionResponse>> ListAllPermissionsAsync();
    Task<SpResult> CreateAsync(string name, string? description, string permissionIds);
    Task<SpResult> UpdateAsync(int id, string name, string? description, string permissionIds);
    Task<SpResult> DeleteAsync(int id);
    Task<SpResult> CreatePermissionAsync(string module, string action, string? description);
    Task<SpResult> UpdatePermissionAsync(int id, string module, string action, string? description);
    Task<SpResult> DeletePermissionAsync(int id);
    Task<SpResult> ToggleStatusRoleAsync(int id);
    Task<SpResult> ToggleStatusPermissionAsync(int id);
}
