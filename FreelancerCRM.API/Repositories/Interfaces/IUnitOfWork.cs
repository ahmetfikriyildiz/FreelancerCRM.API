namespace FreelancerCRM.API.Repositories.Interfaces;

public interface IUnitOfWork : IDisposable
{
    // Repository Properties
    IUserRepository Users { get; }
    IClientRepository Clients { get; }
    IProjectRepository Projects { get; }
    IAssignmentRepository Assignments { get; }
    ITimeEntryRepository TimeEntries { get; }
    IInvoiceRepository Invoices { get; }
    IInvoiceItemRepository InvoiceItems { get; }

    // Transaction Management
    Task<int> SaveChangesAsync();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    
    // Transaction Operations
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
    
    // Bulk Operations
    Task<int> ExecuteSqlRawAsync(string sql, params object[] parameters);
    Task<int> ExecuteSqlRawAsync(string sql, CancellationToken cancellationToken, params object[] parameters);
} 