using ProjectManagement.DTOs.Tasks;

namespace ProjectManagement.BL.Interfaces
{
    public interface ITask
    {
        public List<TasksDTO> GetAllTasks();
        public TasksDTO GetTaskById(int id);
        public List<TasksDTO> GetTasksByProjectId(int id);
        public void CreateTask(CreateTaskDTO dto);
        public void UpdateTask(UpdateTaskDTO dto);
        public void DeleteTask(int id);
    }
}
