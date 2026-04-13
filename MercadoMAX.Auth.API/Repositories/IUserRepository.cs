using MercadoMAX.Auth.API.DTOs;
using MercadoMAX.Auth.API.Models;
using MercadoMAX.Shared.DTOs;

namespace MercadoMAX.Auth.API.Repositories;

public interface IUserRepository
{
    Task<SpResult> CreateAsync(string firstName, string lastName, string email, string passwordHash, string? phone);
    Task<SpResult> RegisterAsync(string firstName, string lastName, string email, string passwordHash, string? phone);
    Task<User?> GetByIdAsync(int id);
    Task<(List<UserListRow> Items, int TotalRecords)> ListAsync(string? search, int pageNumber, int pageSize);
    Task<SpResult> UpdateAsync(int id, string firstName, string lastName, string email, string? phone);
    Task<SpResult> DeleteAsync(int id);
    Task<SpResult> AssignRoleAsync(int userId, int roleId);
    Task<SpResult> RemoveRoleAsync(int userId, int roleId);
    Task<SpResult> AssignPermissionAsync(int userId, int permissionId);
    Task<SpResult> RemovePermissionAsync(int userId, int permissionId);
    Task<List<int>> GetDirectPermissionIdsAsync(int userId);
    Task<SpResult> AssignRolePermissionsAsync(int userId, int roleId);
    Task<SpResult> ToggleStatusAsync(int id);
}
