using ProjectManager.Models;

namespace ProjectManager.Services;

/// <summary>
/// Utilisateurs « annuaire » (rôle User, hors Admin) et affectation aux projets.
/// </summary>
public interface IUserProjectService
{
    /// <summary>Comptes avec le rôle User, exclus ceux qui ont aussi le rôle Admin.</summary>
    Task<IReadOnlyList<ApplicationUser>> GetDirectoryUsersAsync();

    Task<IReadOnlyList<ApplicationUser>> GetUsersInProjectAsync(int projectId);

    Task AddUserToProjectAsync(int projectId, string userId);

    Task RemoveUserFromProjectAsync(int projectId, string userId);

    Task<bool> IsUserInProjectAsync(int projectId, string userId);

    Task<int> GetUserProjectCountAsync(string userId);

    /// <summary>Projets auxquels l’utilisateur est lié (table <see cref="ProjectUser"/>).</summary>
    Task<IReadOnlyList<Project>> GetProjectsForUserAsync(string userId);
}
