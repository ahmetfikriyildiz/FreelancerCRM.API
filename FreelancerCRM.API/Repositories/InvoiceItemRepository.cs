using Microsoft.EntityFrameworkCore;
using FreelancerCRM.API.Data;
using FreelancerCRM.API.Models;
using FreelancerCRM.API.Repositories.Interfaces;

namespace FreelancerCRM.API.Repositories;

public class InvoiceItemRepository : GenericRepository<InvoiceItem>, IInvoiceItemRepository
{
    public InvoiceItemRepository(FreelancerCrmDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<InvoiceItem>> GetItemsByInvoiceIdAsync(int invoiceId)
    {
        return await _dbSet.Where(ii => ii.InvoiceID == invoiceId).ToListAsync();
    }

    public async Task<IEnumerable<InvoiceItem>> GetItemsByTypeAsync(string itemType)
    {
        return await _dbSet.Where(ii => ii.ItemType == itemType).ToListAsync();
    }

    public async Task<decimal> GetTotalAmountByInvoiceAsync(int invoiceId)
    {
        return await _dbSet
            .Where(ii => ii.InvoiceID == invoiceId)
            .SumAsync(ii => ii.Amount);
    }

    public async Task<decimal> GetTotalQuantityByInvoiceAsync(int invoiceId)
    {
        return await _dbSet
            .Where(ii => ii.InvoiceID == invoiceId)
            .SumAsync(ii => ii.Quantity);
    }
} 