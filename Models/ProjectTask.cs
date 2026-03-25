using System.ComponentModel.DataAnnotations;

namespace ProjectManager.Models;

public class ProjectTask
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Le titre est obligatoire.")]
    [StringLength(200, ErrorMessage = "200 caractères maximum.")]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required(ErrorMessage = "La date limite est obligatoire.")]
    public DateTime DueDate { get; set; }

    public Models.TaskStatus Status { get; set; } = Models.TaskStatus.Todo;

    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public int? AssignedMemberId { get; set; }
    public Member? AssignedMember { get; set; }
}

public enum TaskStatus { Todo, InProgress, Done }