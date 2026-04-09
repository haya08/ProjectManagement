using ProjectManagement.Models;

namespace ProjectManagement.Repositories.Interfaces
{
    public interface ITaskRepository
    {
        List<TbTask> GetAll();
        TbTask GetById(int id);
        public List<TbTask> GetByProjectId(int id);
        void Add(TbTask task);
        void Update(TbTask task);
        void Delete(TbTask task);
        void Save();
    }
}
