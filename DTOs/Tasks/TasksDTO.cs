namespace ProjectManagement.DTOs.Tasks
{
    public class TasksDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public string AssignedUserName { get; set; }
        public DateTime? DueDate { get; set; }
    }
}
