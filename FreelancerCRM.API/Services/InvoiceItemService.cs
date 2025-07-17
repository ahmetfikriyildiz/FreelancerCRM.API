using AutoMapper;
using FreelancerCRM.API.DTOs;
using FreelancerCRM.API.Models;
using FreelancerCRM.API.Repositories.Interfaces;
using FreelancerCRM.API.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace FreelancerCRM.API.Services;

public class InvoiceItemService : BaseService<InvoiceItem, InvoiceItemCreateDto, InvoiceItemUpdateDto, InvoiceItemResponseDto, InvoiceItemSummaryDto>, IInvoiceItemService
{
    public InvoiceItemService(
        IUnitOfWork unitOfWork,
        ILogger<InvoiceItemService> logger,
        IMapper mapper)
        : base(unitOfWork, logger, mapper)
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

    public async Task<ServiceResult<InvoiceItem>> AddItemToInvoiceAsync(int invoiceId, string description, decimal quantity, decimal unitPrice)
    {
        try
        {
            var invoice = await _unitOfWork.Invoices.GetByIdAsync(invoiceId);
            if (invoice == null)
            {
                return ServiceResult<InvoiceItem>.Failure("Invoice not found");
            }

            var invoiceItem = new InvoiceItem
            {
                InvoiceID = invoiceId,
                Description = description,
                Quantity = quantity,
                UnitPrice = unitPrice,
                TotalPrice = quantity * unitPrice
            };

            var validationResult = await ValidateEntityAsync(invoiceItem, false);
            if (!validationResult.IsValid)
            {
                return ServiceResult<InvoiceItem>.ValidationFailure(validationResult.Errors);
            }

            await _unitOfWork.BeginTransactionAsync();
            var createdItem = await _unitOfWork.InvoiceItems.AddAsync(invoiceItem);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return ServiceResult<InvoiceItem>.Success(createdItem);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error adding item to invoice {InvoiceId}", invoiceId);
            return ServiceResult<InvoiceItem>.Failure("An error occurred while adding the invoice item");
        }
    }

    public async Task<ServiceResult<bool>> RemoveItemFromInvoiceAsync(int invoiceItemId)
    {
        try
        {
            var invoiceItem = await _unitOfWork.InvoiceItems.GetByIdAsync(invoiceItemId);
            if (invoiceItem == null)
            {
                return ServiceResult<bool>.Failure("Invoice item not found");
            }

            await _unitOfWork.BeginTransactionAsync();
            await _unitOfWork.InvoiceItems.DeleteAsync(invoiceItem);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error removing invoice item {InvoiceItemId}", invoiceItemId);
            return ServiceResult<bool>.Failure("An error occurred while removing the invoice item");
        }
    }

    public async Task<ServiceResult<bool>> UpdateItemQuantityAsync(int invoiceItemId, decimal quantity)
    {
        try
        {
            var invoiceItem = await _unitOfWork.InvoiceItems.GetByIdAsync(invoiceItemId);
            if (invoiceItem == null)
            {
                return ServiceResult<bool>.Failure("Invoice item not found");
            }

            invoiceItem.Quantity = quantity;
            invoiceItem.TotalPrice = quantity * invoiceItem.UnitPrice;

            var validationResult = await ValidateEntityAsync(invoiceItem, true);
            if (!validationResult.IsValid)
            {
                return ServiceResult<bool>.ValidationFailure(validationResult.Errors);
            }

            await _unitOfWork.BeginTransactionAsync();
            await _unitOfWork.InvoiceItems.UpdateAsync(invoiceItem);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error updating quantity for invoice item {InvoiceItemId}", invoiceItemId);
            return ServiceResult<bool>.Failure("An error occurred while updating the invoice item quantity");
        }
    }

    public async Task<ServiceResult<bool>> UpdateItemPriceAsync(int invoiceItemId, decimal unitPrice)
    {
        try
        {
            var invoiceItem = await _unitOfWork.InvoiceItems.GetByIdAsync(invoiceItemId);
            if (invoiceItem == null)
            {
                return ServiceResult<bool>.Failure("Invoice item not found");
            }

            invoiceItem.UnitPrice = unitPrice;
            invoiceItem.TotalPrice = unitPrice * invoiceItem.Quantity;

            var validationResult = await ValidateEntityAsync(invoiceItem, true);
            if (!validationResult.IsValid)
            {
                return ServiceResult<bool>.ValidationFailure(validationResult.Errors);
            }

            await _unitOfWork.BeginTransactionAsync();
            await _unitOfWork.InvoiceItems.UpdateAsync(invoiceItem);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error updating price for invoice item {InvoiceItemId}", invoiceItemId);
            return ServiceResult<bool>.Failure("An error occurred while updating the invoice item price");
        }
    }

    public async Task<ServiceResult<decimal>> CalculateItemTotalAsync(int invoiceItemId)
    {
        try
        {
            var invoiceItem = await _unitOfWork.InvoiceItems.GetByIdAsync(invoiceItemId);
            if (invoiceItem == null)
            {
                return ServiceResult<decimal>.Failure("Invoice item not found");
            }

            var total = invoiceItem.Quantity * invoiceItem.UnitPrice;
            return ServiceResult<decimal>.Success(total);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating total for invoice item {InvoiceItemId}", invoiceItemId);
            return ServiceResult<decimal>.Failure("An error occurred while calculating the invoice item total");
        }
    }

    public async Task<ServiceResult<decimal>> GetInvoiceTotalAsync(int invoiceId)
    {
        try
        {
            var items = await _unitOfWork.InvoiceItems.GetItemsByInvoiceIdAsync(invoiceId);
            var total = items.Sum(item => item.TotalPrice);
            return ServiceResult<decimal>.Success(total);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating total for invoice {InvoiceId}", invoiceId);
            return ServiceResult<decimal>.Failure("An error occurred while calculating the invoice total");
        }
    }

    public async Task<ServiceResult<bool>> BulkAddItemsAsync(int invoiceId, List<InvoiceItem> items)
    {
        try
        {
            var invoice = await _unitOfWork.Invoices.GetByIdAsync(invoiceId);
            if (invoice == null)
            {
                return ServiceResult<bool>.Failure("Invoice not found");
            }

            foreach (var item in items)
            {
                item.InvoiceID = invoiceId;
                item.TotalPrice = item.Quantity * item.UnitPrice;

                var validationResult = await ValidateEntityAsync(item, false);
                if (!validationResult.IsValid)
                {
                    return ServiceResult<bool>.ValidationFailure(validationResult.Errors);
                }
            }

            await _unitOfWork.BeginTransactionAsync();
            foreach (var item in items)
            {
                await _unitOfWork.InvoiceItems.AddAsync(item);
            }
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error bulk adding items to invoice {InvoiceId}", invoiceId);
            return ServiceResult<bool>.Failure("An error occurred while adding the invoice items");
        }
    }

    public async Task<ServiceResult<bool>> BulkRemoveItemsAsync(List<int> invoiceItemIds)
    {
        try
        {
            var items = new List<InvoiceItem>();
            foreach (var id in invoiceItemIds)
            {
                var item = await _unitOfWork.InvoiceItems.GetByIdAsync(id);
                if (item == null)
                {
                    return ServiceResult<bool>.Failure($"Invoice item with ID {id} not found");
                }
                items.Add(item);
            }

            await _unitOfWork.BeginTransactionAsync();
            foreach (var item in items)
            {
                await _unitOfWork.InvoiceItems.DeleteAsync(item);
            }
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error bulk removing invoice items");
            return ServiceResult<bool>.Failure("An error occurred while removing the invoice items");
        }
    }
} 