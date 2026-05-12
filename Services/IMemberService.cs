using ProjectManager.Models;

namespace ProjectManager.Services;

public interface IMemberService
{
    Task<List<Member>> GetAllAsync();
    Task<Member?> GetByIdAsync(int id);
    Task AddAsync(Member member);
    Task UpdateAsync(Member member);
    Task DeleteAsync(int id);
    Task AddToProjectAsync(int memberId, int projectId);
    Task<List<Member>> GetByProjectAsync(int projectId);
    Task<List<Member>> GetMembersByProjectAsync(int projectId);
    Task AddMemberToProjectAsync(int projectId, int memberId);
Task RemoveMemberFromProjectAsync(int projectId, int memberId);
}