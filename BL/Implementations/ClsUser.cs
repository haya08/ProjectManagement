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
                LastName = dto.LastName
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                response.Errors = new List<object>(result.Errors.Select(e => e.Description).ToList());
                response.StatusCode = "400";
                return response;
            }

            response.Data = "User created";
            response.StatusCode = "201";
            return response;
        }
    }
}
