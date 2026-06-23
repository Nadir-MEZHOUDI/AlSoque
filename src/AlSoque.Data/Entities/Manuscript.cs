using System.ComponentModel.DataAnnotations;

namespace AlSoque.Data.Entities;

/// <summary>
/// مخطوطة أو وثيقة تاريخية أو إجازة أو صورة من تراث السوق
/// </summary>
public class Manuscript : BaseEntity
{
    [Required(ErrorMessage = "عنوان المخطوطة/الوثيقة مطلوب")]
    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public ManuscriptCategory Category { get; set; } = ManuscriptCategory.Manuscript;

    public string? FilePath { get; set; }

    public int? EstimatedYear { get; set; }

    public string Slug { get; set; } = null!;

    public int? ScholarId { get; set; }
    public Scholar? Scholar { get; set; }

    public int? FamilyId { get; set; }
    public Family? Family { get; set; }
}
