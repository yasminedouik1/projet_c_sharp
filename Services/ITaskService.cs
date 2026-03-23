using ProjectManager.Models;

namespace ProjectManager.Services;

public interface ITaskService
{
    Task<List<ProjectTask>> GetAllByProjectAsync(int projectId);
    Task<ProjectTask?> GetByIdAsync(int id);
    Task AddAsync(ProjectTask task);
    Task UpdateAsync(ProjectTask task);
    Task DeleteAsync(int id);
    Task<List<ProjectTask>> GetOverdueTasksAsync();
}