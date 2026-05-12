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
    public async Task<List<Member>> GetMembersByProjectAsync(int projectId)
{
    return await _context.ProjectMembers
        .Where(pm => pm.ProjectId == projectId)
        .Include(pm => pm.Member)
        .Select(pm => pm.Member!)
        .OrderBy(m => m.FullName)
        .ToListAsync();
}

public async Task AddMemberToProjectAsync(int projectId, int memberId)
{
    // Vérifier si la relation existe déjà
    var exists = await _context.ProjectMembers
        .AnyAsync(pm => pm.ProjectId == projectId && pm.MemberId == memberId);

    if (exists)
        return;

    var projectMember = new ProjectMember
    {
        ProjectId = projectId,
        MemberId = memberId
    };

    _context.ProjectMembers.Add(projectMember);
    await _context.SaveChangesAsync();
}

public async Task RemoveMemberFromProjectAsync(int projectId, int memberId)
{
    var projectMember = await _context.ProjectMembers
        .FirstOrDefaultAsync(pm => pm.ProjectId == projectId && pm.MemberId == memberId);

    if (projectMember != null)
    {
        _context.ProjectMembers.Remove(projectMember);
        await _context.SaveChangesAsync();
    }
}
}