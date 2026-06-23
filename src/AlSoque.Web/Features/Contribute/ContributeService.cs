using System.Text.Json;
using AlSoque.Data;
using AlSoque.Data.Entities;
using AlSoque.Web.Shared.Contributions;
using Microsoft.EntityFrameworkCore;

namespace AlSoque.Web.Features.Contribute;

public class ContributeService(IDbContextFactory<AppDbContext> dbFactory)
{
    public async Task<List<Family>> GetFamiliesAsync()
    {
        using var db = await dbFactory.CreateDbContextAsync();
        return await db.Families.OrderBy(f => f.Name).ToListAsync();
    }

    public async Task<List<Scholar>> GetScholarsAsync()
    {
        using var db = await dbFactory.CreateDbContextAsync();
        return await db.Scholars.OrderBy(s => s.Name).ToListAsync();
    }

    public async Task SubmitAsync(ContributionType type, int? targetScholarId, ScholarContributionPayload payload, string userId)
    {
        using var db = await dbFactory.CreateDbContextAsync();

        db.Contributions.Add(new Contribution
        {
            Type = type,
            Status = ContributionStatus.Pending,
            TargetScholarId = targetScholarId,
            PayloadJson = JsonSerializer.Serialize(payload),
            SubmittedByUserId = userId,
        });

        await db.SaveChangesAsync();
    }
}
