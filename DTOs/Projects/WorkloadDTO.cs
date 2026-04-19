namespace ProjectManagement.DTOs.Projects
{
    public class WorkloadDTO
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public int TotalTasks { get; set; }
        public int DoneTasks { get; set; }
        public int PendingTasks { get; set; }
        public int OverDueTasks { get; set; }
    }
}
