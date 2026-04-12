using ProjectManagement.DTOs.Projects;

namespace ProjectManagement.BL.Interfaces
{
    public interface IProjectBL
    {
        void CreateProject(CreateProjectDTO dto);

        List<ProjectDTO> GetAllProjects();

        ProjectDTO GetProjectById(int id);

        void UpdateProject(UpdateProjectDTO dto);

        void DeleteProject(int id);

        void AddMember(int projectId, int userId, string role);

        List<ProjectMemberDTO> GetMembersByProjectId(int projectId);

        void DeleteMember(int projectId, int userId);

        void UpdateMemberRole(int projectId, int userId, string role);

        List<ProjectDTO> GetProjectsByManager(int userId);

    }
}