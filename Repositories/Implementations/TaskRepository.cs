using Microsoft.EntityFrameworkCore;
using ProjectManagement.DTOs.Tasks;
using ProjectManagement.Models;
using ProjectManagement.Repositories.Interfaces;

namespace ProjectManagement.Repositories.Implementations
{
    public class TaskRepository : ITaskRepository
    {
        private readonly ProjectManagementContext _context;

        public TaskRepository(ProjectManagementContext context)
        {
            _context = context;
        }

        public List<TbTask> GetAll(TaskQueryDTO query)
        {
            var tasks = _context.TbTasks.Include(t => t.AssignedToNavigation).AsQueryable();

            // Filtering
            if (!string.IsNullOrEmpty(query.Status))
            {
                tasks = tasks.Where(t => t.Status == query.Status);
            }

            if (!string.IsNullOrEmpty(query.Priority))
            {
                tasks = tasks.Where(t => t.Priority == query.Priority);
            }

            // Pagination
            tasks = tasks
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize);

            return tasks.ToList();
        }

        public TbTask GetById(int id)
        {
            return _context.TbTasks.FirstOrDefault(t => t.Id == id);
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
