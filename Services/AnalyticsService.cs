using Microsoft.EntityFrameworkCore;
using ProjectManager.Data;
using ProjectManager.Models;

namespace ProjectManager.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly AppDbContext _context;

    public AnalyticsService(AppDbContext context)
    {
        _context = context;
    }


    public async Task<int> GetCompletedTasksCountAsync(int projectId)
    {
        return await _context.Tasks
            .Where(t => t.ProjectId == projectId &&
                       t.Status == Models.TaskStatus.Done)
            .CountAsync();
    }

    public async Task<int> GetOverdueTasksCountAsync(int projectId)
    {
        return await _context.Tasks
            .Where(t => t.ProjectId == projectId &&
                       t.DueDate < DateTime.Now &&
                       t.Status != Models.TaskStatus.Done)
            .CountAsync();
    }

    public async Task<double> GetHealthScoreAsync(int projectId)
    {
        var total = await _context.Tasks
            .Where(t => t.ProjectId == projectId)
            .CountAsync();

        if (total == 0) return 0;

        var completed = await GetCompletedTasksCountAsync(projectId);
        var overdue   = await GetOverdueTasksCountAsync(projectId);

        double completionRate = (double)completed / total;
        double delayRate      = (double)overdue / total;

        double score = (completionRate * 0.6) + ((1 - delayRate) * 0.4);
        return Math.Round(score * 100, 1);
    }
   
public async Task<double> GetMemberCompletionRateAsync(int memberId)
{
    var total = await _context.Tasks
        .Where(t => t.AssignedMemberId == memberId)
        .CountAsync();

    if (total == 0) return 0;

    var done = await _context.Tasks
        .Where(t => t.AssignedMemberId == memberId &&
                   t.Status == Models.TaskStatus.Done)
        .CountAsync();

    return Math.Round((double)done / total * 100, 1);
}

public async Task<int> GetMemberWorkloadAsync(int memberId)
{
    return await _context.Tasks
        .Where(t => t.AssignedMemberId == memberId &&
                   t.Status != Models.TaskStatus.Done)
        .CountAsync();
}

public async Task<int> GetMemberOverdueCountAsync(int memberId)
{
    return await _context.Tasks
        .Where(t => t.AssignedMemberId == memberId &&
                   t.DueDate < DateTime.Now &&
                   t.Status != Models.TaskStatus.Done)
        .CountAsync();
}
}