using FreelancerCRM.API.Models;
using FreelancerCRM.API.Repositories.Interfaces;
using FreelancerCRM.API.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace FreelancerCRM.API.Services;

public class InvoiceItemService : BaseService<InvoiceItem>, IInvoiceItemService
{
    public InvoiceItemService(IUnitOfWork unitOfWork, ILogger<InvoiceItemService> logger) 
        : base(unitOfWork, logger)
    {
    }

    protected override async Task<InvoiceItem?> GetEntityByIdAsync(int id)
    {
        return await _unitOfWork.InvoiceItems.GetByIdAsync(id);
    }

    protected override async Task<IEnumerable<InvoiceItem>> GetAllEntitiesAsync()
    {
        return await _unitOfWork.InvoiceItems.GetAllAsync();
    }

    protected override async Task<InvoiceItem> CreateEntityAsync(InvoiceItem entity)
    {
        return await _unitOfWork.InvoiceItems.AddAsync(entity);
    }

    protected override async Task UpdateEntityAsync(InvoiceItem entity)
    {
        await _unitOfWork.InvoiceItems.UpdateAsync(entity);
    }

    protected override async Task DeleteEntityAsync(InvoiceItem entity)
    {
        await _unitOfWork.InvoiceItems.DeleteAsync(entity);
    }

    protected override async Task<bool> EntityExistsAsync(int id)
    {
        return await _unitOfWork.InvoiceItems.ExistsAsync(ii => ii.InvoiceItemID == id);
    }

    protected override async Task<ValidationResult> ValidateEntityAsync(InvoiceItem entity, bool isUpdate)
    {
        var errors = new List<string>();

        if (entity.InvoiceID <= 0)
        {
            errors.Add("Invoice ID is required");
        }

        if (entity.Quantity <= 0)
        {
            errors.Add("Quantity must be greater than 0");
        }

        if (entity.UnitPrice < 0)
        {
            errors.Add("Unit price cannot be negative");
        }

        await Task.CompletedTask;
        return errors.Any() ? ValidationResult.Failure(errors) : ValidationResult.Success();
    }

    protected override async Task<(bool CanDelete, string Reason)> CanDeleteEntityAsync(InvoiceItem entity)
    {
        await Task.CompletedTask;
        return (true, string.Empty);
    }

    // Basic implementations
    public async Task<ServiceResult<IEnumerable<InvoiceItem>>> GetItemsByInvoiceIdAsync(int invoiceId)
    {
        try
        {
            var items = await _unitOfWork.InvoiceItems.GetItemsByInvoiceIdAsync(invoiceId);
            return ServiceResult<IEnumerable<InvoiceItem>>.Success(items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting items for invoice {InvoiceId}", invoiceId);
            return ServiceResult<IEnumerable<InvoiceItem>>.Failure("An error occurred while retrieving invoice items");
        }
    }

    // TODO: Implement remaining interface methods
    public Task<ServiceResult<InvoiceItem>> AddItemToInvoiceAsync(int invoiceId, string description, decimal quantity, decimal unitPrice, string itemType) => throw new NotImplementedException();
    public Task<ServiceResult<bool>> RemoveItemFromInvoiceAsync(int invoiceItemId) => throw new NotImplementedException();
    public Task<ServiceResult<bool>> UpdateItemQuantityAsync(int invoiceItemId, decimal quantity) => throw new NotImplementedException();
    public Task<ServiceResult<bool>> UpdateItemPriceAsync(int invoiceItemId, decimal unitPrice) => throw new NotImplementedException();
    public Task<ServiceResult<decimal>> CalculateItemTotalAsync(int invoiceItemId) => throw new NotImplementedException();
    public Task<ServiceResult<decimal>> GetInvoiceTotalAsync(int invoiceId) => throw new NotImplementedException();
    public Task<ServiceResult<IEnumerable<InvoiceItem>>> GetItemsByTypeAsync(string itemType) => throw new NotImplementedException();
    public Task<ServiceResult<bool>> BulkAddItemsAsync(int invoiceId, List<InvoiceItem> items) => throw new NotImplementedException();
    public Task<ServiceResult<bool>> BulkRemoveItemsAsync(List<int> invoiceItemIds) => throw new NotImplementedException();
} 