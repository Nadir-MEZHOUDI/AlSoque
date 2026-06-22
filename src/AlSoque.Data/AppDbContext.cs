using AlSoque.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AlSoque.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Family> Families => Set<Family>();
    public DbSet<Scholar> Scholars => Set<Scholar>();
    public DbSet<Specialization> Specializations => Set<Specialization>();
    public DbSet<ScholarRelation> ScholarRelations => Set<ScholarRelation>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ScholarRelation>(relation =>
        {
            relation.HasOne(r => r.Teacher)
                .WithMany(s => s.StudentLinks)
                .HasForeignKey(r => r.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            relation.HasOne(r => r.Student)
                .WithMany(s => s.TeacherLinks)
                .HasForeignKey(r => r.StudentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Scholar>()
            .HasIndex(s => s.Slug)
            .IsUnique();

        builder.Entity<Family>()
            .HasIndex(f => f.Slug)
            .IsUnique();
    }
}
