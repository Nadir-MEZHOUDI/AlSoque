using System.ComponentModel.DataAnnotations;

namespace AlSoque.Data.Entities;

/// <summary>
/// أسرة من أسر علماء السوق — السوقيون ينتمون لأسر متعددة لا أسرة واحدة
/// </summary>
public class Family : BaseEntity
{
    [Required(ErrorMessage = "اسم الأسرة مطلوب")]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string Slug { get; set; } = null!;

    public List<Scholar> Scholars { get; set; } = [];
}
