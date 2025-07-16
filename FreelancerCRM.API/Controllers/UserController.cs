using FreelancerCRM.API.DTOs;
using FreelancerCRM.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerCRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Belirtilen ID'ye sahip kullanıcıyı getirir
    /// </summary>
    [HttpGet("{id}")]
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
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _userService.DeleteAsync(id);
        if (result.IsSuccess) return NoContent();
        return NotFound(result.ErrorMessage);
    }

    /// <summary>
    /// Yeni bir kullanıcı kaydı oluşturur
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] UserCreateDto createDto)
    {
        var result = await _userService.RegisterUserAsync(createDto);
        if (result.IsSuccess) return Ok(result.Data);
        return BadRequest(result.ValidationErrors);
    }

    /// <summary>
    /// Kullanıcı girişi yapar
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] UserLoginDto loginDto)
    {
        var result = await _userService.AuthenticateAsync(loginDto);
        if (result.IsSuccess) return Ok(result.Data);
        return Unauthorized(result.ErrorMessage);
    }

    /// <summary>
    /// Kullanıcı şifresini değiştirir
    /// </summary>
    [HttpPost("change-password")]
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
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> IsEmailAvailable([FromQuery] string email)
    {
        var result = await _userService.IsEmailAvailableAsync(email);
        return Ok(result.Data);
    }
} 