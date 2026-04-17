using ProjectManagement.BL.Interfaces;
using ProjectManagement.DTOs;
using ProjectManagement.DTOs.ProjectMembers;
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
            var result = new ApiResponse();

            var currentUserId = _httpContext.HttpContext.User
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var currentUserRole = _httpContext.HttpContext.User
                .FindFirst(ClaimTypes.Role)?.Value;

            // check: only Admin or ProjectManager can add members
            var existing = _repo.GetByUserAndProject(currentUserId, dto.ProjectId);

            if (currentUserRole != "Admin" &&
                (existing == null || existing.Role != "Project Manager"))
            {
                result.Errors.Add(new { Message = "Not authorized" });
                result.StatusCode = "403";
                return result;
            }

            // check if already member
            var memberExists = _repo.GetByUserAndProject(dto.UserId, dto.ProjectId);
            if (memberExists != null)
            {
                result.Errors.Add(new { Message = "User already in project" });
                result.StatusCode = "400";
                return result;
            }

            var member = new TbProjectMember
            {
                ProjectId = dto.ProjectId,
                UserId = dto.UserId,
                Role = dto.Role,
                JoinedAt = DateTime.UtcNow
            };

            _repo.Add(member);
            _repo.Save();

            result.Data = new 
            {
                UserId = member.UserId,
                ProjectId = member.ProjectId,
                Role = member.Role,
                JoinedAt = member.JoinedAt
            };
            result.StatusCode = "201";

            return result;
        }

        public ApiResponse GetProjectMembers(int projectId)
        {
            var members = _repo.GetByProjectId(projectId);

            var result = members.Select(m => new ProjectMemberDTO
            {
                Id = m.Id,
                UserId = m.UserId,
                UserName = m.User?.UserName,
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

            _repo.Delete(member);
            _repo.Save();

            result.StatusCode = "200";
            return result;
        }
    }
}
