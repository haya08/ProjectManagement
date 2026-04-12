using Microsoft.EntityFrameworkCore;
using ProjectManagement.Models;
using ProjectManagement.Repositories.Interfaces;
using System.Linq;

namespace ProjectManagement.Repositories.Implementations
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly ProjectManagementContext _context;

        public ProjectRepository(ProjectManagementContext context)
        {
            _context = context;
        }

        public List<TbProject> GetAll()
        {
            return _context.TbProjects
                .Include(p => p.TbProjectMembers)
                .ToList();
        }

        public TbProject GetById(int id)
        {
            return _context.TbProjects
                .Include(p => p.TbProjectMembers)
                .FirstOrDefault(p => p.Id == id);
        }

        public void Add(TbProject project)
        {
            project.CreatedAt = DateTime.Now;

            _context.TbProjects.Add(project);
        }

        public void Update(TbProject project)
        {
            _context.TbProjects.Update(project);
        }

        public void Delete(TbProject project)
        {
            _context.TbProjects.Remove(project);
        }

        public void AddMember(TbProjectMember member)
        {
            member.JoinedAt = DateTime.Now;

            _context.TbProjectMembers.Add(member);
        }

        public List<TbProjectMember> GetMembersByProjectId(int projectId)
        {
            return _context.TbProjectMembers
                .Include(m => m.User)
                .Where(m => m.ProjectId == projectId)
                .ToList();
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void DeleteMember(int projectId, int userId)
        {
            var member = _context.TbProjectMembers
                .FirstOrDefault(m => m.ProjectId == projectId 
                                && m.UserId == userId);

            if (member != null)
            {
                _context.TbProjectMembers.Remove(member);
            }
        }

        public void UpdateMemberRole(int projectId, int userId, string role)
        {
            var member = _context.TbProjectMembers
                .FirstOrDefault(m => m.ProjectId == projectId 
                                && m.UserId == userId);

            if (member != null)
            {
                member.Role = role;
            }
        }

        public List<TbProject> GetProjectsByManager(int userId)
        {
            return _context.TbProjectMembers
                .Where(pm => pm.UserId == userId)
                .Select(pm => pm.Project)
                .ToList();
        }
    }
}