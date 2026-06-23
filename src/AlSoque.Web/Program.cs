using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AlSoque.Web.Components;
using AlSoque.Web.Components.Account;
using AlSoque.Data;
using AlSoque.Web.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseNpgsql(connectionString));
// Identity (UserStore/RoleStore) يحتاج AppDbContext كخدمة Scoped مباشرة، تُحَلّ هنا عبر المصنع نفسه.
builder.Services.AddScoped<AppDbContext>(sp => sp.GetRequiredService<IDbContextFactory<AppDbContext>>().CreateDbContext());
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddScoped<AlSoque.Web.Features.ScholarsBrowse.ScholarsBrowseService>();
builder.Services.AddScoped<AlSoque.Web.Features.ScholarProfile.ScholarProfileService>();
builder.Services.AddScoped<AlSoque.Web.Features.Books.BooksService>();
builder.Services.AddScoped<AlSoque.Web.Features.Manuscripts.ManuscriptsService>();
builder.Services.AddScoped<AlSoque.Web.Features.Contribute.ContributeService>();
builder.Services.AddScoped<AlSoque.Web.Features.Admin.Contributions.AdminContributionsService>();
builder.Services.AddScoped<AlSoque.Web.Features.Admin.Scholars.AdminScholarsService>();
builder.Services.AddScoped<AlSoque.Web.Features.Admin.Books.AdminBooksService>();
builder.Services.AddScoped<AlSoque.Web.Features.Admin.Manuscripts.AdminManuscriptsService>();
builder.Services.AddScoped<AlSoque.Web.Features.Admin.Families.AdminFamiliesService>();
builder.Services.AddScoped<AlSoque.Web.Features.Admin.Specializations.AdminSpecializationsService>();
builder.Services.AddScoped<AlSoque.Web.Features.Admin.Users.AdminUsersService>();

builder.Services.AddIdentityCore<ApplicationUser>(options =>
    {
        // تسجيل عام بلا تأكيد بريد إلزامي — قرار معتمد لمرحلة MVP
        options.SignIn.RequireConfirmedAccount = false;
        options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

if (!builder.Environment.IsDevelopment())
{
    // يحفظ مفاتيح Data Protection (كوكيز الهوية وAntiforgery) على القرص داخل الحاوية،
    // عبر volume "dpkeys" — بدونه تُفقَد كل الجلسات عند كل إعادة تشغيل للحاوية.
    builder.Services.AddDataProtection()
        .SetApplicationName("AlSoque")
        .PersistKeysToFileSystem(new DirectoryInfo("/app/dpkeys"));
}

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
    await using var db = await dbFactory.CreateDbContextAsync();
    await db.Database.MigrateAsync();

    await IdentitySeeder.SeedAsync(scope.ServiceProvider, app.Configuration);
}

if (app.Environment.IsDevelopment())
{
    await AlSoque.Web.Seed.DemoDataSeeder.SeedAsync(app.Services);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();
