using AlSoque.Data;
using AlSoque.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AlSoque.Web.Features.Admin.Books;

public class AdminBooksService(IDbContextFactory<AppDbContext> dbFactory)
{
    public async Task<List<Book>> GetAllAsync()
    {
        using var db = await dbFactory.CreateDbContextAsync();
        return await db.Books.Include(b => b.Scholar).OrderBy(b => b.Title).ToListAsync();
    }

    public async Task<Book?> GetByIdAsync(int id)
    {
        using var db = await dbFactory.CreateDbContextAsync();
        return await db.Books.FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<List<Scholar>> GetScholarsAsync()
    {
        using var db = await dbFactory.CreateDbContextAsync();
        return await db.Scholars.OrderBy(s => s.Name).ToListAsync();
    }

    public async Task<bool> IsSlugTakenAsync(string slug, int? excludeId)
    {
        using var db = await dbFactory.CreateDbContextAsync();
        return await db.Books.AnyAsync(b => b.Slug == slug && b.Id != (excludeId ?? 0));
    }

    public async Task SaveAsync(Book input)
    {
        using var db = await dbFactory.CreateDbContextAsync();

        if (input.Id == 0)
        {
            db.Books.Add(new Book
            {
                Title = input.Title,
                Description = input.Description,
                Status = input.Status,
                PublicationYear = input.PublicationYear,
                Slug = input.Slug,
                ScholarId = input.ScholarId,
            });
        }
        else
        {
            var book = await db.Books.FirstAsync(b => b.Id == input.Id);
            book.Title = input.Title;
            book.Description = input.Description;
            book.Status = input.Status;
            book.PublicationYear = input.PublicationYear;
            book.Slug = input.Slug;
            book.ScholarId = input.ScholarId;
        }

        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        using var db = await dbFactory.CreateDbContextAsync();
        var book = await db.Books.FindAsync(id);
        if (book is not null)
        {
            db.Books.Remove(book);
            await db.SaveChangesAsync();
        }
    }
}
