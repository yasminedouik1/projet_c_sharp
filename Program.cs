using ProjectManager.Components;
using Microsoft.EntityFrameworkCore;
using ProjectManager.Data;
using ProjectManager.Services;
using ProjectManager.Models;

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
var app = builder.Build();
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

app.Run();
