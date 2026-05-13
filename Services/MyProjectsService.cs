using Microsoft.EntityFrameworkCore;
using ProjectManager.Data;
using ProjectManager.Models;

namespace ProjectManager.Services;

public class MyProjectsService : IMyProjectsService
{
    private readonly AppDbContext _db;

    public MyProjectsService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Project>> GetProjectsByUserIdAsync(string userId)
    {
        return await _db.Projects
            .AsNoTracking()
            .Where(p => p.ProjectUsers.Any(pu => pu.UserId == userId))
            .Include(p => p.Tasks)
            .ToListAsync();
    }

    public async Task<List<ProjectTask>> GetTasksByUserAndProjectAsync(string userId, int projectId)
    {
        return await _db.Tasks
            .Where(t => t.ProjectId == projectId && t.AssignedUserId == userId)
            .OrderBy(t => t.DueDate)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task UpdateTaskStatusAsync(int taskId, Models.TaskStatus newStatus)
    {
        var task = await _db.Tasks.FindAsync(taskId);
        if (task is not null)
        {
            task.Status = newStatus;
            await _db.SaveChangesAsync();
        }
    }
}
