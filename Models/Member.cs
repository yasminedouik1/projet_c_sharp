using System.ComponentModel.DataAnnotations;

namespace ProjectManager.Models;

public class Member
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Le nom est obligatoire.")]
    [StringLength(100, ErrorMessage = "100 caractères maximum.")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "L'email est obligatoire.")]
    [EmailAddress(ErrorMessage = "Format email invalide.")]
    public string Email { get; set; } = string.Empty;

    [StringLength(50)]
    public string? Role { get; set; }

    public ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
    public ICollection<ProjectMember> ProjectMemberships { get; set; } = new List<ProjectMember>();
}