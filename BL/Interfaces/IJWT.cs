using ProjectManagement.Models;

namespace ProjectManagement.BL.Interfaces
{
    public interface IJWT
    {
        string GenerateToken(ApplicationUser user);
    }
}
