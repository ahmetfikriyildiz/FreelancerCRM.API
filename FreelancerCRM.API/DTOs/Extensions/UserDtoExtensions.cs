using FreelancerCRM.API.Models;

namespace FreelancerCRM.API.DTOs.Extensions;

public static class UserDtoExtensions
{
    public static User ToEntity(this UserResponseDto dto)
    {
        return new User
        {
            Id = dto.Id,
            Username = dto.Username,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Role = dto.Role,
            IsActive = dto.IsActive,
            Phone = dto.Phone,
            ProfilePicture = dto.ProfilePicture,
            Timezone = dto.Timezone
        };
    }
} 