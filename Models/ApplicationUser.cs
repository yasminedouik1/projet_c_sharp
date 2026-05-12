using Microsoft.AspNetCore.Identity;

namespace ProjectManager.Models;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
}