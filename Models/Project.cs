using System.ComponentModel.DataAnnotations;

namespace ProjectManager.Models;

public class Project
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Le nom est obligatoire.")]
    [StringLength(100, ErrorMessage = "100 caractères maximum.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    public DateTime StartDate { get; set; } = DateTime.Now;

    [Required(ErrorMessage = "La date limite est obligatoire.")]
    public DateTime DueDate { get; set; }

    public ProjectStatus Status { get; set; } = ProjectStatus.Active;

    public ICollection<ProjectTask> Tasks   { get; set; } = new List<ProjectTask>();
    public ICollection<ProjectMember> Members { get; set; } = new List<ProjectMember>();
}

public enum ProjectStatus { Active, Completed, OnHold, Cancelled }