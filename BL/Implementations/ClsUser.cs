using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ProjectManagement.BL.Interfaces;
using ProjectManagement.DTOs;
using ProjectManagement.DTOs.Projects;
using ProjectManagement.DTOs.Tasks;
using ProjectManagement.DTOs.Users;
using ProjectManagement.Models;
using ProjectManagement.Repositories.Interfaces;
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
        private readonly ITaskRepository _taskRepo;
        private readonly IProjectRepository _projectRepo;
        private readonly IProjectMemberRepository _projectMemberRepo;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserRepository _userRepository;
        public ClsUser(UserManager<ApplicationUser> userManager,
            IJWT jWT,
            IHttpContextAccessor httpContext,
            IWebHostEnvironment env,
            ITaskRepository taskRepo,
            IProjectMemberRepository projectMemberRepo,
            IProjectRepository projectRepo,
            RoleManager<IdentityRole> roleManager,
            IUserRepository userRepository)
        {
            _userManager = userManager;
            _JWT = jWT;
            _httpContext = httpContext;
            _env = env;
            _taskRepo = taskRepo;
            _projectMemberRepo = projectMemberRepo;
            _projectRepo = projectRepo;
            _roleManager = roleManager;
            _userRepository = userRepository;
        }

        public async Task<ApiResponse> GetAllUsers()
        {
            var users = _userRepository.GetAll();
            return new ApiResponse
            {
                Data = users.Select(u => new UserDTO
                {
                    Id = u.Id,
                    Email = u.Email,
                    Name = u.FirstName + " " + u.LastName,
                    Role = _userManager.GetRolesAsync(u).Result.FirstOrDefault() // Get the first role (assuming one role per user)
                }).ToList(),
                StatusCode = "200"
            };
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
                return new ApiResponse
                {
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
                Status = "Pending",
                ProfileImageUrl = ""
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

            if (!string.IsNullOrWhiteSpace(dto.Email))
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

        public ApiResponse GetUserDashboard()
        {
            var result = new ApiResponse();

            var userId = _httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Tasks
            var tasks = _taskRepo.GetByUser(userId);

            // Projects
            var projectMembers = _projectMemberRepo.GetByUser(userId);
            var projects = projectMembers.Select(pm => pm.Project).ToList();

            // Stats
            var total = tasks.Count;
            var done = tasks.Count(t => t.Status == "done");
            var inProgress = tasks.Count(t => t.Status == "in_progress");
            var todo = tasks.Count(t => t.Status == "todo");
            var overdue = tasks.Count(t => t.DueDate < DateTime.UtcNow && t.Status != "done");

            var dashboard = new UserDashboardDTO
            {
                MyTasks = tasks.Select(t => new TasksDTO
                {
                    Id = t.Id,
                    Title = t.Title,
                    Status = t.Status,
                    Priority = t.Priority
                }).ToList(),

                MyProjects = projects.Select(p => new ProjectsDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    CreatedAt = p.CreatedAt
                }).ToList(),

                Stats = new UserStatsDTO
                {
                    TotalTasks = total,
                    DoneTasks = done,
                    InProgressTasks = inProgress,
                    TodoTasks = todo,
                    OverDueTasks = overdue
                }
            };

            result.Data = dashboard;
            result.StatusCode = "200";

            return result;
        }

        public async Task<ApiResponse> GetAdminDashboard()
        {
            var result = new ApiResponse();

            // Users
            var users = _userManager.Users.ToList();
            var totalUsers = users.Count;

            var projectManagers = await _userManager.GetUsersInRoleAsync("Project Manager");

            var pendingPM = projectManagers.Count(u => u.Status == "Pending");
            var approvedPM = projectManagers.Count(u => u.Status == "Approved");

            // Projects
            var totalProjects = _projectRepo.GetAll().Count();

            // Tasks
            var tasks = _taskRepo.GetAll(new TaskQueryDTO());
            var totalTasks = tasks.Count();

            var doneTasks = tasks.Count(t => t.Status == "done");
            var pendingTasks = tasks.Count(t => t.Status != "done");
            var overdueTasks = tasks.Count(t => t.DueDate < DateTime.UtcNow && t.Status != "done");

            // Progress
            var progress = totalTasks == 0 ? 0 : (double)doneTasks / totalTasks * 100;

            result.Data = new AdminDashboardDTO
            {
                TotalUsers = totalUsers,
                TotalProjects = totalProjects,
                TotalTasks = totalTasks,

                TotalProjectManagers = projectManagers.Count,
                PendingProjectManagers = pendingPM,
                ApprovedProjectManagers = approvedPM,

                DoneTasks = doneTasks,
                PendingTasks = pendingTasks,
                OverDueTasks = overdueTasks,

                SystemProgress = Math.Round(progress, 2)
            };

            result.StatusCode = "200";
            return result;
        }

        public async Task<ApiResponse> AssignRole(RoleDTO dto)
        {
            var result = new ApiResponse();

            var user = await _userManager.FindByIdAsync(dto.UserId);

            if (user == null)
            {
                result.StatusCode = "404";
                result.Errors.Add(new { User = "User not found" });
                return result;
            }

            var roleExists = await _roleManager.RoleExistsAsync(dto.Role);

            if (!roleExists)
            {
                result.StatusCode = "400";
                result.Errors.Add(new { Role = "Role does not exist" });
                return result;
            }

            var addRoleResult = await _userManager.AddToRoleAsync(user, dto.Role);

            if (!addRoleResult.Succeeded)
            {
                result.StatusCode = "400";
                result.Errors = new List<object>(addRoleResult.Errors.Select(e => e.Description).ToList());
                return result;
            }

            result.StatusCode = "200";
            result.Data = "Role assigned successfully";

            return result;
        }

        public async Task<ApiResponse> ChangeRole(RoleDTO dto)
        {
            var result = new ApiResponse();

            var user = await _userManager.FindByIdAsync(dto.UserId);

            if (user == null)
            {
                result.StatusCode = "404";
                result.Errors.Add(new { User = "User not found" });
                return result;
            }

            var currentRoles = await _userManager.GetRolesAsync(user);

            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            await _userManager.AddToRoleAsync(user, dto.Role);

            result.StatusCode = "200";
            result.Data = "Role updated successfully";

            return result;
        }

        public async Task<ApiResponse> RemoveRole(RoleDTO dto)
        {
            var result = new ApiResponse();

            var user = await _userManager.FindByIdAsync(dto.UserId);

            if (user == null)
            {
                result.StatusCode = "404";
                result.Errors.Add(new { User = "User not found" });
                return result;
            }

            var removeResult = await _userManager.RemoveFromRoleAsync(user, dto.Role);

            if (!removeResult.Succeeded)
            {
                result.StatusCode = "400";
                result.Errors = new List<object>(removeResult.Errors.Select(e => e.Description).ToList());
                return result;
            }

            result.StatusCode = "200";
            result.Data = "Role removed successfully";

            return result;
        }

        public async Task<ApiResponse> CreateUser(AdminCreateUserDTO dto)
        {
            var result = new ApiResponse();

            // validation
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            {
                result.StatusCode = "400";
                result.Errors.Add(new { Message = "Email and Password are required" });
                return result;
            }

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Status = "Approved" // admin-created users usually auto approved
            };

            var createResult = await _userManager.CreateAsync(user, dto.Password);

            if (!createResult.Succeeded)
            {
                result.StatusCode = "400";
                result.Errors = new List<object>(createResult.Errors.Select(e => e.Description).ToList());
                return result;
            }

            // assign role
            if (!string.IsNullOrEmpty(dto.Role))
            {
                var roleExists = await _roleManager.RoleExistsAsync(dto.Role);

                if (!roleExists)
                {
                    result.StatusCode = "400";
                    result.Errors.Add(new { Role = "Invalid role" });
                    return result;
                }

                await _userManager.AddToRoleAsync(user, dto.Role);
            }

            result.StatusCode = "201";
            result.Data = new
            {
                user.Id,
                user.Email,
                Role = dto.Role
            };

            return result;
        }

        public async Task<ApiResponse> DeleteUser(string userId)
        {
            var currentAdminId = _httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = new ApiResponse();

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                result.StatusCode = "404";
                result.Errors.Add(new { Message = "User not found" });
                return result;
            }

            // admin مايمسحش نفسه
            if (userId == currentAdminId)
            {
                result.StatusCode = "400";
                result.Errors.Add(new { Message = "You cannot delete yourself" });
                return result;
            }

            var deleteResult = await _userManager.DeleteAsync(user);

            if (!deleteResult.Succeeded)
            {
                result.StatusCode = "400";
                result.Errors = new List<object>(deleteResult.Errors.Select(e => e.Description).ToList());
                return result;
            }

            result.StatusCode = "200";
            result.Data = "User deleted successfully";

            return result;
        }
    }
}
