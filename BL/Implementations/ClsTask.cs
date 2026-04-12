using ProjectManagement.BL.Interfaces;
using ProjectManagement.DTOs.Tasks;
using ProjectManagement.Models;
using ProjectManagement.Repositories.Interfaces;

namespace ProjectManagement.BL.Implementations
{
    public class ClsTask : ITask
    {
        private readonly ITaskRepository _repo;

        public ClsTask(ITaskRepository repo)
        {
            _repo = repo;
        }

        public List<TasksDTO> GetAllTasks()
        {
            var tasks = _repo.GetAll();

            return tasks.Select(t => new TasksDTO
            {
                Id = t.Id,
                Title = t.Title,
                Status = t.Status,
                Priority = t.Priority,
                AssignedUserName = t.AssignedToNavigation?.Name,
                DueDate = (DateTime)t.DueDate
            }).ToList();
        }

        public TasksDTO GetTaskById(int id)
        {
            var task = _repo.GetById(id);

            if (task == null) return null;

            return new TasksDTO
            {
                Id = task.Id,
                Title = task.Title,
                Status = task.Status,
                Priority = task.Priority,
                AssignedUserName = task.AssignedToNavigation?.Name,
                DueDate = (DateTime)task.DueDate
            };
        }

        public List<TasksDTO> GetTasksByProjectId(int id)
        {
            var tasks = _repo.GetByProjectId(id);

            return tasks.Select(t => new TasksDTO
            {
                Id = t.Id,
                Title = t.Title,
                Status = t.Status,
                Priority = t.Priority,
                AssignedUserName = t.AssignedToNavigation?.Name,
                DueDate = t.DueDate
            }).ToList();
        }

        public void CreateTask(CreateTaskDTO dto)
        {
            var task = new TbTask
            {
                Title = dto.Title,
                Description = dto.Description,
                ProjectId = dto.ProjectId,
                AssignedTo = dto.AssignedTo,
                DueDate = dto.DueDate,
                Priority = dto.Priority,
                Status = "todo"
            };

            _repo.Add(task);
            _repo.Save();
        }

        public void UpdateTask(UpdateTaskDTO dto)
{
    TbTask? task = _repo.GetById(dto.Id);

    if(task == null) return;
    task.Title = dto.Title;
    task.Description = dto.Description;
    task.Status = dto.Status;
    task.Priority = dto.Priority;
    task.DueDate = dto.DueDate;

    _repo.Update(task);
    _repo.Save();
}

        public void DeleteTask(int id)
        {
            var task = _repo.GetById(id);
            if (task == null) return;

            _repo.Delete(task);
            _repo.Save();
        }
    }
}
