using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ProjectManagement.BL.Interfaces;
using ProjectManagement.DTOs;
using ProjectManagement.DTOs.Users;
using ProjectManagement.Models;

namespace ProjectManagement.BL.Implementations
{
    [Authorize]
    public class ClsUser : IUser
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJWT _JWT;
        public ClsUser(UserManager<ApplicationUser> userManager, IJWT jWT)
        {
            _userManager = userManager;
            _JWT = jWT;
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
    }
}
