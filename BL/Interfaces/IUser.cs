using ProjectManagement.DTOs;
using ProjectManagement.DTOs.Users;

namespace ProjectManagement.BL.Interfaces
{
    public interface IUser
    {
        Task<ApiResponse> Register(RegisterDTO dto);
        Task<ApiResponse> Login(LoginDTO dto);
        Task<ApiResponse> GetCurrentUser(string userId);
    }
}
