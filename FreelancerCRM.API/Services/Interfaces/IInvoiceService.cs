using FreelancerCRM.API.DTOs;
using FreelancerCRM.API.Models;

namespace FreelancerCRM.API.Services.Interfaces;

public interface IInvoiceService : IBaseService<Invoice, InvoiceCreateDto, InvoiceUpdateDto, InvoiceResponseDto, InvoiceSummaryDto>
{
    Task<ServiceResult<IEnumerable<Invoice>>> GetInvoicesByUserIdAsync(int userId);
    Task<ServiceResult<IEnumerable<Invoice>>> GetInvoicesByClientIdAsync(int clientId);
    Task<ServiceResult<IEnumerable<Invoice>>> GetInvoicesByStatusAsync(string status);
    Task<ServiceResult<IEnumerable<Invoice>>> GetOverdueInvoicesAsync();
    Task<ServiceResult<Invoice>> GetInvoiceWithItemsAsync(int invoiceId);
    Task<ServiceResult<Invoice>> CreateInvoiceFromTimeEntriesAsync(int userId, int clientId, int? projectId, List<int> timeEntryIds);
    Task<ServiceResult<Invoice>> CreateInvoiceFromProjectAsync(int projectId);
    Task<ServiceResult<bool>> SendInvoiceAsync(int invoiceId);
    Task<ServiceResult<bool>> MarkAsPaidAsync(int invoiceId, DateTime paidDate);
    Task<ServiceResult<bool>> CancelInvoiceAsync(int invoiceId);
    Task<ServiceResult<string>> GenerateInvoiceNumberAsync();
    Task<ServiceResult<decimal>> CalculateInvoiceTotalAsync(int invoiceId);
    Task<ServiceResult<bool>> ApplyDiscountAsync(int invoiceId, decimal discountAmount);
    Task<ServiceResult<bool>> UpdatePaymentTermsAsync(int invoiceId, string paymentTerms);
    Task<ServiceResult<decimal>> GetTotalOutstandingAmountAsync(int userId);
    Task<ServiceResult<decimal>> GetTotalPaidAmountAsync(int userId, DateTime? startDate = null, DateTime? endDate = null);
    Task<ServiceResult<IEnumerable<Invoice>>> GetInvoicesByDateRangeAsync(DateTime startDate, DateTime endDate);
} 