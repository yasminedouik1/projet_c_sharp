using Microsoft.EntityFrameworkCore;
using ProjectManager.Data;
using ProjectManager.Models;

namespace ProjectManager.Services;

public class MemberService : IMemberService
{
    private readonly AppDbContext _context;

    public MemberService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Member>> GetAllAsync()
    {
        return await _context.Members.ToListAsync();
    }

    public async Task<Member?> GetByIdAsync(int id)
    {
        return await _context.Members
            .Include(m => m.Tasks)
            .Include(m => m.ProjectMemberships)
                .ThenInclude(pm => pm.Project)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task AddAsync(Member member)
    {
        _context.Members.Add(member);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Member member)
    {
        _context.Members.Update(member);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var member = await _context.Members.FindAsync(id);
        if (member != null)
        {
            _context.Members.Remove(member);
            await _context.SaveChangesAsync();
        }
    }

    public async Task AddToProjectAsync(int memberId, int projectId)
    {
        // Vérifier qu'il n'est pas déjà dans ce projet
        var exists = await _context.ProjectMembers
            .AnyAsync(pm => pm.MemberId == memberId && pm.ProjectId == projectId);

        if (!exists)
        {
            _context.ProjectMembers.Add(new ProjectMember
            {
                MemberId = memberId,
                ProjectId = projectId
            });
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<Member>> GetByProjectAsync(int projectId)
    {
        return await _context.ProjectMembers
            .Where(pm => pm.ProjectId == projectId)
            .Select(pm => pm.Member)
            .ToListAsync();
    }
}