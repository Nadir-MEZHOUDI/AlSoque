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
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Manuscript> Manuscripts => Set<Manuscript>();
    public DbSet<Contribution> Contributions => Set<Contribution>();

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

        builder.Entity<Book>(book =>
        {
            book.HasOne(b => b.Scholar)
                .WithMany(s => s.Books)
                .HasForeignKey(b => b.ScholarId);
            book.HasIndex(b => b.Slug).IsUnique();
        });

        builder.Entity<Manuscript>(manuscript =>
        {
            manuscript.HasOne(m => m.Scholar)
                .WithMany(s => s.Manuscripts)
                .HasForeignKey(m => m.ScholarId)
                .OnDelete(DeleteBehavior.SetNull);
            manuscript.HasOne(m => m.Family)
                .WithMany(f => f.Manuscripts)
                .HasForeignKey(m => m.FamilyId)
                .OnDelete(DeleteBehavior.SetNull);
            manuscript.HasIndex(m => m.Slug).IsUnique();
        });

        builder.Entity<Contribution>(contribution =>
        {
            contribution.HasOne(c => c.TargetScholar)
                .WithMany()
                .HasForeignKey(c => c.TargetScholarId)
                .OnDelete(DeleteBehavior.SetNull);

            contribution.HasOne(c => c.SubmittedByUser)
                .WithMany()
                .HasForeignKey(c => c.SubmittedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            contribution.HasOne(c => c.ReviewedByUser)
                .WithMany()
                .HasForeignKey(c => c.ReviewedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
