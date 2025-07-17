namespace FreelancerCRM.API.Exceptions;

public class UserNotFoundException : Exception
{
    public UserNotFoundException(string message = "User not found") : base(message)
    {
    }

    public UserNotFoundException(int userId) : base($"User with ID {userId} not found")
    {
    }
}

public class UserAlreadyExistsException : Exception
{
    public UserAlreadyExistsException(string message = "User already exists") : base(message)
    {
    }

    public UserAlreadyExistsException(string field, string value) : base($"{field} '{value}' already exists")
    {
    }
}

public class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException(string message = "Invalid email or password") : base(message)
    {
    }
}

public class TokenValidationException : Exception
{
    public TokenValidationException(string message = "Token validation failed") : base(message)
    {
    }
}

public class RefreshTokenExpiredException : Exception
{
    public RefreshTokenExpiredException(string message = "Refresh token has expired") : base(message)
    {
    }
}

public class RefreshTokenRevokedException : Exception
{
    public RefreshTokenRevokedException(string message = "Refresh token has been revoked") : base(message)
    {
    }
}

public class ValidationException : Exception
{
    public List<string> Errors { get; }

    public ValidationException(List<string> errors) : base("Validation failed")
    {
        Errors = errors;
    }

    public ValidationException(string message) : base(message)
    {
        Errors = new List<string> { message };
    }
} 