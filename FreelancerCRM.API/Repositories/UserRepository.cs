using Microsoft.EntityFrameworkCore;
using FreelancerCRM.API.Data;
using FreelancerCRM.API.Models;
using FreelancerCRM.API.Repositories.Interfaces;

namespace FreelancerCRM.API.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(FreelancerCrmDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> IsUsernameExistsAsync(string username)
    {
        return await _dbSet.AnyAsync(u => u.Username == username);
    }

    public async Task<bool> IsEmailExistsAsync(string email)
    {
        return await _dbSet.AnyAsync(u => u.Email == email);
    }

    /* Türkiye'ye özgü metodlar - İleride kullanılabilir
    public async Task<User?> GetByTCKNAsync(string tckn)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.TCKN == tckn);
    }

    public async Task<IEnumerable<User>> GetUsersByTaxOfficeAsync(string taxOffice)
    {
        return await _dbSet.Where(u => u.TaxOffice == taxOffice).ToListAsync();
    }
    */

    public async Task<IEnumerable<User>> GetActiveUsersAsync()
    {
        return await _dbSet.Where(u => u.IsActive).ToListAsync();
    }

    public async Task<User?> GetUserWithClientsAsync(int userId)
    {
        return await _dbSet
            .Include(u => u.Clients)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<User?> GetUserWithProjectsAsync(int userId)
    {
        return await _dbSet
            .Include(u => u.Projects)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<User?> GetUserWithAllRelationsAsync(int userId)
    {
        return await _dbSet
            .Include(u => u.Clients)
            .Include(u => u.Projects)
            .Include(u => u.TimeEntries)
            .Include(u => u.Invoices)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }
} 