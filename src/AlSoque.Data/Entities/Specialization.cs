using System.ComponentModel.DataAnnotations;

namespace AlSoque.Data.Entities;

/// <summary>
/// مجال علمي مثل الفقه أو الحديث أو التفسير
/// </summary>
public class Specialization : BaseEntity
{
    [Required(ErrorMessage = "اسم المجال العلمي مطلوب")]
    public string Name { get; set; } = null!;

    public List<Scholar> Scholars { get; set; } = [];

    public override string ToString() => Name;
}
