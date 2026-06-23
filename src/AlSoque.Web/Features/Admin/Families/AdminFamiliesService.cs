using AlSoque.Data;
using AlSoque.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AlSoque.Web.Features.Admin.Families;

public class AdminFamiliesService(IDbContextFactory<AppDbContext> dbFactory)
{
    public async Task<List<Family>> GetAllAsync()
    {
        using var db = await dbFactory.CreateDbContextAsync();
        return await db.Families.OrderBy(f => f.Name).ToListAsync();
    }

    public async Task AddAsync(Family family)
    {
        using var db = await dbFactory.CreateDbContextAsync();
        db.Families.Add(family);
        await db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Family family)
    {
        using var db = await dbFactory.CreateDbContextAsync();
        db.Families.Update(family);
        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        using var db = await dbFactory.CreateDbContextAsync();

        if (await db.Scholars.AnyAsync(s => s.FamilyId == id) || await db.Manuscripts.AnyAsync(m => m.FamilyId == id))
        {
            throw new InvalidOperationException("لا يمكن حذف هذه الأسرة لارتباط علماء أو مخطوطات بها.");
        }

        var family = await db.Families.FindAsync(id);
        if (family is not null)
        {
            db.Families.Remove(family);
            await db.SaveChangesAsync();
        }
    }
}
