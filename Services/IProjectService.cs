using ProjectManager.Models;

namespace ProjectManager.Services;

public interface IProjectService
{
    Task<List<Project>> GetAllAsync();
    Task<Project?> GetByIdAsync(int id);
    Task AddAsync(Project project);
    Task UpdateAsync(Project project);
    Task DeleteAsync(int id);
    Task<List<Project>> SearchAsync(string keyword);
}