using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using FreelancerCRM.API.Data;
using FreelancerCRM.API.Repositories.Interfaces;

namespace FreelancerCRM.API.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly FreelancerCrmDbContext _context;
    private IDbContextTransaction? _transaction;

    // Repository instances
    private IUserRepository? _users;
    private IClientRepository? _clients;
    private IProjectRepository? _projects;
    private IAssignmentRepository? _assignments;
    private ITimeEntryRepository? _timeEntries;
    private IInvoiceRepository? _invoices;
    private IInvoiceItemRepository? _invoiceItems;

    public UnitOfWork(FreelancerCrmDbContext context)
    {
        _context = context;
    }

    // Repository Properties (Lazy Loading)
    public IUserRepository Users => _users ??= new UserRepository(_context);
    public IClientRepository Clients => _clients ??= new ClientRepository(_context);
    public IProjectRepository Projects => _projects ??= new ProjectRepository(_context);
    public IAssignmentRepository Assignments => _assignments ??= new AssignmentRepository(_context);
    public ITimeEntryRepository TimeEntries => _timeEntries ??= new TimeEntryRepository(_context);
    public IInvoiceRepository Invoices => _invoices ??= new InvoiceRepository(_context);
    public IInvoiceItemRepository InvoiceItems => _invoiceItems ??= new InvoiceItemRepository(_context);

    // Transaction Management
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    // Transaction Operations
    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    // Bulk Operations
    public async Task<int> ExecuteSqlRawAsync(string sql, params object[] parameters)
    {
        return await _context.Database.ExecuteSqlRawAsync(sql, parameters);
    }

    public async Task<int> ExecuteSqlRawAsync(string sql, CancellationToken cancellationToken, params object[] parameters)
    {
        return await _context.Database.ExecuteSqlRawAsync(sql, cancellationToken, parameters);
    }

    // Dispose Pattern
    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
} 