using Microsoft.EntityFrameworkCore;
using ProjectManagement.Models;
using ProjectManagement.Repositories.Interfaces;

namespace ProjectManagement.Repositories.Implementations
{
    public class ProjectMemberRepository : IProjectMemberRepository
    {
        private readonly ProjectManagementContext _context;

        public ProjectMemberRepository(ProjectManagementContext context)
        {
            _context = context;
        }

        public void Add(TbProjectMember member)
            => _context.TbProjectMembers.Add(member);

        public List<TbProjectMember> GetByProjectId(int projectId)
            => _context.TbProjectMembers
                .Where(m => m.ProjectId == projectId)
                .ToList();

        public TbProjectMember GetByUserAndProject(string userId, int projectId)
            => _context.TbProjectMembers
                .FirstOrDefault(m => m.UserId == userId && m.ProjectId == projectId);

        public List<TbProjectMember> GetByUser(string userId)
            => _context.TbProjectMembers
                .Include(m => m.Project) 
                .Where(m => m.UserId == userId)
                .ToList();

        public void Delete(TbProjectMember member)
            => _context.TbProjectMembers.Remove(member);

        public void Save()
            => _context.SaveChanges();
    }
}
