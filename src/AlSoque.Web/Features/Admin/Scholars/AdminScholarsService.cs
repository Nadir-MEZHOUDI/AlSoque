using AlSoque.Data;
using AlSoque.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AlSoque.Web.Features.Admin.Scholars;

public class AdminScholarsService(IDbContextFactory<AppDbContext> dbFactory)
{
    public async Task<List<Scholar>> GetAllAsync()
    {
        using var db = await dbFactory.CreateDbContextAsync();
        return await db.Scholars.Include(s => s.Family).OrderBy(s => s.Name).ToListAsync();
    }

    public async Task<Scholar?> GetByIdAsync(int id)
    {
        using var db = await dbFactory.CreateDbContextAsync();
        return await db.Scholars
            .Include(s => s.Specializations)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<List<Family>> GetFamiliesAsync()
    {
        using var db = await dbFactory.CreateDbContextAsync();
        return await db.Families.OrderBy(f => f.Name).ToListAsync();
    }

    public async Task<List<Specialization>> GetSpecializationsAsync()
    {
        using var db = await dbFactory.CreateDbContextAsync();
        return await db.Specializations.OrderBy(s => s.Name).ToListAsync();
    }

    public async Task<bool> IsSlugTakenAsync(string slug, int? excludeId)
    {
        using var db = await dbFactory.CreateDbContextAsync();
        return await db.Scholars.AnyAsync(s => s.Slug == slug && s.Id != (excludeId ?? 0));
    }

    public async Task SaveAsync(Scholar input, List<int> specializationIds)
    {
        using var db = await dbFactory.CreateDbContextAsync();

        Scholar scholar;
        if (input.Id == 0)
        {
            scholar = new Scholar();
            db.Scholars.Add(scholar);
        }
        else
        {
            scholar = await db.Scholars.Include(s => s.Specializations).FirstAsync(s => s.Id == input.Id);
        }

        scholar.Name = input.Name;
        scholar.AlKunyah = input.AlKunyah;
        scholar.AlLaqab = input.AlLaqab;
        scholar.AlIsm = input.AlIsm;
        scholar.AlNisbah = input.AlNisbah;
        scholar.AlAb = input.AlAb;
        scholar.AlNasab = input.AlNasab;
        scholar.AlMadhab = input.AlMadhab;
        scholar.AlAkidah = input.AlAkidah;
        scholar.AlMawlid = input.AlMawlid;
        scholar.AlWufat = input.AlWufat;
        scholar.WolidaBi = input.WolidaBi;
        scholar.TowofiyaBi = input.TowofiyaBi;
        scholar.Biography = input.Biography;
        scholar.ImagePath = input.ImagePath;
        scholar.Slug = input.Slug;
        scholar.FamilyId = input.FamilyId;

        var specializations = await db.Specializations.Where(s => specializationIds.Contains(s.Id)).ToListAsync();
        scholar.Specializations.Clear();
        foreach (var specialization in specializations)
        {
            scholar.Specializations.Add(specialization);
        }

        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        using var db = await dbFactory.CreateDbContextAsync();

        if (await db.ScholarRelations.AnyAsync(r => r.TeacherId == id || r.StudentId == id))
        {
            throw new InvalidOperationException("لا يمكن حذف هذا العالم لوجود سند علمي (شيوخ/تلاميذ) مرتبط به.");
        }

        var scholar = await db.Scholars.FindAsync(id);
        if (scholar is not null)
        {
            db.Scholars.Remove(scholar);
            await db.SaveChangesAsync();
        }
    }
}
