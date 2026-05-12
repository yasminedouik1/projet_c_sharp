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

    // Récupère tous les membres pour le dropdown
    public async Task<List<Member>> GetAllMembersAsync()
    {
        return await _db.Members
            .OrderBy(m => m.FullName)
            .ToListAsync();
    }

    // Récupère un membre par son Id
    public async Task<Member?> GetMemberByIdAsync(int memberId)
    {
        return await _db.Members.FindAsync(memberId);
    }

    // Projets dont le membre fait partie
    public async Task<List<Project>> GetProjectsByMemberAsync(int memberId)
    {
        return await _db.ProjectMembers
            .Where(pm => pm.MemberId == memberId)
            .Include(pm => pm.Project)
                .ThenInclude(p => p.Tasks)
            .Select(pm => pm.Project)
            .ToListAsync();
    }

    // Tâches assignées au membre dans un projet donné
    public async Task<List<ProjectTask>> GetTasksByMemberAndProjectAsync(int memberId, int projectId)
    {
        return await _db.Tasks
            .Where(t => t.ProjectId == projectId && t.AssignedMemberId == memberId)
            .OrderBy(t => t.DueDate)
            .ToListAsync();
    }

    // Mise à jour du statut (appelée par le drag & drop)
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