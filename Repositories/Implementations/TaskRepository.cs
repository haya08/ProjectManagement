using Microsoft.EntityFrameworkCore;
using ProjectManagement.Models;
using ProjectManagement.Repositories.Interfaces;
using System.Linq;

namespace ProjectManagement.Repositories.Implementations
{
    public class TaskRepository : ITaskRepository
    {
        private readonly ProjectManagementContext _context;

        public TaskRepository(ProjectManagementContext context)
        {
            _context = context;
        }

        public List<TbTask> GetAll()
        {
            return _context.TbTasks.ToList();
        }

        public TbTask? GetById(int id)
{
        return _context.TbTasks
        .FirstOrDefault(t => t.Id == id);
}

        public List<TbTask> GetByProjectId(int id)
        {
            //return _context.TbTasks.Where(t => t.ProjectId == id).ToList();
            return _context.TbTasks
                .Include(t => t.AssignedToNavigation)
                .Where(t => t.ProjectId == id)
                .ToList();
        }

        public void Add(TbTask task)
        {
            _context.TbTasks.Add(task);
        }

        public void Update(TbTask task)
        {
            _context.TbTasks.Update(task);
        }

        public void Delete(TbTask task)
        {
            _context.TbTasks.Remove(task);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
