namespace AlSoque.Data.Entities;

/// <summary>
/// مساهمة من مستخدم مسجّل (ترجمة جديدة أو تصحيح) — تبقى Pending حتى تراجعها الإدارة.
/// </summary>
public class Contribution : BaseEntity
{
    public ContributionType Type { get; set; }

    public ContributionStatus Status { get; set; } = ContributionStatus.Pending;

    public int? TargetScholarId { get; set; }
    public Scholar? TargetScholar { get; set; }

    /// <summary>البيانات المقترحة كـ JSON — jsonb على PostgreSQL.</summary>
    public string PayloadJson { get; set; } = "{}";

    public string SubmittedByUserId { get; set; } = null!;
    public ApplicationUser SubmittedByUser { get; set; } = null!;

    public string? ReviewedByUserId { get; set; }
    public ApplicationUser? ReviewedByUser { get; set; }

    public string? ReviewerNote { get; set; }

    public DateTime? ReviewedAt { get; set; }
}
