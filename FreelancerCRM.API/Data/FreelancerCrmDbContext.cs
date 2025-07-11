using Microsoft.EntityFrameworkCore;
using FreelancerCRM.API.Models;

namespace FreelancerCRM.API.Data;

public class FreelancerCrmDbContext : DbContext
{
    public FreelancerCrmDbContext(DbContextOptions<FreelancerCrmDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Client> Clients { get; set; } = null!;
    public DbSet<Project> Projects { get; set; } = null!;
    public DbSet<Assignment> Assignments { get; set; } = null!;
    public DbSet<TimeEntry> TimeEntries { get; set; } = null!;
    public DbSet<Invoice> Invoices { get; set; } = null!;
    public DbSet<InvoiceItem> InvoiceItems { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Project - User (Restrict)
        modelBuilder.Entity<Project>()
            .HasOne(p => p.User)
            .WithMany(u => u.Projects)
            .HasForeignKey(p => p.UserID)
            .OnDelete(DeleteBehavior.Restrict);

        // Project - Client (Restrict)
        modelBuilder.Entity<Project>()
            .HasOne(p => p.Client)
            .WithMany(c => c.Projects)
            .HasForeignKey(p => p.ClientID)
            .OnDelete(DeleteBehavior.Restrict);

        // Client - User (Cascade OK)
        modelBuilder.Entity<Client>()
            .HasOne(c => c.User)
            .WithMany(u => u.Clients)
            .HasForeignKey(c => c.UserID)
            .OnDelete(DeleteBehavior.Cascade);

        // Invoice - User (Restrict)
        modelBuilder.Entity<Invoice>()
            .HasOne(i => i.User)
            .WithMany(u => u.Invoices)
            .HasForeignKey(i => i.UserID)
            .OnDelete(DeleteBehavior.Restrict);

        // Invoice - Client (Restrict)
        modelBuilder.Entity<Invoice>()
            .HasOne(i => i.Client)
            .WithMany(c => c.Invoices)
            .HasForeignKey(i => i.ClientID)
            .OnDelete(DeleteBehavior.Restrict);

        // Invoice - Project (Restrict)
        modelBuilder.Entity<Invoice>()
            .HasOne(i => i.Project)
            .WithMany(p => p.Invoices)
            .HasForeignKey(i => i.ProjectID)
            .OnDelete(DeleteBehavior.Restrict);

        // Assignment - Project (Cascade OK)
        modelBuilder.Entity<Assignment>()
            .HasOne(a => a.Project)
            .WithMany(p => p.Tasks)
            .HasForeignKey(a => a.ProjectID)
            .OnDelete(DeleteBehavior.Cascade);

        // TimeEntry - User (Cascade OK)
        modelBuilder.Entity<TimeEntry>()
            .HasOne(t => t.User)
            .WithMany(u => u.TimeEntries)
            .HasForeignKey(t => t.UserID)
            .OnDelete(DeleteBehavior.Cascade);

        // TimeEntry - Project (Cascade OK)
        modelBuilder.Entity<TimeEntry>()
            .HasOne(t => t.Project)
            .WithMany(p => p.TimeEntries)
            .HasForeignKey(t => t.ProjectID)
            .OnDelete(DeleteBehavior.Cascade);

        // TimeEntry - Assignment (Restrict, nullable FK)
        modelBuilder.Entity<TimeEntry>()
            .HasOne(t => t.Assignment)
            .WithMany(a => a.TimeEntries)
            .HasForeignKey(t => t.AssignmentID)
            .OnDelete(DeleteBehavior.Restrict);

        // InvoiceItem - Invoice (Cascade OK)
        modelBuilder.Entity<InvoiceItem>()
            .HasOne(ii => ii.Invoice)
            .WithMany(i => i.InvoiceItems)
            .HasForeignKey(ii => ii.InvoiceID)
            .OnDelete(DeleteBehavior.Cascade);

        // Decimal precision ayarları (18,2)
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(decimal) || property.ClrType == typeof(decimal?))
                {
                    property.SetPrecision(18);
                    property.SetScale(2);
                }
            }
        }
    }
} 