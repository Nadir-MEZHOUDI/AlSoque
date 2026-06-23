using AlSoque.Data;
using AlSoque.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AlSoque.Web.Features.Admin.Specializations;

public class AdminSpecializationsService(IDbContextFactory<AppDbContext> dbFactory)
{
    public async Task<List<Specialization>> GetAllAsync()
    {
        using var db = await dbFactory.CreateDbContextAsync();
        return await db.Specializations.OrderBy(s => s.Name).ToListAsync();
    }

    public async Task AddAsync(string name)
    {
        using var db = await dbFactory.CreateDbContextAsync();
        db.Specializations.Add(new Specialization { Name = name });
        await db.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, string name)
    {
        using var db = await dbFactory.CreateDbContextAsync();
        var specialization = await db.Specializations.FindAsync(id);
        if (specialization is not null)
        {
            specialization.Name = name;
            await db.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(int id)
    {
        using var db = await dbFactory.CreateDbContextAsync();
        var specialization = await db.Specializations.FindAsync(id);
        if (specialization is not null)
        {
            db.Specializations.Remove(specialization);
            await db.SaveChangesAsync();
        }
    }
}
