using ProjectManager.Models;

namespace ProjectManager.Services;

public interface IMyProjectsService
{
    Task<List<Member>> GetAllMembersAsync();
    Task<Member?> GetMemberByIdAsync(int memberId);
    Task<List<Project>> GetProjectsByMemberAsync(int memberId);
    Task<List<ProjectTask>> GetTasksByMemberAndProjectAsync(int memberId, int projectId);
    Task UpdateTaskStatusAsync(int taskId, Models.TaskStatus newStatus);
    Task<int> GetProjectMemberCountAsync(int memberId);
    Task<Member?> GetMemberByEmailAsync(string email);
}