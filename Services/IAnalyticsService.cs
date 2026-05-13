namespace ProjectManager.Services;

public interface IAnalyticsService
{
    Task<double> GetHealthScoreAsync(int projectId);
    Task<int> GetCompletedTasksCountAsync(int projectId);
    Task<int> GetOverdueTasksCountAsync(int projectId);

    Task<double> GetUserCompletionRateAsync(string userId);
    Task<int> GetUserWorkloadAsync(string userId);
    Task<int> GetUserOverdueCountAsync(string userId);
}
