using ProjectManagement.DTOs.Projects;
using ProjectManagement.DTOs.Tasks;

namespace ProjectManagement.DTOs.Users
{
    public class UserDashboardDTO
    {
        //public List<TasksDTO> MyTasks { get; set; }
        //public List<ProjectsDTO> MyProjects { get; set; }
        //public UserStatsDTO Stats { get; set; }
        //public ProjectProgressDTO ProjectProgress { get; set; }

        public int TotalProjects { get; set; }
        public int TotalTasks { get; set; }
        public int DoneTasks { get; set; }
        public int InProgressTasks { get; set; }
        public int TodoTasks { get; set; }
        public int OverDueTasks { get; set; }
        public double PlannedPercentage { get; set; }
        public double ActualPercentage { get; set; }
    }
}
