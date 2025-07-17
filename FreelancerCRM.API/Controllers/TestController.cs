using FreelancerCRM.API.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerCRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    /// <summary>
    /// Test exception'ları tetiklemek için endpoint'ler
    /// </summary>
    
    [HttpGet("not-found")]
    public IActionResult TestNotFound()
    {
        throw new UserNotFoundException(999);
    }

    [HttpGet("bad-request")]
    public IActionResult TestBadRequest()
    {
        throw new ArgumentException("Invalid parameter provided");
    }

    [HttpGet("validation-error")]
    public IActionResult TestValidationError()
    {
        var errors = new List<string>
        {
            "First name is required",
            "Email format is invalid",
            "Password must be at least 8 characters"
        };
        throw new ValidationException(errors);
    }

    [HttpGet("unauthorized")]
    public IActionResult TestUnauthorized()
    {
        throw new UnauthorizedAccessException("Access denied");
    }

    [HttpGet("internal-error")]
    public IActionResult TestInternalError()
    {
        throw new InvalidOperationException("Something went wrong internally");
    }

    [HttpGet("database-error")]
    public IActionResult TestDatabaseError()
    {
        throw new Microsoft.EntityFrameworkCore.DbUpdateException("Database constraint violation");
    }
} 