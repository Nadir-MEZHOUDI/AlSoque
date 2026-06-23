namespace AlSoque.Web.Shared.Contributions;

/// <summary>
/// شكل البيانات المقترَحة لعالم جديد أو تصحيح على عالم موجود — يُسلسَل في Contribution.PayloadJson.
/// </summary>
public class ScholarContributionPayload
{
    public string Name { get; set; } = string.Empty;
    public string? AlKunyah { get; set; }
    public string? AlLaqab { get; set; }
    public string? AlIsm { get; set; }
    public string? AlNisbah { get; set; }
    public string? AlAb { get; set; }
    public string? AlNasab { get; set; }
    public string? AlMadhab { get; set; }
    public string? AlAkidah { get; set; }
    public int? AlMawlid { get; set; }
    public int? AlWufat { get; set; }
    public string? WolidaBi { get; set; }
    public string? TowofiyaBi { get; set; }
    public string? Biography { get; set; }
    public int? FamilyId { get; set; }
}
