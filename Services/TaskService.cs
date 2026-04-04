using Microsoft.EntityFrameworkCore;
using ProjectManager.Data;
using ProjectManager.Models;

namespace ProjectManager.Services;

public class TaskService : ITaskService
{
    private readonly AppDbContext _context;

    public TaskService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProjectTask>> GetAllByProjectAsync(int projectId)
    {
        return await _context.Tasks
            .Include(t => t.AssignedMember)
            .Where(t => t.ProjectId == projectId)
            .ToListAsync();
    }

    public async Task<ProjectTask?> GetByIdAsync(int id)
    {
        return await _context.Tasks
            .Include(t => t.AssignedMember)
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task AddAsync(ProjectTask task)
    {
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ProjectTask task)
    {
        _context.Tasks.Update(task);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task != null)
        {
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<ProjectTask>> GetOverdueTasksAsync()
    {
        return await _context.Tasks
            .Include(t => t.Project)
            .Include(t => t.AssignedMember)
            .Where(t => t.DueDate < DateTime.Now && 
                       t.Status != Models.TaskStatus.Done)
            .ToListAsync();
    }
}