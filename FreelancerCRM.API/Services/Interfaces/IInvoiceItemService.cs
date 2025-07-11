using FreelancerCRM.API.Models;

namespace FreelancerCRM.API.Services.Interfaces;

public interface IInvoiceItemService : IBaseService<InvoiceItem>
{
    Task<ServiceResult<IEnumerable<InvoiceItem>>> GetItemsByInvoiceIdAsync(int invoiceId);
    Task<ServiceResult<InvoiceItem>> AddItemToInvoiceAsync(int invoiceId, string description, decimal quantity, decimal unitPrice, string itemType);
    Task<ServiceResult<bool>> RemoveItemFromInvoiceAsync(int invoiceItemId);
    Task<ServiceResult<bool>> UpdateItemQuantityAsync(int invoiceItemId, decimal quantity);
    Task<ServiceResult<bool>> UpdateItemPriceAsync(int invoiceItemId, decimal unitPrice);
    Task<ServiceResult<decimal>> CalculateItemTotalAsync(int invoiceItemId);
    Task<ServiceResult<decimal>> GetInvoiceTotalAsync(int invoiceId);
    Task<ServiceResult<IEnumerable<InvoiceItem>>> GetItemsByTypeAsync(string itemType);
    Task<ServiceResult<bool>> BulkAddItemsAsync(int invoiceId, List<InvoiceItem> items);
    Task<ServiceResult<bool>> BulkRemoveItemsAsync(List<int> invoiceItemIds);
} 