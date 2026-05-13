using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectManager.Data;
using ProjectManager.Models;

namespace ProjectManager.Services;

public class UserProjectService : IUserProjectService
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserProjectService(AppDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<IReadOnlyList<ApplicationUser>> GetDirectoryUsersAsync()
    {
        var admins = await _userManager.GetUsersInRoleAsync("Admin");
        var adminIds = admins.Select(a => a.Id).ToHashSet();

        var users = await _userManager.GetUsersInRoleAsync("User");
        return users
            .Where(u => !adminIds.Contains(u.Id))
            .OrderBy(u => u.DisplayName ?? u.Email ?? u.UserName)
            .ToList();
    }

    public async Task<IReadOnlyList<ApplicationUser>> GetUsersInProjectAsync(int projectId)
    {
        var list = await _db.ProjectUsers
            .AsNoTracking()
            .Where(pu => pu.ProjectId == projectId)
            .Include(pu => pu.User)
            .Select(pu => pu.User!)
            .ToListAsync();

        return list
            .OrderBy(u => u.DisplayName ?? u.Email)
            .ToList();
    }

    public async Task AddUserToProjectAsync(int projectId, string userId)
    {
        if (await _db.ProjectUsers.AnyAsync(pu => pu.ProjectId == projectId && pu.UserId == userId))
            return;

        _db.ProjectUsers.Add(new ProjectUser
        {
            ProjectId = projectId,
            UserId = userId,
            JoinedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();
    }

    public async Task RemoveUserFromProjectAsync(int projectId, string userId)
    {
        var link = await _db.ProjectUsers
            .FirstOrDefaultAsync(pu => pu.ProjectId == projectId && pu.UserId == userId);

        if (link is null)
            return;

        _db.ProjectUsers.Remove(link);
        await _db.SaveChangesAsync();
    }

    public Task<bool> IsUserInProjectAsync(int projectId, string userId) =>
        _db.ProjectUsers.AnyAsync(pu => pu.ProjectId == projectId && pu.UserId == userId);

    public Task<int> GetUserProjectCountAsync(string userId) =>
        _db.ProjectUsers.CountAsync(pu => pu.UserId == userId);

    public async Task<IReadOnlyList<Project>> GetProjectsForUserAsync(string userId)
    {
        return await _db.ProjectUsers
            .AsNoTracking()
            .Where(pu => pu.UserId == userId)
            .Select(pu => pu.Project!)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }
}
