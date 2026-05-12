using ProjectManager.Components;
using Microsoft.EntityFrameworkCore;
using ProjectManager.Data;
using ProjectManager.Services;
using ProjectManager.Models;
using Microsoft.Extensions.FileProviders; 
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddScoped<IMyProjectsService, MyProjectsService>();


builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddCascadingAuthenticationState();

var app = builder.Build();
// Servir les fichiers statiques de wwwroot (existant)
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    if (!await roleManager.RoleExistsAsync("Admin"))
        await roleManager.CreateAsync(new IdentityRole("Admin"));

    if (await userManager.FindByEmailAsync("admin@data.com") == null)
    {
        var adminUser = new IdentityUser { UserName = "admin@data.com", Email = "admin@data.com" };
        var result = await userManager.CreateAsync(adminUser, "Admin123!");

        if (result.Succeeded)
            await userManager.AddToRoleAsync(adminUser, "Admin");
    }
}

app.UseStaticFiles();

// Servir les fichiers du dossier UI (NOUVEAU)
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "UI")),
    RequestPath = "/UI"
});
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();

    if (!context.Members.Any())
    {
        context.Members.AddRange(
            new Member { FullName = "Ghada Ben Mansour", Email = "ghada@mail.com", Role = "Développeuse" },
            new Member { FullName = "Yasmine Douik",   Email = "yasmine@mail.com",   Role = "Chef de projet" }
        );
        context.Projects.Add(new Project
        {
            Name        = "Projet Demo",
            Description = "Projet de démonstration",
            StartDate   = DateTime.Now.AddDays(-10),
            DueDate     = DateTime.Now.AddDays(20),
            Status      = ProjectStatus.Active
        });
        context.SaveChanges();
    }
}
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
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


app.MapPost("/api/auth/login", async (
    [FromServices] SignInManager<IdentityUser> signInManager,
    [FromForm] string email, 
    [FromForm] string password) =>
{
    var result = await signInManager.PasswordSignInAsync(email, password, isPersistent: false, lockoutOnFailure: false);
    
    if (result.Succeeded) return Results.Redirect("/dashboard");
    
    return Results.Redirect("/login?error=Invalid+credentials");
}).DisableAntiforgery(); 

app.MapPost("/api/auth/logout", async ([FromServices] SignInManager<IdentityUser> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.Redirect("/");
}).DisableAntiforgery();

app.Run(); 