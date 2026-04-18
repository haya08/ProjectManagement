using ProjectManagement.Models;
using ProjectManagement.Repositories.Interfaces;

namespace ProjectManagement.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly ProjectManagementContext _context;

        public UserRepository(ProjectManagementContext context)
        {
            _context = context;
        }

        // get all users
        public List<ApplicationUser> GetAll()
        {
            return _context.Users.ToList();
        }
    }
}
