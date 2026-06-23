using AlSoque.Data;
using AlSoque.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AlSoque.Web.Features.Manuscripts;

public class ManuscriptsService(IDbContextFactory<AppDbContext> dbFactory)
{
    public async Task<List<Manuscript>> GetAllAsync()
    {
        using var db = await dbFactory.CreateDbContextAsync();
        return await db.Manuscripts
            .Include(m => m.Scholar)
            .Include(m => m.Family)
            .OrderBy(m => m.Title)
            .ToListAsync();
    }

    public async Task<Manuscript?> GetBySlugAsync(string slug)
    {
        using var db = await dbFactory.CreateDbContextAsync();
        return await db.Manuscripts
            .Include(m => m.Scholar)
            .Include(m => m.Family)
            .FirstOrDefaultAsync(m => m.Slug == slug);
    }
}
