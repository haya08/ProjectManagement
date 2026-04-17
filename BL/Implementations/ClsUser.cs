using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ProjectManagement.BL.Interfaces;
using ProjectManagement.DTOs;
using ProjectManagement.DTOs.Users;
using ProjectManagement.Models;
using System.Security.Claims;

namespace ProjectManagement.BL.Implementations
{
    [Authorize]
    public class ClsUser : IUser
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJWT _JWT;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IWebHostEnvironment _env;
        public ClsUser(UserManager<ApplicationUser> userManager,
            IJWT jWT,
            IHttpContextAccessor httpContext,
            IWebHostEnvironment env)
        {
            _userManager = userManager;
            _JWT = jWT;
            _httpContext = httpContext;
            _env = env;
        }

        public async Task<ApiResponse> GetCurrentUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            return new ApiResponse
            {
                Data = new UserDTO
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.FirstName + " " + user.LastName
                },
                StatusCode = "200"
            };
        }

        [AllowAnonymous]
        public async Task<ApiResponse> Login(LoginDTO dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
                return new ApiResponse { StatusCode = "401" };

            var valid = await _userManager.CheckPasswordAsync(user, dto.Password);

            if (!valid)
                return new ApiResponse { 
                    Errors = new List<object>
                    {
                        new { Field = "password", Message = "Invalid password" }
                    },
                    StatusCode = "401" 
                };

            if (user.Status != "Approved")
            {
                return new ApiResponse
                {
                    StatusCode = "403",
                    Errors = new List<object>
                    {   
                        new { Message = "Your account is not approved yet" }
                    }
                };
            }

            // هنا هنولد JWT بعد كده
            var token = _JWT.GenerateToken(user);

            return new ApiResponse
            {
                Data = new
                {
                    token = token,
                    userId = user.Id,
                    email = user.Email
                },
                StatusCode = "200"
            };
        }

        [AllowAnonymous]
        public async Task<ApiResponse> Register(RegisterDTO dto)
        {
            var response = new ApiResponse();

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Status = "Pending"
            };

            // a user cannot choose Admin
            if (dto.Role == "Admin")
            {
                response.Errors.Add(new { Message = "Invalid role" });
                response.StatusCode = "400";
                return response;
            }

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                response.Errors = result.Errors.Select(e => (object)e.Description).ToList();
                response.StatusCode = "400";
                return response;
            }

            // assign role
            await _userManager.AddToRoleAsync(user, dto.Role);

            // ProjectManager is Pending
            if (dto.Role == "Project Manager")
            {
                user.Status = "Pending";
            }
            else
            {
                user.Status = "Approved"; // TeamMember مثلاً يدخل علطول
            }

            await _userManager.UpdateAsync(user);

            response.Data = "User created";
            response.StatusCode = "201";
            return response;
        }

        public async Task<ApiResponse> LogOut()
        {
            // Since we're using JWT, logout is handled on the client side by deleting the token.
            return new ApiResponse
            {
                Data = "Logged out",
                StatusCode = "200"
            };
        }

        public async Task<ApiResponse> ApproveProjectManager(string userId)
        {
            var result = new ApiResponse();

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                result.Errors.Add(new { Message = "User not found" });
                result.StatusCode = "404";
                return result;
            }

            if (!await _userManager.IsInRoleAsync(user, "Project Manager"))
            {
                result.Errors.Add(new { Message = "User is not a Project Manager" });
                result.StatusCode = "400";
                return result;
            }

            user.Status = "Approved";
            await _userManager.UpdateAsync(user);

            result.StatusCode = "200";
            result.Data = new { Message = "Project Manager approved" };

            return result;
        }

        public async Task<ApiResponse> RejectProjectManager(string userId)
        {
            var result = new ApiResponse();

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                result.Errors.Add(new { Message = "User not found" });
                result.StatusCode = "404";
                return result;
            }

            user.Status = "Rejected";
            await _userManager.UpdateAsync(user);

            result.StatusCode = "200";
            result.Data = new { Message = "Project Manager rejected" };

            return result;
        }

        public async Task<ApiResponse> GetPendingProjectManagers()
        {
            var users = await _userManager.GetUsersInRoleAsync("Project Manager");

            var pending = users
                .Where(u => u.Status == "Pending")
                .Select(u => new UserDTO
                {
                    Id = u.Id,
                    Email = u.Email,
                    Name = u.FirstName + " " + u.LastName
                });

            return new ApiResponse
            {
                Data = pending,
                StatusCode = "200"
            };
        }

        public async Task<ApiResponse> GetMyProfile()
        {
            var userId = _httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _userManager.FindByIdAsync(userId);

            return new ApiResponse
            {
                Data = new UserDTO
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.FirstName + " " + user.LastName
                },
                StatusCode = "200"
            };
        }

        public async Task<ApiResponse> UpdateProfile(UpdateProfileDTO dto)
        {
            var result = new ApiResponse();

            var userId = _httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                result.StatusCode = "404";
                result.Errors.Add(new { Message = "User not found" });
                return result;
            }

            if (!string.IsNullOrWhiteSpace(dto.Name))
                user.FirstName = dto.Name;

            if(!string.IsNullOrWhiteSpace(dto.Email))
                user.Email = dto.Email;

            await _userManager.UpdateAsync(user);

            result.StatusCode = "200";
            result.Data = "Profile updated";

            return result;
        }

        public async Task<ApiResponse> ChangePassword(ChangePasswordDTO dto)
        {
            var result = new ApiResponse();

            var userId = _httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            var changeResult = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);

            if (!changeResult.Succeeded)
            {
                result.StatusCode = "400";
                result.Errors = changeResult.Errors.Select(e => (object)e.Description).ToList();
                return result;
            }

            result.StatusCode = "200";
            result.Data = "Password changed successfully";

            return result;
        }

        public async Task<ApiResponse> UploadProfileImage(IFormFile file)
        {
            var result = new ApiResponse();

            if (file == null || file.Length == 0)
            {
                result.StatusCode = "400";
                result.Errors.Add(new { Message = "File is required" });
                return result;
            }

            var userId = _httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            var uploadsFolder = Path.Combine(_env.WebRootPath, "profiles");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var path = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            user.ProfileImageUrl = "/profiles/" + fileName;
            await _userManager.UpdateAsync(user);

            result.StatusCode = "200";
            result.Data = user.ProfileImageUrl;

            return result;
        }
    }
}
