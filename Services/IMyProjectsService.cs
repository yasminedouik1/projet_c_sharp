using ProjectManager.Models;

namespace ProjectManager.Services;

public interface IMyProjectsService
{
    Task<List<Project>> GetProjectsByUserIdAsync(string userId);

    Task<List<ProjectTask>> GetTasksByUserAndProjectAsync(string userId, int projectId);

    Task UpdateTaskStatusAsync(int taskId, Models.TaskStatus newStatus);
}
