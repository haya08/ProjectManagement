using ProjectManagement.DTOs;
using ProjectManagement.DTOs.Users;

namespace ProjectManagement.BL.Interfaces
{
    public interface IUser
    {
        Task<ApiResponse> Register(RegisterDTO dto);
        Task<ApiResponse> Login(LoginDTO dto);
        Task<ApiResponse> GetCurrentUser(string userId);
        Task<ApiResponse> ApproveProjectManager(string userId);
        Task<ApiResponse> RejectProjectManager(string userId);
        Task<ApiResponse> GetPendingProjectManagers();
    }
}
