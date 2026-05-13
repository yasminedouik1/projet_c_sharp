namespace ProjectManager.Models;

/// <summary>
/// Table de jointure Projet ↔ Utilisateur (membres du projet).
/// </summary>
public class ProjectUser
{
    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}
