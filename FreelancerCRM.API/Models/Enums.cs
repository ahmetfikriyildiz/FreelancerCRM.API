namespace FreelancerCRM.API.Models
{
    public enum ProjectStatus
    {
        Planning = 1,
        InProgress = 2,
        OnHold = 3,
        Completed = 4,
        Cancelled = 5
    }

    public enum InvoiceStatus
    {
        Draft = 1,
        Sent = 2,
        Paid = 3,
        Overdue = 4,
        Cancelled = 5
    }

    public enum AssignmentStatus
    {
        NotStarted = 1,
        InProgress = 2,
        Completed = 3,
        OnHold = 4,
        Cancelled = 5
    }

    public enum Priority
    {
        Low = 1,
        Medium = 2,
        High = 3,
        Critical = 4,
        Urgent = 5
    }
} 