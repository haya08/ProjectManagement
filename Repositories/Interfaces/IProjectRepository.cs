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
        void Save();
    }
}
