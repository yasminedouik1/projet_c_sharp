using Microsoft.EntityFrameworkCore;
using ProjectManager.Data;
using ProjectManager.Models;

namespace ProjectManager.Services;

public class ProjectService : IProjectService
{
    private readonly AppDbContext _context;

    // Le contexte est injecté automatiquement par Blazor 
    public ProjectService(AppDbContext context)
    {
        _context = context;
    }

    // Récupérer tous les projets avec leurs tâches et membres
    public async Task<List<Project>> GetAllAsync()
    {
        return await _context.Projects
            .Include(p => p.Tasks)
            .Include(p => p.Members)
            .ToListAsync();
    }

    // Récupérer un projet par son Id
    public async Task<Project?> GetByIdAsync(int id)
    {
        return await _context.Projects
            .Include(p => p.Tasks)
            .Include(p => p.Members)
                .ThenInclude(pm => pm.Member)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    // Ajouter un nouveau projet
    public async Task AddAsync(Project project)
    {
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();
    }

    // Modifier un projet existant
    public async Task UpdateAsync(Project project)
    {
        _context.Projects.Update(project);
        await _context.SaveChangesAsync();
    }

    // Supprimer un projet par son Id
    public async Task DeleteAsync(int id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project != null)
        {
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
        }
    }

    // Recherche par mot-clé dans le nom ou la description (LINQ)
    public async Task<List<Project>> SearchAsync(string keyword)
    {
        return await _context.Projects
            .Where(p => p.Name.Contains(keyword) || 
                       (p.Description != null && p.Description.Contains(keyword)))
            .ToListAsync();
    }
}