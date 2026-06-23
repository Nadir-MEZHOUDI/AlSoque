using System.ComponentModel.DataAnnotations;
using AlSoque.Data.Extensions;

namespace AlSoque.Data.Entities;

/// <summary>
/// عالم من علماء آل سوق
/// </summary>
public class Scholar : BaseEntity
{
    [Required(ErrorMessage = "اسم الشهرة مطلوب")]
    public string Name { get; set; } = null!;

    public string? AlKunyah { get; set; }
    public string? AlLaqab { get; set; }
    public string? AlIsm { get; set; }
    public string? AlNisbah { get; set; }
    public string? AlAb { get; set; }
    public string? AlNasab { get; set; }
    public string? AlMadhab { get; set; }
    public string? AlAkidah { get; set; }

    [Range(1, 1500)]
    public int? AlMawlid { get; set; }

    [Range(1, 1500)]
    public int? AlWufat { get; set; }

    public string? WolidaBi { get; set; }
    public string? TowofiyaBi { get; set; }

    public string? Biography { get; set; }
    public string? ImagePath { get; set; }

    public string Slug { get; set; } = null!;

    public int? FamilyId { get; set; }
    public Family? Family { get; set; }

    public List<Specialization> Specializations { get; set; } = [];

    /// <summary>روابط يكون فيها هذا العالم تلميذًا</summary>
    public List<ScholarRelation> TeacherLinks { get; set; } = [];

    /// <summary>روابط يكون فيها هذا العالم شيخًا</summary>
    public List<ScholarRelation> StudentLinks { get; set; } = [];

    public List<Book> Books { get; set; } = [];

    public List<Manuscript> Manuscripts { get; set; } = [];

    public int? AlMawlidGregorian => AlMawlid.ToGregorian();
    public int? AlWufatGregorian => AlWufat.ToGregorian();

    public string FullName => string.Join(" ", new[] { AlLaqab, AlKunyah, AlIsm, AlNasab, AlNisbah }
        .Where(part => !string.IsNullOrWhiteSpace(part)));
}
