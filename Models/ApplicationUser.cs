using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ProjectManager.Models;

/// <summary>
/// Utilisateur applicatif (Identity). Un seul modèle utilisateur : plus d’entité Member dupliquée.
/// </summary>
public class ApplicationUser : IdentityUser
{
    [StringLength(200)]
    public string? DisplayName { get; set; }

    public ICollection<ProjectUser> ProjectUsers { get; set; } = new List<ProjectUser>();
}
