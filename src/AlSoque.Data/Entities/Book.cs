using System.ComponentModel.DataAnnotations;

namespace AlSoque.Data.Entities;

/// <summary>
/// كتاب من تأليف عالم من علماء آل سوق
/// </summary>
public class Book : BaseEntity
{
    [Required(ErrorMessage = "عنوان الكتاب مطلوب")]
    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public BookStatus Status { get; set; } = BookStatus.Manuscript;

    public int? PublicationYear { get; set; }

    public string Slug { get; set; } = null!;

    public int ScholarId { get; set; }
    public Scholar Scholar { get; set; } = null!;
}
