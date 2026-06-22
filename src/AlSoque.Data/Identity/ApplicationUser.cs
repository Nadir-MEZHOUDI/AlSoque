using Microsoft.AspNetCore.Identity;

namespace AlSoque.Data;

public class ApplicationUser : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;

    public string? Bio { get; set; }
}
