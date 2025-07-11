using FreelancerCRM.API.Models;

namespace FreelancerCRM.API.Repositories.Interfaces;

public interface IInvoiceRepository : IGenericRepository<Invoice>
{
    Task<IEnumerable<Invoice>> GetInvoicesByUserIdAsync(int userId);
    Task<IEnumerable<Invoice>> GetInvoicesByClientIdAsync(int clientId);
    Task<IEnumerable<Invoice>> GetInvoicesByProjectIdAsync(int projectId);
    Task<IEnumerable<Invoice>> GetInvoicesByStatusAsync(string status);
    Task<IEnumerable<Invoice>> GetOverdueInvoicesAsync();
    Task<IEnumerable<Invoice>> GetPaidInvoicesAsync();
    Task<IEnumerable<Invoice>> GetInvoicesByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<Invoice?> GetInvoiceByNumberAsync(string invoiceNumber);
    Task<Invoice?> GetInvoiceWithItemsAsync(int invoiceId);
    Task<decimal> GetTotalAmountByUserAsync(int userId);
    Task<decimal> GetTotalAmountByClientAsync(int clientId);
    Task<decimal> GetTotalPaidAmountByUserAsync(int userId);
    Task<decimal> GetTotalOutstandingAmountByUserAsync(int userId);
    Task<string> GenerateInvoiceNumberAsync();
} 