using FreelancerCRM.API.Models;

namespace FreelancerCRM.API.Repositories.Interfaces;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
    Task<bool> IsUsernameExistsAsync(string username);
    Task<bool> IsEmailExistsAsync(string email);
    Task<IEnumerable<User>> GetActiveUsersAsync();
    Task<User?> GetUserWithClientsAsync(int userId);
    Task<User?> GetUserWithProjectsAsync(int userId);
    Task<User?> GetUserWithAllRelationsAsync(int userId);

    /* Türkiye'ye özgü metodlar - İleride kullanılabilir
    Task<User?> GetByTCKNAsync(string tckn);
    Task<IEnumerable<User>> GetUsersByTaxOfficeAsync(string taxOffice);
    */
} 