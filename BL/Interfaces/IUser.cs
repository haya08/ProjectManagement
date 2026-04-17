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
        Task<ApiResponse> GetMyProfile();
        Task<ApiResponse> UpdateProfile(UpdateProfileDTO dto);
        Task<ApiResponse> ChangePassword(ChangePasswordDTO dto);
        Task<ApiResponse> UploadProfileImage(IFormFile file);
        ApiResponse GetUserDashboard();
        Task<ApiResponse> GetAdminDashboard();
        Task<ApiResponse> AssignRole(RoleDTO dto);
        Task<ApiResponse> ChangeRole(RoleDTO dto);
        Task<ApiResponse> RemoveRole(RoleDTO dto);
    }
}
