namespace ProjectManagement.DTOs.Tasks
{
    public class CreateTaskDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int ProjectId { get; set; }
        public int? AssignedTo { get; set; }
        public DateTime DueDate { get; set; }
        public string Priority { get; set; }
        public int CreatedBy { get; set; }
    }
}
