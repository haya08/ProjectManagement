using ProjectManagement.DTOs;
using ProjectManagement.DTOs.ProjectMembers;

namespace ProjectManagement.BL.Interfaces
{
    public interface IProjectMembers
    {
        ApiResponse AddMember(AddMemberDTO dto);
        ApiResponse GetProjectMembers(int projectId);
        ApiResponse RemoveMember(int projectId, string userId);
    }
}
