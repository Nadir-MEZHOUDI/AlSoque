namespace AlSoque.Data.Entities;

/// <summary>
/// سند علمي: علاقة شيخ↔تلميذ بين عالمين
/// </summary>
public class ScholarRelation
{
    public int Id { get; set; }

    public int TeacherId { get; set; }
    public Scholar Teacher { get; set; } = null!;

    public int StudentId { get; set; }
    public Scholar Student { get; set; } = null!;
}
