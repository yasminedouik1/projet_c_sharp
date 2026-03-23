namespace ProjectManager.Models;

public class Project
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; } = DateTime.Now;
    public DateTime DueDate { get; set; }
    public ProjectStatus Status { get; set; } = ProjectStatus.Active;

    public ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
    public ICollection<ProjectMember> Members { get; set; } = new List<ProjectMember>();
}

public enum ProjectStatus { Active, Completed, OnHold, Cancelled }