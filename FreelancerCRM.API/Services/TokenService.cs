using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FreelancerCRM.API.Data;
using FreelancerCRM.API.DTOs;
using FreelancerCRM.API.Models;
using FreelancerCRM.API.Models.Configurations;
using FreelancerCRM.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FreelancerCRM.API.Services;

public class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<TokenService> _logger;
    private readonly FreelancerCrmDbContext _context;

    public TokenService(
        IOptions<JwtSettings> jwtSettings, 
        ILogger<TokenService> logger,
        FreelancerCrmDbContext context)
    {
        _jwtSettings = jwtSettings.Value;
        _logger = logger;
        _context = context;
    }

    public string GenerateAccessToken(User user)
    {
        try
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while generating access token for user {UserId}", user.Id);
            throw new InvalidOperationException("Could not generate access token", ex);
        }
    }

    public async Task<string> GenerateRefreshTokenAsync(User user)
    {
        try
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            var refreshToken = Convert.ToBase64String(randomBytes);

            var token = new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpiryDate = DateTime.UtcNow.AddDays(7) // 7 günlük refresh token
            };

            _context.RefreshTokens.Add(token);
            await _context.SaveChangesAsync();

            return refreshToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while generating refresh token for user {UserId}", user.Id);
            throw new InvalidOperationException("Could not generate refresh token", ex);
        }
    }

    public bool ValidateAccessToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            return false;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Token validation failed");
            return false;
        }
    }

    public string? GetUserIdFromToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            return null;

        var tokenHandler = new JwtSecurityTokenHandler();
        
        try
        {
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var userIdClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub);
            return userIdClaim?.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while extracting user ID from token");
            return null;
        }
    }

    public async Task<TokenResponse> RefreshTokenAsync(string accessToken, string refreshToken)
    {
        try
        {
            var userId = GetUserIdFromToken(accessToken);
            if (string.IsNullOrEmpty(userId))
                throw new InvalidOperationException("Invalid access token");

            var storedRefreshToken = await _context.RefreshTokens
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Token == refreshToken);

            if (storedRefreshToken == null)
                throw new InvalidOperationException("Invalid refresh token");

            if (!storedRefreshToken.IsActive)
                throw new InvalidOperationException("Refresh token is not active");

            if (storedRefreshToken.UserId != int.Parse(userId))
                throw new InvalidOperationException("Refresh token does not match the user");

            // Yeni token'ları oluştur
            var user = storedRefreshToken.User;
            var newAccessToken = GenerateAccessToken(user);
            var newRefreshToken = await GenerateRefreshTokenAsync(user);

            // Eski refresh token'ı geçersiz kıl
            storedRefreshToken.RevokedDate = DateTime.UtcNow;
            storedRefreshToken.ReplacedByToken = newRefreshToken;
            storedRefreshToken.RevokedReason = "Replaced by new token";
            await _context.SaveChangesAsync();

            return new TokenResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while refreshing token");
            throw;
        }
    }

    public async Task<bool> RevokeRefreshTokenAsync(string refreshToken)
    {
        try
        {
            var storedRefreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(r => r.Token == refreshToken);

            if (storedRefreshToken == null || !storedRefreshToken.IsActive)
                return false;

            storedRefreshToken.RevokedDate = DateTime.UtcNow;
            storedRefreshToken.RevokedReason = "Revoked by user";
            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while revoking refresh token");
            return false;
        }
    }

    public async Task<bool> RevokeAllUserRefreshTokensAsync(int userId)
    {
        try
        {
            var userRefreshTokens = await _context.RefreshTokens
                .Where(r => r.UserId == userId && r.IsActive)
                .ToListAsync();

            foreach (var token in userRefreshTokens)
            {
                token.RevokedDate = DateTime.UtcNow;
                token.RevokedReason = "Revoked all user tokens";
            }

            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while revoking all refresh tokens for user {UserId}", userId);
            return false;
        }
    }
} 