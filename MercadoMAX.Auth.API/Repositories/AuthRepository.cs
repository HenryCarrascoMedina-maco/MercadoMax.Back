using Dapper;
using MercadoMAX.Auth.API.Models;
using MercadoMAX.Shared.Data;
using MercadoMAX.Shared.DTOs;
using System.Data;

namespace MercadoMAX.Auth.API.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly DbConnectionFactory _db;

    public AuthRepository(DbConnectionFactory db)
    {
        _db = db;
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<User>(
            "auth.SP_LOGIN_USER",
            new { Email = email },
            commandType: CommandType.StoredProcedure);
    }

    public async Task UpdateRefreshTokenAsync(int userId, string refreshToken, DateTime expiry)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync(
            "auth.SP_UPDATE_USER_REFRESH_TOKEN",
            new { Id = userId, RefreshToken = refreshToken, RefreshTokenExpiry = expiry },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<List<string>> GetUserRolesAsync(int userId)
    {
        using var conn = _db.CreateConnection();
        var roles = await conn.QueryAsync<string>(
            "auth.SP_LIST_ROLE_BY_USER",
            new { UserId = userId },
            commandType: CommandType.StoredProcedure);
        return roles.ToList();
    }

    public async Task<List<string>> GetUserPermissionsAsync(int userId)
    {
        using var conn = _db.CreateConnection();
        var permissions = await conn.QueryAsync<string>(
            "auth.SP_LIST_PERMISSION_BY_USER",
            new { UserId = userId },
            commandType: CommandType.StoredProcedure);
        return permissions.ToList();
    }

    public async Task<SpResult> CreatePasswordResetTokenAsync(string email, string token, DateTime expiresAt)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>(
            "auth.SP_CREATE_PASSWORD_RESET_TOKEN",
            new { Email = email, Token = token, ExpiresAt = expiresAt },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<SpResult> ValidatePasswordResetTokenAsync(string token)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>(
            "auth.SP_VALIDATE_PASSWORD_RESET_TOKEN",
            new { Token = token },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<SpResult> ResetPasswordAsync(string token, string newPasswordHash)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>(
            "auth.SP_RESET_PASSWORD",
            new { Token = token, NewPasswordHash = newPasswordHash },
            commandType: CommandType.StoredProcedure);
    }

    public async Task IncrementTokenVersionAsync(int userId)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync(
            "UPDATE auth.[User] SET TokenVersion = TokenVersion + 1, RefreshToken = NULL, RefreshTokenExpiry = NULL WHERE Id = @Id",
            new { Id = userId });
    }

    public async Task<int> GetTokenVersionAsync(int userId)
    {
        using var conn = _db.CreateConnection();
        return await conn.ExecuteScalarAsync<int>(
            "SELECT TokenVersion FROM auth.[User] WHERE Id = @Id",
            new { Id = userId });
    }
}
