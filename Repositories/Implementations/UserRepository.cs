using Microsoft.EntityFrameworkCore;
using ProjectManagement.Models;
using ProjectManagement.Repositories.Interfaces;

namespace ProjectManagement.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApplicationUser> GetByEmail(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<ApplicationUser> GetById(string id)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<List<ApplicationUser>> GetAll()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task Add(ApplicationUser user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task Update(ApplicationUser user)
        {
            _context.Users.Update(user);
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}
