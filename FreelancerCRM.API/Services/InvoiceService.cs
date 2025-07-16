using AutoMapper;
using FreelancerCRM.API.DTOs;
using FreelancerCRM.API.Models;
using FreelancerCRM.API.Repositories.Interfaces;
using FreelancerCRM.API.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace FreelancerCRM.API.Services;

public class InvoiceService : BaseService<Invoice, InvoiceCreateDto, InvoiceUpdateDto, InvoiceResponseDto, InvoiceSummaryDto>, IInvoiceService
{
    public InvoiceService(
        IUnitOfWork unitOfWork, 
        ILogger<InvoiceService> logger,
        IMapper mapper) 
        : base(unitOfWork, logger, mapper)
    {
    }

    protected override async Task<Invoice?> GetEntityByIdAsync(int id)
    {
        return await _unitOfWork.Invoices.GetByIdAsync(id);
    }

    protected override async Task<IEnumerable<Invoice>> GetAllEntitiesAsync()
    {
        return await _unitOfWork.Invoices.GetAllAsync();
    }

    protected override async Task<Invoice> CreateEntityAsync(Invoice entity)
    {
        return await _unitOfWork.Invoices.AddAsync(entity);
    }

    protected override async Task UpdateEntityAsync(Invoice entity)
    {
        await _unitOfWork.Invoices.UpdateAsync(entity);
    }

    protected override async Task DeleteEntityAsync(Invoice entity)
    {
        await _unitOfWork.Invoices.DeleteAsync(entity);
    }

    protected override async Task<bool> EntityExistsAsync(int id)
    {
        return await _unitOfWork.Invoices.ExistsAsync(i => i.InvoiceID == id);
    }

    protected override async Task<ValidationResult> ValidateEntityAsync(Invoice entity, bool isUpdate)
    {
        var errors = new List<string>();

        if (entity.UserID <= 0)
        {
            errors.Add("User ID is required");
        }

        if (entity.ClientID <= 0)
        {
            errors.Add("Client ID is required");
        }

        if (entity.IssueDate > entity.DueDate)
        {
            errors.Add("Due date must be after issue date");
        }

        if (entity.Subtotal < 0)
        {
            errors.Add("Subtotal cannot be negative");
        }

        if (entity.TaxRate < 0 || entity.TaxRate > 100)
        {
            errors.Add("Tax rate must be between 0 and 100");
        }

        if (entity.DiscountRate < 0 || entity.DiscountRate > 100)
        {
            errors.Add("Discount rate must be between 0 and 100");
        }

        await Task.CompletedTask;
        return errors.Any() ? ValidationResult.Failure(errors) : ValidationResult.Success();
    }

    protected override async Task<(bool CanDelete, string Reason)> CanDeleteEntityAsync(Invoice entity)
    {
        // Check if invoice has any payments
        if (entity.PaidAmount > 0)
        {
            return (false, "Cannot delete invoice with payments");
        }

        await Task.CompletedTask;
        return (true, string.Empty);
    }

    public async Task<ServiceResult<decimal>> CalculateInvoiceTotalsAsync(Invoice invoice)
    {
        try
        {
            // Calculate subtotal from invoice items
            decimal subtotal = 0;
            if (invoice.InvoiceItems != null)
            {
                subtotal = invoice.InvoiceItems.Sum(item => item.TotalPrice);
            }
            invoice.Subtotal = subtotal;

            // Calculate tax amount
            invoice.TaxAmount = invoice.Subtotal * (invoice.TaxRate / 100);

            // Calculate discount amount
            invoice.DiscountAmount = invoice.Subtotal * (invoice.DiscountRate / 100);

            // Calculate total amount
            invoice.TotalAmount = invoice.Subtotal + invoice.TaxAmount - invoice.DiscountAmount;

            // Calculate outstanding amount
            invoice.OutstandingAmount = invoice.TotalAmount - invoice.PaidAmount;

            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<decimal>.Success(invoice.TotalAmount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating invoice totals");
            return ServiceResult<decimal>.Failure("An error occurred while calculating invoice totals");
        }
    }

    // Basic implementations
    public async Task<ServiceResult<IEnumerable<Invoice>>> GetInvoicesByUserIdAsync(int userId)
    {
        try
        {
            var invoices = await _unitOfWork.Invoices.GetInvoicesByUserIdAsync(userId);
            return ServiceResult<IEnumerable<Invoice>>.Success(invoices);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting invoices for user {UserId}", userId);
            return ServiceResult<IEnumerable<Invoice>>.Failure("An error occurred while retrieving invoices");
        }
    }

    public async Task<ServiceResult<IEnumerable<Invoice>>> GetInvoicesByClientIdAsync(int clientId)
    {
        try
        {
            var invoices = await _unitOfWork.Invoices.GetInvoicesByClientIdAsync(clientId);
            return ServiceResult<IEnumerable<Invoice>>.Success(invoices);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting invoices for client {ClientId}", clientId);
            return ServiceResult<IEnumerable<Invoice>>.Failure("An error occurred while retrieving invoices");
        }
    }

    public async Task<ServiceResult<string>> GenerateInvoiceNumberAsync()
    {
        try
        {
            var invoiceNumber = await _unitOfWork.Invoices.GenerateInvoiceNumberAsync();
            return ServiceResult<string>.Success(invoiceNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating invoice number");
            return ServiceResult<string>.Failure("An error occurred while generating invoice number");
        }
    }

    public async Task<ServiceResult<IEnumerable<Invoice>>> GetInvoicesByStatusAsync(string status)
    {
        try
        {
            var invoices = await _unitOfWork.Invoices.GetInvoicesByStatusAsync(status);
            return ServiceResult<IEnumerable<Invoice>>.Success(invoices);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting invoices by status {Status}", status);
            return ServiceResult<IEnumerable<Invoice>>.Failure("An error occurred while retrieving invoices");
        }
    }

    public async Task<ServiceResult<IEnumerable<Invoice>>> GetOverdueInvoicesAsync()
    {
        try
        {
            var invoices = await _unitOfWork.Invoices.GetOverdueInvoicesAsync();
            return ServiceResult<IEnumerable<Invoice>>.Success(invoices);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting overdue invoices");
            return ServiceResult<IEnumerable<Invoice>>.Failure("An error occurred while retrieving overdue invoices");
        }
    }

    public async Task<ServiceResult<Invoice>> GetInvoiceWithItemsAsync(int invoiceId)
    {
        try
        {
            var invoice = await _unitOfWork.Invoices.GetInvoiceWithItemsAsync(invoiceId);
            if (invoice == null)
            {
                return ServiceResult<Invoice>.Failure("Invoice not found");
            }

            return ServiceResult<Invoice>.Success(invoice);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting invoice with items {InvoiceId}", invoiceId);
            return ServiceResult<Invoice>.Failure("An error occurred while retrieving invoice");
        }
    }

    public async Task<ServiceResult<bool>> MarkAsPaidAsync(int invoiceId, DateTime paidDate)
    {
        try
        {
            var invoice = await _unitOfWork.Invoices.GetByIdAsync(invoiceId);
            if (invoice == null)
            {
                return ServiceResult<bool>.Failure("Invoice not found");
            }

            if (invoice.Status == "Paid")
            {
                return ServiceResult<bool>.Failure("Invoice is already marked as paid");
            }

            if (invoice.Status == "Cancelled")
            {
                return ServiceResult<bool>.Failure("Cannot mark cancelled invoice as paid");
            }

            await _unitOfWork.BeginTransactionAsync();

            invoice.Status = "Paid";
            invoice.PaidAt = paidDate;
            invoice.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Invoices.UpdateAsync(invoice);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error marking invoice {InvoiceId} as paid", invoiceId);
            return ServiceResult<bool>.Failure("An error occurred while marking invoice as paid");
        }
    }

    public async Task<ServiceResult<bool>> CancelInvoiceAsync(int invoiceId)
    {
        try
        {
            var invoice = await _unitOfWork.Invoices.GetByIdAsync(invoiceId);
            if (invoice == null)
            {
                return ServiceResult<bool>.Failure("Invoice not found");
            }

            if (invoice.Status == "Paid")
            {
                return ServiceResult<bool>.Failure("Cannot cancel paid invoice");
            }

            if (invoice.Status == "Cancelled")
            {
                return ServiceResult<bool>.Failure("Invoice is already cancelled");
            }

            await _unitOfWork.BeginTransactionAsync();

            invoice.Status = "Cancelled";
            invoice.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Invoices.UpdateAsync(invoice);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error cancelling invoice {InvoiceId}", invoiceId);
            return ServiceResult<bool>.Failure("An error occurred while cancelling invoice");
        }
    }

    public async Task<ServiceResult<decimal>> CalculateInvoiceTotalAsync(int invoiceId)
    {
        try
        {
            var items = await _unitOfWork.InvoiceItems.GetItemsByInvoiceIdAsync(invoiceId);
            var subtotal = items.Sum(item => item.TotalPrice);

            var invoice = await _unitOfWork.Invoices.GetByIdAsync(invoiceId);
            if (invoice == null)
            {
                return ServiceResult<decimal>.Failure("Invoice not found");
            }

            // Calculate tax
            var taxAmount = subtotal * (invoice.TaxRate / 100);
            
            // Apply discount
            var discountAmount = invoice.DiscountAmount;
            
            // Calculate total
            var total = subtotal + taxAmount - discountAmount;

            return ServiceResult<decimal>.Success(total);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating invoice total {InvoiceId}", invoiceId);
            return ServiceResult<decimal>.Failure("An error occurred while calculating invoice total");
        }
    }

    public async Task<ServiceResult<bool>> ApplyDiscountAsync(int invoiceId, decimal discountAmount)
    {
        try
        {
            var invoice = await _unitOfWork.Invoices.GetByIdAsync(invoiceId);
            if (invoice == null)
            {
                return ServiceResult<bool>.Failure("Invoice not found");
            }

            if (invoice.Status == "Paid")
            {
                return ServiceResult<bool>.Failure("Cannot apply discount to paid invoice");
            }

            if (discountAmount < 0)
            {
                return ServiceResult<bool>.ValidationFailure(new List<string> { "Discount amount cannot be negative" });
            }

            if (discountAmount > invoice.Subtotal)
            {
                return ServiceResult<bool>.ValidationFailure(new List<string> { "Discount amount cannot exceed subtotal" });
            }

            await _unitOfWork.BeginTransactionAsync();

            invoice.DiscountAmount = discountAmount;
            invoice.TotalAmount = invoice.Subtotal + invoice.TaxAmount - discountAmount;
            invoice.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Invoices.UpdateAsync(invoice);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error applying discount to invoice {InvoiceId}", invoiceId);
            return ServiceResult<bool>.Failure("An error occurred while applying discount");
        }
    }

    public async Task<ServiceResult<decimal>> GetTotalOutstandingAmountAsync(int userId)
    {
        try
        {
            var outstandingAmount = await _unitOfWork.Invoices.GetTotalOutstandingAmountByUserAsync(userId);
            return ServiceResult<decimal>.Success(outstandingAmount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting total outstanding amount for user {UserId}", userId);
            return ServiceResult<decimal>.Failure("An error occurred while calculating outstanding amount");
        }
    }

    public async Task<ServiceResult<decimal>> GetTotalPaidAmountAsync(int userId, DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var paidAmount = await _unitOfWork.Invoices.GetTotalPaidAmountByUserAsync(userId);
            return ServiceResult<decimal>.Success(paidAmount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting total paid amount for user {UserId}", userId);
            return ServiceResult<decimal>.Failure("An error occurred while calculating paid amount");
        }
    }

    public async Task<ServiceResult<IEnumerable<Invoice>>> GetInvoicesByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            var invoices = await _unitOfWork.Invoices.GetInvoicesByDateRangeAsync(startDate, endDate);
            return ServiceResult<IEnumerable<Invoice>>.Success(invoices);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting invoices by date range");
            return ServiceResult<IEnumerable<Invoice>>.Failure("An error occurred while retrieving invoices");
        }
    }

    public async Task<ServiceResult<Invoice>> CreateInvoiceFromTimeEntriesAsync(int userId, int clientId, int? projectId, List<int> timeEntryIds)
    {
        try
        {
            if (timeEntryIds == null || !timeEntryIds.Any())
            {
                return ServiceResult<Invoice>.Failure("No time entries provided");
            }

            // Get time entries one by one since we don't have GetTimeEntriesByIdsAsync
            var timeEntries = new List<TimeEntry>();
            foreach (var id in timeEntryIds)
            {
                var timeEntry = await _unitOfWork.TimeEntries.GetByIdAsync(id);
                if (timeEntry != null)
                {
                    timeEntries.Add(timeEntry);
                }
            }

            if (!timeEntries.Any())
            {
                return ServiceResult<Invoice>.Failure("No valid time entries found");
            }

            await _unitOfWork.BeginTransactionAsync();

            var invoice = new Invoice
            {
                UserID = userId,
                ClientID = clientId,
                ProjectID = projectId,
                IssueDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(30), // Default 30 days
                Status = "Draft",
                InvoiceNumber = await _unitOfWork.Invoices.GenerateInvoiceNumberAsync()
            };

            await _unitOfWork.Invoices.AddAsync(invoice);
            await _unitOfWork.SaveChangesAsync();

            // Create invoice items from time entries
            foreach (var timeEntry in timeEntries)
            {
                var invoiceItem = new InvoiceItem
                {
                    InvoiceID = invoice.InvoiceID,
                    Description = $"Time entry: {timeEntry.Description}",
                    Quantity = timeEntry.Duration,
                    UnitPrice = timeEntry.HourlyRate,
                    TotalPrice = timeEntry.Duration * timeEntry.HourlyRate
                };

                await _unitOfWork.InvoiceItems.AddAsync(invoiceItem);
            }

            await _unitOfWork.SaveChangesAsync();

            // Calculate totals
            await CalculateInvoiceTotalsAsync(invoice);
            
            await _unitOfWork.CommitTransactionAsync();

            return ServiceResult<Invoice>.Success(invoice);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error creating invoice from time entries");
            return ServiceResult<Invoice>.Failure("An error occurred while creating invoice from time entries");
        }
    }

    public async Task<ServiceResult<Invoice>> CreateInvoiceFromProjectAsync(int projectId)
    {
        try
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(projectId);
            if (project == null)
            {
                return ServiceResult<Invoice>.Failure("Project not found");
            }

            var timeEntries = await _unitOfWork.TimeEntries.GetTimeEntriesByProjectIdAsync(projectId);
            if (!timeEntries.Any())
            {
                return ServiceResult<Invoice>.Failure("No time entries found for this project");
            }

            return await CreateInvoiceFromTimeEntriesAsync(
                project.UserID,
                project.ClientID,
                projectId,
                timeEntries.Select(te => te.TimeEntryID).ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating invoice from project {ProjectId}", projectId);
            return ServiceResult<Invoice>.Failure("An error occurred while creating invoice from project");
        }
    }

    public async Task<ServiceResult<bool>> SendInvoiceAsync(int invoiceId)
    {
        try
        {
            var invoice = await _unitOfWork.Invoices.GetByIdAsync(invoiceId);
            if (invoice == null)
            {
                return ServiceResult<bool>.Failure("Invoice not found");
            }

            if (invoice.Status != "Draft")
            {
                return ServiceResult<bool>.Failure("Only draft invoices can be sent");
            }

            await _unitOfWork.BeginTransactionAsync();

            invoice.Status = "Sent";
            invoice.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Invoices.UpdateAsync(invoice);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            // TODO: Implement email sending logic here

            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error sending invoice {InvoiceId}", invoiceId);
            return ServiceResult<bool>.Failure("An error occurred while sending the invoice");
        }
    }

    public async Task<ServiceResult<bool>> UpdatePaymentTermsAsync(int invoiceId, string paymentTerms)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(paymentTerms))
            {
                return ServiceResult<bool>.Failure("Payment terms cannot be empty");
            }

            var invoice = await _unitOfWork.Invoices.GetByIdAsync(invoiceId);
            if (invoice == null)
            {
                return ServiceResult<bool>.Failure("Invoice not found");
            }

            if (invoice.Status == "Paid" || invoice.Status == "Cancelled")
            {
                return ServiceResult<bool>.Failure("Cannot update payment terms for paid or cancelled invoices");
            }

            await _unitOfWork.BeginTransactionAsync();

            invoice.PaymentTerms = paymentTerms;
            invoice.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Invoices.UpdateAsync(invoice);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error updating payment terms for invoice {InvoiceId}", invoiceId);
            return ServiceResult<bool>.Failure("An error occurred while updating payment terms");
        }
    }
} 