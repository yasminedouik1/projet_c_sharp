using ProjectManager.Models;

namespace ProjectManager.Services;

public interface IAnalyticsService
{
    Task<double> GetHealthScoreAsync(int projectId);
    Task<int> GetCompletedTasksCountAsync(int projectId);
    Task<int> GetOverdueTasksCountAsync(int projectId);

    Task<double> GetMemberCompletionRateAsync(int memberId);
    Task<int> GetMemberWorkloadAsync(int memberId);
    Task<int> GetMemberOverdueCountAsync(int memberId);
}