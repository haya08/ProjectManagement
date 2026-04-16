namespace ProjectManagement.DTOs.ProjectMembers
{
    public class AddMemberDTO
    {
        public int ProjectId { get; set; }
        public string UserId { get; set; }
        public string Role { get; set; } // ProjectManager / TeamMember
    }
}
