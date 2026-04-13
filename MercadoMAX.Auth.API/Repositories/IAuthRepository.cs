using MercadoMAX.Auth.API.Models;
using MercadoMAX.Shared.DTOs;

namespace MercadoMAX.Auth.API.Repositories;

public interface IAuthRepository
{
    Task<User?> GetUserByEmailAsync(string email);
    Task UpdateRefreshTokenAsync(int userId, string refreshToken, DateTime expiry);
    Task<List<string>> GetUserRolesAsync(int userId);
    Task<List<string>> GetUserPermissionsAsync(int userId);
    Task<SpResult> CreatePasswordResetTokenAsync(string email, string token, DateTime expiresAt);
    Task<SpResult> ValidatePasswordResetTokenAsync(string token);
    Task<SpResult> ResetPasswordAsync(string token, string newPasswordHash);
    Task IncrementTokenVersionAsync(int userId);
    Task<int> GetTokenVersionAsync(int userId);
}
