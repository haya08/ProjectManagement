using ProjectManagement.BL.Interfaces;
using ProjectManagement.DTOs;
using ProjectManagement.DTOs.Projects;
using ProjectManagement.Models;
using ProjectManagement.Repositories.Interfaces;
using System.Security.Claims;

namespace ProjectManagement.BL.Implementations
{
    public class ClsProject : IProject
    {
        private readonly IProjectRepository _repo;
        private readonly IHttpContextAccessor _httpContext;
        public ClsProject(IProjectRepository repo, IHttpContextAccessor httpContext)
        {
            _repo = repo;
            _httpContext = httpContext;
        }

        public ApiResponse GetAll()
        {
            var projects = _repo.GetAll();

            var result = projects.Select(p => new ProjectsDTO
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                CreatedAt = p.CreatedAt
            });

            return new ApiResponse
            {
                Data = result,
                StatusCode = "200"
            };
        }

        public ApiResponse GetById(int id)
        {
            var project = _repo.GetById(id);

            if (project == null)
                return new ApiResponse { StatusCode = "404" };

            return new ApiResponse
            {
                Data = new ProjectsDTO
                {
                    Id = project.Id,
                    Name = project.Name,
                    Description = project.Description,
                    CreatedBy = project.CreatedBy,
                    CreatedAt = project.CreatedAt
                },
                StatusCode = "200"
            };
        }

        public ApiResponse Create(CreateProjectDTO dto)
        {
            var result = new ApiResponse();

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                result.Errors.Add(new { Name = "Name is required" });
                result.StatusCode = "400";
                return result;
            }

            var userId = _httpContext.HttpContext.User
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var project = new TbProject
            {
                Name = dto.Name,
                Description = dto.Description,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow
            };

            _repo.Add(project);
            _repo.Save();

            result.Data = new ProjectsDTO
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                CreatedBy = project.CreatedBy,
                CreatedAt = project.CreatedAt
            };
            result.StatusCode = "201";

            return result;
        }

        public ApiResponse Update(UpdateProjectDTO dto)
        {
            var project = _repo.GetById(dto.Id);

            var result = new ApiResponse();

            if (project == null)
            {
                result.Errors.Add(new
                {
                    Field = "Id",
                    Message = "Project not found"
                });
                result.StatusCode = "404";
                return result;
            }

            if (dto.Name != null)
                project.Name = dto.Name;

            if (dto.Description != null)
                project.Description = dto.Description;

            _repo.Update(project);
            _repo.Save();

            return new ApiResponse
            {
                Data = new ProjectsDTO
                {
                    Id = project.Id,
                    Name = project.Name,
                    Description = project.Description,
                    CreatedBy = project.CreatedBy,
                    CreatedAt = project.CreatedAt
                },
                StatusCode = "200"
            };
        }

        public ApiResponse Delete(int id)
        {
            var project = _repo.GetById(id);

            if (project == null)
                return new ApiResponse { StatusCode = "404" };

            _repo.Delete(project);
            _repo.Save();

            return new ApiResponse
            {
                StatusCode = "200"
            };
        }
    }
}
