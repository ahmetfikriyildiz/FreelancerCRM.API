using FreelancerCRM.API.Models;

namespace FreelancerCRM.API.Repositories.Interfaces;

public interface IInvoiceItemRepository : IGenericRepository<InvoiceItem>
{
    Task<IEnumerable<InvoiceItem>> GetItemsByInvoiceIdAsync(int invoiceId);
    Task<decimal> GetTotalAmountByInvoiceAsync(int invoiceId);
    Task<decimal> GetTotalQuantityByInvoiceAsync(int invoiceId);
} 