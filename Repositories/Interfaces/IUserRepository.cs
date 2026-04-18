using ProjectManagement.Models;

namespace ProjectManagement.Repositories.Interfaces
{
    public interface IUserRepository
    {
        List<ApplicationUser> GetAll();
    }
}
