namespace ProjectManager.Models;

public class ProjectTask
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime DueDate { get; set; }
    public TaskStatus Status { get; set; } = TaskStatus.Todo;

    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public int? AssignedMemberId { get; set; }
    public Member? AssignedMember { get; set; }
}

public enum TaskStatus { Todo, InProgress, Done }