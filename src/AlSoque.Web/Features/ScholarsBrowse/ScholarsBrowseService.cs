using AlSoque.Data;
using AlSoque.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AlSoque.Web.Features.ScholarsBrowse;

public record ScholarsBrowseFilter(string? SearchText, int? FamilyId, int? SpecializationId, int? Century);

public record ScholarsBrowseResult(List<Scholar> Scholars, List<Family> Families, List<Specialization> Specializations);

public class ScholarsBrowseService(IDbContextFactory<AppDbContext> dbFactory)
{
    public async Task<ScholarsBrowseResult> GetAsync(ScholarsBrowseFilter filter)
    {
        using var db = await dbFactory.CreateDbContextAsync();

        var query = db.Scholars
            .Include(s => s.Family)
            .Include(s => s.Specializations)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.SearchText))
        {
            var term = filter.SearchText.Trim();
            query = query.Where(s => s.Name.Contains(term) || (s.AlNisbah != null && s.AlNisbah.Contains(term)));
        }

        if (filter.FamilyId is { } familyId)
        {
            query = query.Where(s => s.FamilyId == familyId);
        }

        if (filter.SpecializationId is { } specializationId)
        {
            query = query.Where(s => s.Specializations.Any(sp => sp.Id == specializationId));
        }

        if (filter.Century is { } century)
        {
            var from = ((century - 1) * 100) + 1;
            var to = century * 100;
            query = query.Where(s => s.AlWufat >= from && s.AlWufat <= to);
        }

        var scholars = await query
            .OrderBy(s => s.AlWufat ?? int.MaxValue)
            .ThenBy(s => s.Name)
            .ToListAsync();

        var families = await db.Families.OrderBy(f => f.Name).ToListAsync();
        var specializations = await db.Specializations.OrderBy(s => s.Name).ToListAsync();

        return new ScholarsBrowseResult(scholars, families, specializations);
    }
}
