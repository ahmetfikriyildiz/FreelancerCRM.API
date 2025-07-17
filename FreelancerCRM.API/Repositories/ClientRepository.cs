using Microsoft.EntityFrameworkCore;
using FreelancerCRM.API.Data;
using FreelancerCRM.API.Models;
using FreelancerCRM.API.Repositories.Interfaces;

namespace FreelancerCRM.API.Repositories;

public class ClientRepository : GenericRepository<Client>, IClientRepository
{
    public ClientRepository(FreelancerCrmDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Client>> GetClientsByUserIdAsync(int userId)
    {
        return await _dbSet.Where(c => c.UserID == userId).ToListAsync();
    }

    public async Task<IEnumerable<Client>> GetActiveClientsAsync()
    {
        return await _dbSet.Where(c => c.Status == "Active").ToListAsync();
    }

    public async Task<IEnumerable<Client>> GetClientsByStatusAsync(string status)
    {
        return await _dbSet.Where(c => c.Status == status).ToListAsync();
    }

    public async Task<IEnumerable<Client>> GetClientsByPriorityAsync(string priority)
    {
        return await _dbSet.Where(c => c.Priority == priority).ToListAsync();
    }

    public async Task<IEnumerable<Client>> GetClientsByIndustryAsync(string industry)
    {
        return await _dbSet.Where(c => c.Industry == industry).ToListAsync();
    }

    public async Task<IEnumerable<Client>> GetClientsByCityAsync(string city)
    {
        return await _dbSet.Where(c => c.City == city).ToListAsync();
    }

    /* Türkiye'ye özgü metodlar - İleride kullanılabilir
    public async Task<Client?> GetClientByTaxNumberAsync(string taxNumber)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.TaxNumber == taxNumber);
    }
    */

    public async Task<Client?> GetClientWithProjectsAsync(int clientId)
    {
        return await _dbSet
            .Include(c => c.Projects)
            .FirstOrDefaultAsync(c => c.ClientID == clientId);
    }

    public async Task<Client?> GetClientWithInvoicesAsync(int clientId)
    {
        return await _dbSet
            .Include(c => c.Invoices)
            .FirstOrDefaultAsync(c => c.ClientID == clientId);
    }

    public async Task<Client?> GetClientWithAllRelationsAsync(int clientId)
    {
        return await _dbSet
            .Include(c => c.Projects)
            .Include(c => c.Invoices)
            .FirstOrDefaultAsync(c => c.ClientID == clientId);
    }

    public async Task<IEnumerable<Client>> SearchClientsAsync(string searchTerm)
    {
        return await _dbSet
            .Where(c => 
                c.CompanyName.Contains(searchTerm) ||
                c.ContactFirstName!.Contains(searchTerm) ||
                c.ContactLastName!.Contains(searchTerm) ||
                c.Email!.Contains(searchTerm) ||
                c.Phone!.Contains(searchTerm) ||
                c.City!.Contains(searchTerm) ||
                c.Country!.Contains(searchTerm) ||
                c.Industry!.Contains(searchTerm))
            .ToListAsync();
    }
} 