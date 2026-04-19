using ProjectManagement.BL.Interfaces;
using ProjectManagement.DTOs;
using ProjectManagement.DTOs.ProjectMembers;
using ProjectManagement.DTOs.Users;
using ProjectManagement.Models;
using ProjectManagement.Repositories.Interfaces;
using System.Security.Claims;

namespace ProjectManagement.BL.Implementations
{
    public class ClsProjectMembers : IProjectMembers
    {
        private readonly IProjectMemberRepository _repo;
        private readonly IHttpContextAccessor _httpContext;

        public ClsProjectMembers(IProjectMemberRepository repo, IHttpContextAccessor httpContext)
        {
            _repo = repo;
            _httpContext = httpContext;
        }

        public ApiResponse AddMember(AddMemberDTO dto)
        {

            var currentUserId = _httpContext.HttpContext.User
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var member = _repo.GetByUserAndProject(currentUserId, dto.ProjectId);

            if(member == null)
            {
                var result = new ApiResponse
                {
                    Errors = new List<object> { new { Message = "Not a member of the project" } },
                    StatusCode = "403"
                };
                return result;
            }

            if (member.Role.ToLower() != "project manager")
            {
                var result = new ApiResponse
                {
                    Errors = new List<object> { new { Message = "Not authorized" } },
                    StatusCode = "403"
                };
                return result;
            }

            var response = new ApiResponse();

            var newMember = new TbProjectMember
            {
                ProjectId = dto.ProjectId,
                UserId = dto.UserId,
                Role = dto.Role,
                JoinedAt = DateTime.UtcNow
            };

            _repo.Add(newMember);
            _repo.Save();

            response.Data = new
            {
                UserId = newMember.UserId,
                ProjectId = newMember.ProjectId,
                Role = newMember.Role,
                JoinedAt = newMember.JoinedAt
            };
            response.StatusCode = "201";

            return response;
        }

        public ApiResponse GetProjectMembers(int projectId)
        {
            var members = _repo.GetByProjectId(projectId);

            var result = members.Select(m => new ProjectMemberDTO
            {
                Id = m.Id,
                UserId = m.UserId,
                UserName = m.User?.FirstName + " " + m.User?.LastName,
                Role = m.Role
            });

            return new ApiResponse
            {
                Data = result,
                StatusCode = "200"
            };
        }

        public ApiResponse RemoveMember(int projectId, string userId)
        {
            var result = new ApiResponse();

            var member = _repo.GetByUserAndProject(userId, projectId);

            if (member == null)
            {
                result.StatusCode = "404";
                return result;
            }

            if(member.Role.ToLower() != "project manager")
            {
                result.Errors.Add(new { Message = "Not authorized" });
                result.StatusCode = "403";
                return result;
            }

            _repo.Delete(member);
            _repo.Save();

            result.StatusCode = "200";
            return result;
        }

        public ApiResponse ChangeRole(ProjectRoleDTO dto)
        {
            var result = new ApiResponse();

            // Authorization check: Only project managers can change roles

            var user = _httpContext.HttpContext.User;
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var projectManger = _repo.GetByUserAndProject(userId, dto.ProjectId);

            if (projectManger == null)
            {
                result.StatusCode = "404";
                return result;
            }

            if(projectManger.Role.ToLower() != "project manager")
            {
                result.Errors.Add(new { Message = "Not authorized" });
                result.StatusCode = "403";
                return result;
            }

            var member = _repo.GetByUserAndProject(dto.UserId, dto.ProjectId);

            if(member == null)
            {
                return new ApiResponse
                {
                    Errors = new List<object> { new { Message = "Member not found" } },
                    StatusCode = "404"
                };
            }

            member.Role = dto.Role;
            _repo.Save();

            result.StatusCode = "200";
            return result;
        }
    }
}
