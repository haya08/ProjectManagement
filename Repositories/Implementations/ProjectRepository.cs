using ProjectManagement.DTOs.Tasks;
using ProjectManagement.Models;
using ProjectManagement.Repositories.Interfaces;

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
            return _context.TbProjects.ToList();
        }

        public TbProject GetById(int id)
        {
            return _context.TbProjects.FirstOrDefault(t => t.Id == id);
        }

        public void Add(TbProject project)
        {
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

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
