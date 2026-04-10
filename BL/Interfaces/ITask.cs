using ProjectManagement.DTOs;
using ProjectManagement.DTOs.Tasks;

namespace ProjectManagement.BL.Interfaces
{
    public interface ITask
    {
        public ApiResponse GetAllTasks(TaskQueryDTO query);
        public ApiResponse GetTaskById(int id);
        public List<TasksDTO> GetTasksByProjectId(int id);
        public ApiResponse CreateTask(CreateTaskDTO dto);
        public ApiResponse UpdateTask(UpdateTaskDTO dto);
        public ApiResponse DeleteTask(int id);
    }
}
