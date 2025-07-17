namespace FreelancerCRM.API.Models;

/// <summary>
/// Tüm entity'ler için temel sınıf
/// </summary>
public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
} 