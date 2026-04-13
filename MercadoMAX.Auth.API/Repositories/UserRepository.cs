using Dapper;
using MercadoMAX.Auth.API.DTOs;
using MercadoMAX.Auth.API.Models;
using MercadoMAX.Shared.Data;
using MercadoMAX.Shared.DTOs;
using System.Data;

namespace MercadoMAX.Auth.API.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DbConnectionFactory _db;

    public UserRepository(DbConnectionFactory db)
    {
        _db = db;
    }

    public async Task<SpResult> CreateAsync(string firstName, string lastName, string email, string passwordHash, string? phone)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>(
            "auth.SP_CREATE_USER",
            new { FirstName = firstName, LastName = lastName, Email = email, PasswordHash = passwordHash, Phone = phone },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<SpResult> RegisterAsync(string firstName, string lastName, string email, string passwordHash, string? phone)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>(
            "auth.SP_REGISTER_USER",
            new { FirstName = firstName, LastName = lastName, Email = email, PasswordHash = passwordHash, Phone = phone },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<User>(
            "auth.SP_READ_USER",
            new { Id = id },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<(List<UserListRow> Items, int TotalRecords)> ListAsync(string? search, int pageNumber, int pageSize)
    {
        using var conn = _db.CreateConnection();
        using var multi = await conn.QueryMultipleAsync(
            "auth.SP_LIST_USER",
            new { Search = search, PageNumber = pageNumber, PageSize = pageSize },
            commandType: CommandType.StoredProcedure);

        var items = (await multi.ReadAsync<UserListRow>()).ToList();
        var total = await multi.ReadFirstAsync<int>();
        return (items, total);
    }

    public async Task<SpResult> UpdateAsync(int id, string firstName, string lastName, string email, string? phone)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>(
            "auth.SP_UPDATE_USER",
            new { Id = id, FirstName = firstName, LastName = lastName, Email = email, Phone = phone },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<SpResult> DeleteAsync(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>(
            "auth.SP_DELETE_USER",
            new { Id = id },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<SpResult> AssignRoleAsync(int userId, int roleId)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>(
            "auth.SP_ASSIGN_ROLE",
            new { UserId = userId, RoleId = roleId },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<SpResult> RemoveRoleAsync(int userId, int roleId)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>(
            "auth.SP_REMOVE_ROLE",
            new { UserId = userId, RoleId = roleId },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<SpResult> AssignPermissionAsync(int userId, int permissionId)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>(
            "auth.SP_ASSIGN_PERMISSION_USER",
            new { UserId = userId, PermissionId = permissionId },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<SpResult> RemovePermissionAsync(int userId, int permissionId)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>(
            "auth.SP_REMOVE_PERMISSION_USER",
            new { UserId = userId, PermissionId = permissionId },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<List<int>> GetDirectPermissionIdsAsync(int userId)
    {
        using var conn = _db.CreateConnection();
        var ids = await conn.QueryAsync<int>(
            "auth.SP_LIST_PERMISSION_ID_BY_USER",
            new { UserId = userId },
            commandType: CommandType.StoredProcedure);
        return ids.ToList();
    }

    public async Task<SpResult> AssignRolePermissionsAsync(int userId, int roleId)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>(
            "auth.SP_ASSIGN_ROLE_PERMISSIONS_TO_USER",
            new { UserId = userId, RoleId = roleId },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<SpResult> ToggleStatusAsync(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>(
            "auth.SP_TOGGLE_STATUS_USER",
            new { Id = id },
            commandType: CommandType.StoredProcedure);
    }
}
