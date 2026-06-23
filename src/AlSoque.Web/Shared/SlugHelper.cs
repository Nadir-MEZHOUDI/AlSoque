using System.Text.RegularExpressions;

namespace AlSoque.Web.Shared;

public static class SlugHelper
{
    public static string Slugify(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return RandomSlug();
        }

        var slug = text.Trim().ToLowerInvariant();
        slug = Regex.Replace(slug, @"[^a-z0-9؀-ۿ]+", "-").Trim('-');
        return string.IsNullOrEmpty(slug) ? RandomSlug() : slug;
    }

    private static string RandomSlug() => $"s-{Guid.NewGuid():N}"[..12];
}
