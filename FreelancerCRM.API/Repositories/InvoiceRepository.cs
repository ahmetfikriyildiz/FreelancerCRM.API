using Microsoft.EntityFrameworkCore;
using FreelancerCRM.API.Data;
using FreelancerCRM.API.Models;
using FreelancerCRM.API.Repositories.Interfaces;

namespace FreelancerCRM.API.Repositories;

public class InvoiceRepository : GenericRepository<Invoice>, IInvoiceRepository
{
    public InvoiceRepository(FreelancerCrmDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Invoice>> GetInvoicesByUserIdAsync(int userId)
    {
        return await _dbSet.Where(i => i.UserID == userId).ToListAsync();
    }

    public async Task<IEnumerable<Invoice>> GetInvoicesByClientIdAsync(int clientId)
    {
        return await _dbSet.Where(i => i.ClientID == clientId).ToListAsync();
    }

    public async Task<IEnumerable<Invoice>> GetInvoicesByProjectIdAsync(int projectId)
    {
        return await _dbSet.Where(i => i.ProjectID == projectId).ToListAsync();
    }

    public async Task<IEnumerable<Invoice>> GetInvoicesByStatusAsync(string status)
    {
        return await _dbSet.Where(i => i.Status == status).ToListAsync();
    }

    public async Task<IEnumerable<Invoice>> GetOverdueInvoicesAsync()
    {
        return await _dbSet
            .Where(i => i.DueDate < DateTime.Now && i.Status != "Paid")
            .ToListAsync();
    }

    public async Task<IEnumerable<Invoice>> GetPaidInvoicesAsync()
    {
        return await _dbSet.Where(i => i.Status == "Paid").ToListAsync();
    }

    public async Task<IEnumerable<Invoice>> GetInvoicesByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Where(i => i.IssueDate >= startDate && i.IssueDate <= endDate)
            .ToListAsync();
    }

    public async Task<Invoice?> GetInvoiceByNumberAsync(string invoiceNumber)
    {
        return await _dbSet.FirstOrDefaultAsync(i => i.InvoiceNumber == invoiceNumber);
    }

    public async Task<Invoice?> GetInvoiceWithItemsAsync(int invoiceId)
    {
        return await _dbSet
            .Include(i => i.InvoiceItems)
            .FirstOrDefaultAsync(i => i.InvoiceID == invoiceId);
    }

    public async Task<decimal> GetTotalAmountByUserAsync(int userId)
    {
        return await _dbSet
            .Where(i => i.UserID == userId)
            .SumAsync(i => i.TotalAmount);
    }

    public async Task<decimal> GetTotalAmountByClientAsync(int clientId)
    {
        return await _dbSet
            .Where(i => i.ClientID == clientId)
            .SumAsync(i => i.TotalAmount);
    }

    public async Task<decimal> GetTotalPaidAmountByUserAsync(int userId)
    {
        return await _dbSet
            .Where(i => i.UserID == userId && i.Status == "Paid")
            .SumAsync(i => i.TotalAmount);
    }

    public async Task<decimal> GetTotalOutstandingAmountByUserAsync(int userId)
    {
        return await _dbSet
            .Where(i => i.UserID == userId && i.Status != "Paid")
            .SumAsync(i => i.TotalAmount);
    }

    public async Task<string> GenerateInvoiceNumberAsync()
    {
        var year = DateTime.Now.Year;
        var lastInvoice = await _dbSet
            .Where(i => i.InvoiceNumber!.StartsWith($"INV-{year}-"))
            .OrderByDescending(i => i.InvoiceNumber)
            .FirstOrDefaultAsync();

        if (lastInvoice == null)
        {
            return $"INV-{year}-0001";
        }

        var lastNumber = lastInvoice.InvoiceNumber!.Split('-').LastOrDefault();
        if (int.TryParse(lastNumber, out int number))
        {
            return $"INV-{year}-{(number + 1):D4}";
        }

        return $"INV-{year}-0001";
    }
} 