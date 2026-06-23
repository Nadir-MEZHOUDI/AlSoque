using AlSoque.Data;
using AlSoque.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AlSoque.Web.Features.Books;

public class BooksService(IDbContextFactory<AppDbContext> dbFactory)
{
    public async Task<List<Book>> GetAllAsync()
    {
        using var db = await dbFactory.CreateDbContextAsync();
        return await db.Books
            .Include(b => b.Scholar)
            .OrderBy(b => b.Title)
            .ToListAsync();
    }

    public async Task<Book?> GetBySlugAsync(string slug)
    {
        using var db = await dbFactory.CreateDbContextAsync();
        return await db.Books
            .Include(b => b.Scholar)
            .FirstOrDefaultAsync(b => b.Slug == slug);
    }
}
