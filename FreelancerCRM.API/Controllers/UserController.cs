using FreelancerCRM.API.DTOs;
using FreelancerCRM.API.DTOs.Extensions;
using FreelancerCRM.API.Models.Configurations;
using FreelancerCRM.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace FreelancerCRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;
    private readonly JwtSettings _jwtSettings;

    public UserController(
        IUserService userService, 
        ITokenService tokenService,
        IOptions<JwtSettings> jwtSettings)
    {
        _userService = userService;
        _tokenService = tokenService;
        _jwtSettings = jwtSettings.Value;
    }

    /// <summary>
    /// Belirtilen ID'ye sahip kullanıcıyı getirir
    /// </summary>
    [HttpGet("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _userService.GetByIdAsync(id);
        if (result.IsSuccess) return Ok(result.Data);
        return NotFound(result.ErrorMessage);
    }

    /// <summary>
    /// Tüm kullanıcıları getirir
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(IEnumerable<UserSummaryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _userService.GetAllAsync();
        if (result.IsSuccess) return Ok(result.Data);
        return StatusCode(500, result.ErrorMessage);
    }

    /// <summary>
    /// Yeni bir kullanıcı oluşturur
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] UserCreateDto createDto)
    {
        var result = await _userService.CreateAsync(createDto);
        if (result.IsSuccess) 
            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
        
        return BadRequest(result.ValidationErrors);
    }

    /// <summary>
    /// Mevcut bir kullanıcıyı günceller
    /// </summary>
    [HttpPut("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UserUpdateDto updateDto)
    {
        var result = await _userService.UpdateAsync(id, updateDto);
        if (result.IsSuccess) return Ok(result.Data);
        if (result.ValidationErrors.Any()) return BadRequest(result.ValidationErrors);
        return NotFound(result.ErrorMessage);
    }

    /// <summary>
    /// Belirtilen ID'ye sahip kullanıcıyı siler
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _userService.DeleteAsync(id);
        if (result.IsSuccess) return NoContent();
        return NotFound(result.ErrorMessage);
    }

    /// <summary>
    /// Yeni bir kullanıcı kaydı oluşturur ve otomatik giriş yapar
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] UserCreateDto createDto)
    {
        var result = await _userService.RegisterUserAsync(createDto);
        if (!result.IsSuccess)
        {
            var errorResponse = new
            {
                Message = "Validation failed",
                Errors = result.ValidationErrors,
                ErrorMessage = result.ErrorMessage
            };
            return BadRequest(errorResponse);
        }

        // Otomatik login
        var user = result.Data;
        var accessToken = _tokenService.GenerateAccessToken(user!.ToEntity());
        var refreshToken = await _tokenService.GenerateRefreshTokenAsync(user!.ToEntity());

        return Ok(new AuthResponseDto 
        { 
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
            User = user!
        });
    }

    /// <summary>
    /// Kullanıcı girişi yapar ve JWT token döndürür
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] UserLoginDto loginDto)
    {
        var result = await _userService.AuthenticateAsync(loginDto);
        if (!result.IsSuccess) return Unauthorized(result.ErrorMessage);

        var user = result.Data;
        var accessToken = _tokenService.GenerateAccessToken(user!.ToEntity());
        var refreshToken = await _tokenService.GenerateRefreshTokenAsync(user!.ToEntity());

        return Ok(new AuthResponseDto 
        { 
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
            User = user!
        });
    }

    /// <summary>
    /// Kullanıcı şifresini değiştirir
    /// </summary>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangePassword([FromBody] UserChangePasswordDto changePasswordDto)
    {
        var result = await _userService.ChangePasswordAsync(changePasswordDto);
        if (result.IsSuccess) return Ok();
        return BadRequest(result.ValidationErrors);
    }

    /// <summary>
    /// Aktif kullanıcıları getirir
    /// </summary>
    [HttpGet("active")]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<UserSummaryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActiveUsers()
    {
        var result = await _userService.GetActiveUsersAsync();
        if (result.IsSuccess) return Ok(result.Data);
        return StatusCode(500, result.ErrorMessage);
    }

    /// <summary>
    /// Kullanıcıyı ilişkili verilerle birlikte getirir
    /// </summary>
    [HttpGet("with-relations/{id}")]
    [Authorize]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserWithRelations(int id)
    {
        var result = await _userService.GetUserWithRelationsAsync(id);
        if (result.IsSuccess) return Ok(result.Data);
        return NotFound(result.ErrorMessage);
    }

    /// <summary>
    /// Kullanıcıyı deaktif eder
    /// </summary>
    [HttpPost("{id}/deactivate")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateUser(int id)
    {
        var result = await _userService.DeactivateUserAsync(id);
        if (result.IsSuccess) return Ok();
        return NotFound(result.ErrorMessage);
    }

    /// <summary>
    /// Kullanıcıyı aktif eder
    /// </summary>
    [HttpPost("{id}/activate")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActivateUser(int id)
    {
        var result = await _userService.ActivateUserAsync(id);
        if (result.IsSuccess) return Ok();
        return NotFound(result.ErrorMessage);
    }

    /// <summary>
    /// Kullanıcı adının kullanılabilir olup olmadığını kontrol eder
    /// </summary>
    [HttpGet("check-username")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> IsUsernameAvailable([FromQuery] string username)
    {
        var result = await _userService.IsUsernameAvailableAsync(username);
        return Ok(result.Data);
    }

    /// <summary>
    /// Email adresinin kullanılabilir olup olmadığını kontrol eder
    /// </summary>
    [HttpGet("check-email")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> IsEmailAvailable([FromQuery] string email)
    {
        var result = await _userService.IsEmailAvailableAsync(email);
        return Ok(result.Data);
    }

    /// <summary>
    /// Access token'ı yenilemek için refresh token kullanır
    /// </summary>
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            var result = await _tokenService.RefreshTokenAsync(request.AccessToken, request.RefreshToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (Exception)
        {
            return BadRequest("An error occurred while refreshing the token");
        }
    }

    /// <summary>
    /// Refresh token'ı geçersiz kılar
    /// </summary>
    [HttpPost("revoke-token")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RevokeToken([FromBody] string refreshToken)
    {
        var result = await _tokenService.RevokeRefreshTokenAsync(refreshToken);
        if (result) return Ok();
        return BadRequest("Invalid refresh token");
    }

    /// <summary>
    /// Kullanıcının tüm refresh token'larını geçersiz kılar
    /// </summary>
    [HttpPost("revoke-all-tokens")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RevokeAllTokens()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var result = await _tokenService.RevokeAllUserRefreshTokensAsync(userId);
        if (result) return Ok();
        return BadRequest("Could not revoke tokens");
    }
} 