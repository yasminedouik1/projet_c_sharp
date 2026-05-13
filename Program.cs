using ProjectManager.Components;
using Microsoft.EntityFrameworkCore;
using ProjectManager.Data;
using ProjectManager.Services;
using ProjectManager.Models;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// ====================== SERVICES ======================
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddScoped<IMyProjectsService, MyProjectsService>();

// ====================== IDENTITY ======================
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login";
    options.AccessDeniedPath = "/login";
});

builder.Services.AddCascadingAuthenticationState();

// ====================== BUILD ======================
var app = builder.Build();

// ====================== SEEDING ======================
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    // Rôles Identity : Admin (back-office), User (comptes « membres » de l'application)
    foreach (var roleName in new[] { "Admin", "User" })
    {
        if (!await roleManager.RoleExistsAsync(roleName))
            await roleManager.CreateAsync(new IdentityRole(roleName));
    }

    // Ancien nom de rôle « Member » → « User » (mise à jour silencieuse des comptes existants)
    if (await roleManager.RoleExistsAsync("Member"))
    {
        var memberRole = await roleManager.FindByNameAsync("Member");
        foreach (var u in await userManager.GetUsersInRoleAsync("Member"))
        {
            await userManager.RemoveFromRoleAsync(u, "Member");
            if (!await userManager.IsInRoleAsync(u, "User"))
                await userManager.AddToRoleAsync(u, "User");
        }

        if (memberRole is not null)
            await roleManager.DeleteAsync(memberRole);
    }

    // Création du compte Admin par défaut
    var adminEmail = "admin@projectmanager.com";
    if (await userManager.FindByEmailAsync(adminEmail) == null)
    {
        var adminUser = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(adminUser, "Admin123!");
        if (result.Succeeded)
            await userManager.AddToRoleAsync(adminUser, "Admin");
    }
}

// ====================== MIDDLEWARE ======================
app.UseStaticFiles();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "UI")),
    RequestPath = "/UI"
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAuthentication();      // ← Important
app.UseAuthorization();       // ← Important
app.UseAntiforgery();

// ====================== ROUTES ======================
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// ====================== AUTH ENDPOINTS ======================

// Login
app.MapPost("/api/auth/login", async (
    [FromServices] SignInManager<IdentityUser> signInManager,
    [FromForm] string email,
    [FromForm] string password) =>
{
    var result = await signInManager.PasswordSignInAsync(email, password, isPersistent: false, lockoutOnFailure: false);

    if (result.Succeeded)
    {
        var user = await signInManager.UserManager.FindByEmailAsync(email);
        if (user is not null && await signInManager.UserManager.IsInRoleAsync(user, "Admin"))
            return Results.Redirect("/dashboard");

        return Results.Redirect("/my-projects");
    }

    return Results.Redirect("/login?error=Invalid+credentials");
}).DisableAntiforgery();

// Register
app.MapPost("/api/auth/register", async (
    [FromServices] UserManager<IdentityUser> userManager,
    [FromServices] SignInManager<IdentityUser> signInManager,
    [FromForm] string email,
    [FromForm] string fullName,
    [FromForm] string password,
    [FromForm] string confirmPassword) =>
{
    if (password != confirmPassword)
        return Results.Redirect("/register?error=Les+mots+de+passe+ne+correspondent+pas");

    var existingUser = await userManager.FindByEmailAsync(email);
    if (existingUser != null)
        return Results.Redirect("/register?error=Cet+email+existe+déjà");

    var newUser = new IdentityUser
    {
        UserName = email,
        Email = email,
        EmailConfirmed = true
    };

    var result = await userManager.CreateAsync(newUser, password);

    if (result.Succeeded)
    {
        await userManager.AddToRoleAsync(newUser, "User");
        await signInManager.SignInAsync(newUser, isPersistent: false);

        return Results.Redirect("/my-projects");
    }

    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
    return Results.Redirect($"/register?error={Uri.EscapeDataString(errors)}");
}).DisableAntiforgery();

// Logout
app.MapPost("/api/auth/logout", async ([FromServices] SignInManager<IdentityUser> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.Redirect("/login");
}).DisableAntiforgery();

app.Run();