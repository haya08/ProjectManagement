using ProjectManagement.Models;

namespace ProjectManagement.Repositories.Interfaces
{
    public interface IProjectRepository
    {
        List<TbProject> GetAll();

        TbProject GetById(int id);

        void Add(TbProject project);

        void Update(TbProject project);

        void Delete(TbProject project);

        void AddMember(TbProjectMember member);

        List<TbProjectMember> GetMembersByProjectId(int projectId);

        void Save();

        void DeleteMember(int projectId, int userId);

        void UpdateMemberRole(int projectId, int userId, string role);

        List<TbProject> GetProjectsByManager(int userId);
        

    }
}