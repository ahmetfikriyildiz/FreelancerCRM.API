using FreelancerCRM.API.DTOs;
using FreelancerCRM.API.Models;

namespace FreelancerCRM.API.Services.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    Task<string> GenerateRefreshTokenAsync(User user);
    bool ValidateAccessToken(string token);
    string? GetUserIdFromToken(string token);
    Task<TokenResponse> RefreshTokenAsync(string accessToken, string refreshToken);
    Task<bool> RevokeRefreshTokenAsync(string refreshToken);
    Task<bool> RevokeAllUserRefreshTokensAsync(int userId);
} 