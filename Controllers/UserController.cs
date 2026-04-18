using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.BL.Interfaces;
using ProjectManagement.DTOs.Users;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProjectManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUser _userBL;
        public UserController(IUser userBL)
        {
            _userBL = userBL;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            var result = await _userBL.Register(dto);
            return StatusCode(int.Parse(result.StatusCode), result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            var result = await _userBL.Login(dto);
            return StatusCode(int.Parse(result.StatusCode), result);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _userBL.GetCurrentUser(userId);

            return StatusCode(int.Parse(result.StatusCode), result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("approve/{userId}")]
        public async Task<IActionResult> Approve(string userId)
        {
            var result = await _userBL.ApproveProjectManager(userId);
            return StatusCode(int.Parse(result.StatusCode), result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("reject/{userId}")]
        public async Task<IActionResult> Reject(string userId)
        {
            var result = await _userBL.RejectProjectManager(userId);
            return StatusCode(int.Parse(result.StatusCode), result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingProjectManagers()
        {
            var result = await _userBL.GetPendingProjectManagers();
            return StatusCode(int.Parse(result.StatusCode), result);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var result = await _userBL.GetMyProfile();
            return StatusCode(int.Parse(result.StatusCode), result);
        }

        [Authorize]
        [HttpPut("me")]
        public async Task<IActionResult> UpdateProfile(UpdateProfileDTO dto)
        {
            var result = await _userBL.UpdateProfile(dto);
            return StatusCode(int.Parse(result.StatusCode), result);
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO dto)
        {
            var result = await _userBL.ChangePassword(dto);
            return StatusCode(int.Parse(result.StatusCode), result);
        }

        [Authorize]
        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile file)
        {
            var result = await _userBL.UploadProfileImage(file);
            return StatusCode(int.Parse(result.StatusCode), result);
        }

        [Authorize]
        [HttpGet("dashboard")]
        public IActionResult GetDashboard()
        {
            var result = _userBL.GetUserDashboard();
            return StatusCode(int.Parse(result.StatusCode), result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("adminDashboard")]
        public async Task<IActionResult> GetAdminDashboard()
        {
            var result = await _userBL.GetAdminDashboard();
            return StatusCode(int.Parse(result.StatusCode), result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole(RoleDTO dto)
        {
            var result = await _userBL.AssignRole(dto);
            return StatusCode(int.Parse(result.StatusCode), result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("change-role")]
        public async Task<IActionResult> ChangeRole(RoleDTO dto)
        {
            var result = await _userBL.ChangeRole(dto);
            return StatusCode(int.Parse(result.StatusCode), result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("remove-role")]
        public async Task<IActionResult> RemoveRole(RoleDTO dto)
        {
            var result = await _userBL.RemoveRole(dto);
            return StatusCode(int.Parse(result.StatusCode), result);
        }

        // Create user
        [Authorize(Roles = "Admin")]
        [HttpPost("create-user")]
        public async Task<IActionResult> CreateUser(AdminCreateUserDTO dto)
        {
            var result = await _userBL.CreateUser(dto);
            return StatusCode(int.Parse(result.StatusCode), result);
        }

        // Delete user
        [Authorize(Roles = "Admin")]
        [HttpDelete("delete-user/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await _userBL.DeleteUser(id);

            return StatusCode(int.Parse(result.StatusCode), result);
        }
    }
}
