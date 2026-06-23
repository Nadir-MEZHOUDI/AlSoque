using AlSoque.Data;
using AlSoque.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AlSoque.Web.Features.Admin.Manuscripts;

public class AdminManuscriptsService(IDbContextFactory<AppDbContext> dbFactory)
{
    public async Task<List<Manuscript>> GetAllAsync()
    {
        using var db = await dbFactory.CreateDbContextAsync();
        return await db.Manuscripts.Include(m => m.Scholar).Include(m => m.Family).OrderBy(m => m.Title).ToListAsync();
    }

    public async Task<Manuscript?> GetByIdAsync(int id)
    {
        using var db = await dbFactory.CreateDbContextAsync();
        return await db.Manuscripts.FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<List<Scholar>> GetScholarsAsync()
    {
        using var db = await dbFactory.CreateDbContextAsync();
        return await db.Scholars.OrderBy(s => s.Name).ToListAsync();
    }

    public async Task<List<Family>> GetFamiliesAsync()
    {
        using var db = await dbFactory.CreateDbContextAsync();
        return await db.Families.OrderBy(f => f.Name).ToListAsync();
    }

    public async Task<bool> IsSlugTakenAsync(string slug, int? excludeId)
    {
        using var db = await dbFactory.CreateDbContextAsync();
        return await db.Manuscripts.AnyAsync(m => m.Slug == slug && m.Id != (excludeId ?? 0));
    }

    public async Task SaveAsync(Manuscript input)
    {
        using var db = await dbFactory.CreateDbContextAsync();

        if (input.Id == 0)
        {
            db.Manuscripts.Add(new Manuscript
            {
                Title = input.Title,
                Description = input.Description,
                Category = input.Category,
                FilePath = input.FilePath,
                EstimatedYear = input.EstimatedYear,
                Slug = input.Slug,
                ScholarId = input.ScholarId,
                FamilyId = input.FamilyId,
            });
        }
        else
        {
            var manuscript = await db.Manuscripts.FirstAsync(m => m.Id == input.Id);
            manuscript.Title = input.Title;
            manuscript.Description = input.Description;
            manuscript.Category = input.Category;
            manuscript.FilePath = input.FilePath;
            manuscript.EstimatedYear = input.EstimatedYear;
            manuscript.Slug = input.Slug;
            manuscript.ScholarId = input.ScholarId;
            manuscript.FamilyId = input.FamilyId;
        }

        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        using var db = await dbFactory.CreateDbContextAsync();
        var manuscript = await db.Manuscripts.FindAsync(id);
        if (manuscript is not null)
        {
            db.Manuscripts.Remove(manuscript);
            await db.SaveChangesAsync();
        }
    }
}
