namespace ProjectManager.Models;

public class ProjectMember
{
    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public int MemberId { get; set; }
    public Member Member { get; set; } = null!;

    public DateTime JoinedAt { get; set; } = DateTime.Now;
}