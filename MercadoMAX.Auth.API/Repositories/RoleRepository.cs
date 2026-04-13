using Dapper;
using MercadoMAX.Auth.API.DTOs;
using MercadoMAX.Shared.Data;
using MercadoMAX.Shared.DTOs;
using System.Data;

namespace MercadoMAX.Auth.API.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly DbConnectionFactory _db;

    public RoleRepository(DbConnectionFactory db)
    {
        _db = db;
    }

    public async Task<List<RoleResponse>> ListAsync()
    {
        using var conn = _db.CreateConnection();
        var roles = await conn.QueryAsync<RoleResponse>(
            "auth.SP_LIST_ROLE",
            commandType: CommandType.StoredProcedure);
        return roles.ToList();
    }

    public async Task<RoleResponse?> GetByIdAsync(int id)
    {
        using var conn = _db.CreateConnection();
        using var multi = await conn.QueryMultipleAsync(
            "auth.SP_READ_ROLE",
            new { Id = id },
            commandType: CommandType.StoredProcedure);

        var role = await multi.ReadFirstOrDefaultAsync<RoleResponse>();
        return role;
    }

    public async Task<List<PermissionResponse>> GetPermissionsByRoleAsync(int roleId)
    {
        using var conn = _db.CreateConnection();
        var permissions = await conn.QueryAsync<PermissionResponse>(
            "auth.SP_LIST_PERMISSION_BY_ROLE",
            new { RoleId = roleId },
            commandType: CommandType.StoredProcedure);
        return permissions.ToList();
    }

    public async Task<List<PermissionResponse>> ListAllPermissionsAsync()
    {
        using var conn = _db.CreateConnection();
        var permissions = await conn.QueryAsync<PermissionResponse>(
            "auth.SP_LIST_PERMISSION",
            commandType: CommandType.StoredProcedure);
        return permissions.ToList();
    }

    public async Task<SpResult> CreateAsync(string name, string? description, string permissionIds)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>(
            "auth.SP_CREATE_ROLE",
            new { Name = name, Description = description, PermissionIds = permissionIds },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<SpResult> UpdateAsync(int id, string name, string? description, string permissionIds)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>(
            "auth.SP_UPDATE_ROLE",
            new { Id = id, Name = name, Description = description, PermissionIds = permissionIds },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<SpResult> DeleteAsync(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>(
            "auth.SP_DELETE_ROLE",
            new { Id = id },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<SpResult> CreatePermissionAsync(string module, string action, string? description)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>(
            "auth.SP_CREATE_PERMISSION",
            new { Module = module, Action = action, Description = description },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<SpResult> UpdatePermissionAsync(int id, string module, string action, string? description)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>(
            "auth.SP_UPDATE_PERMISSION",
            new { Id = id, Module = module, Action = action, Description = description },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<SpResult> DeletePermissionAsync(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>(
            "auth.SP_DELETE_PERMISSION",
            new { Id = id },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<SpResult> ToggleStatusRoleAsync(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>(
            "auth.SP_TOGGLE_STATUS_ROLE",
            new { Id = id },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<SpResult> ToggleStatusPermissionAsync(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>(
            "auth.SP_TOGGLE_STATUS_PERMISSION",
            new { Id = id },
            commandType: CommandType.StoredProcedure);
    }
}
