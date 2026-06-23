using AlSoque.Data;
using AlSoque.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AlSoque.Web.Features.ScholarProfile;

public class ScholarProfileService(IDbContextFactory<AppDbContext> dbFactory)
{
    public async Task<Scholar?> GetBySlugAsync(string slug)
    {
        using var db = await dbFactory.CreateDbContextAsync();

        return await db.Scholars
            .Include(s => s.Family)
            .Include(s => s.Specializations)
            .Include(s => s.TeacherLinks).ThenInclude(r => r.Teacher)
            .Include(s => s.StudentLinks).ThenInclude(r => r.Student)
            .Include(s => s.Books)
            .Include(s => s.Manuscripts)
            .FirstOrDefaultAsync(s => s.Slug == slug);
    }
}
