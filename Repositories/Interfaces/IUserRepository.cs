using ProjectManagement.Models;

namespace ProjectManagement.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<ApplicationUser> GetByEmail(string email);
        Task<ApplicationUser> GetById(string id);
        Task<List<ApplicationUser>> GetAll();
        Task Add(ApplicationUser user);
        Task Update(ApplicationUser user);
        Task Save();
    }
}
