using System.Text.Json;
using AlSoque.Data;
using AlSoque.Data.Entities;
using AlSoque.Web.Shared;
using AlSoque.Web.Shared.Contributions;
using Microsoft.EntityFrameworkCore;

namespace AlSoque.Web.Features.Admin.Contributions;

public class AdminContributionsService(IDbContextFactory<AppDbContext> dbFactory)
{
    public async Task<List<Contribution>> GetByStatusAsync(ContributionStatus status)
    {
        using var db = await dbFactory.CreateDbContextAsync();
        return await db.Contributions
            .Include(c => c.SubmittedByUser)
            .Include(c => c.TargetScholar)
            .Where(c => c.Status == status)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task ApproveAsync(int contributionId, string adminUserId)
    {
        using var db = await dbFactory.CreateDbContextAsync();
        var contribution = await db.Contributions.FirstOrDefaultAsync(c => c.Id == contributionId);
        if (contribution is null || contribution.Status != ContributionStatus.Pending)
        {
            return;
        }

        var payload = JsonSerializer.Deserialize<ScholarContributionPayload>(contribution.PayloadJson) ?? new();

        if (contribution.Type == ContributionType.NewScholar)
        {
            var baseSlug = SlugHelper.Slugify(payload.Name);
            var slug = baseSlug;
            var suffix = 1;
            while (await db.Scholars.AnyAsync(s => s.Slug == slug))
            {
                slug = $"{baseSlug}-{++suffix}";
            }

            db.Scholars.Add(new Scholar
            {
                Name = payload.Name,
                AlKunyah = payload.AlKunyah,
                AlLaqab = payload.AlLaqab,
                AlIsm = payload.AlIsm,
                AlNisbah = payload.AlNisbah,
                AlAb = payload.AlAb,
                AlNasab = payload.AlNasab,
                AlMadhab = payload.AlMadhab,
                AlAkidah = payload.AlAkidah,
                AlMawlid = payload.AlMawlid,
                AlWufat = payload.AlWufat,
                WolidaBi = payload.WolidaBi,
                TowofiyaBi = payload.TowofiyaBi,
                Biography = payload.Biography,
                FamilyId = payload.FamilyId,
                Slug = slug,
            });
        }
        else if (contribution.Type == ContributionType.EditScholar && contribution.TargetScholarId is { } targetId)
        {
            var scholar = await db.Scholars.FirstOrDefaultAsync(s => s.Id == targetId);
            if (scholar is not null)
            {
                scholar.Name = payload.Name;
                scholar.AlKunyah = payload.AlKunyah;
                scholar.AlLaqab = payload.AlLaqab;
                scholar.AlIsm = payload.AlIsm;
                scholar.AlNisbah = payload.AlNisbah;
                scholar.AlAb = payload.AlAb;
                scholar.AlNasab = payload.AlNasab;
                scholar.AlMadhab = payload.AlMadhab;
                scholar.AlAkidah = payload.AlAkidah;
                scholar.AlMawlid = payload.AlMawlid;
                scholar.AlWufat = payload.AlWufat;
                scholar.WolidaBi = payload.WolidaBi;
                scholar.TowofiyaBi = payload.TowofiyaBi;
                scholar.Biography = payload.Biography;
                scholar.FamilyId = payload.FamilyId;
            }
        }

        contribution.Status = ContributionStatus.Approved;
        contribution.ReviewedByUserId = adminUserId;
        contribution.ReviewedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
    }

    public async Task RejectAsync(int contributionId, string adminUserId, string? note)
    {
        using var db = await dbFactory.CreateDbContextAsync();
        var contribution = await db.Contributions.FirstOrDefaultAsync(c => c.Id == contributionId);
        if (contribution is null || contribution.Status != ContributionStatus.Pending)
        {
            return;
        }

        contribution.Status = ContributionStatus.Rejected;
        contribution.ReviewerNote = note;
        contribution.ReviewedByUserId = adminUserId;
        contribution.ReviewedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
    }
}
