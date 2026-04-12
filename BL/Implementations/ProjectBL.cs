using ProjectManagement.BL.Interfaces;
using ProjectManagement.DTOs.Projects;
using ProjectManagement.Models;
using ProjectManagement.Repositories.Interfaces;
using System.Linq;

namespace ProjectManagement.BL.Implementations
{
    public class ProjectBL : IProjectBL
    {
        private readonly IProjectRepository _repo;

        public ProjectBL(IProjectRepository repo)
        {
            _repo = repo;
        }

        // Create Project
        public void CreateProject(CreateProjectDTO dto)
        {
            var project = new TbProject
            {
                Name = dto.Name,
                Description = dto.Description,
                CreatedBy = dto.CreatedBy,
                CreatedAt = DateTime.Now
            };

            _repo.Add(project);
            _repo.Save();
        }

        // Get All Projects
        public List<ProjectDTO> GetAllProjects()
        {
            var projects = _repo.GetAll();

            return projects.Select(p => new ProjectDTO
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                CreatedBy = p.CreatedBy ?? 0,
                CreatedAt = p.CreatedAt ?? DateTime.Now
            }).ToList();
        }

        // Update Project
        public void UpdateProject(UpdateProjectDTO dto)
        {
            var project = _repo.GetById(dto.Id);

            if (project == null)
                return;

            project.Name = dto.Name;
            project.Description = dto.Description;

            _repo.Update(project);
            _repo.Save();
        }

        // Delete Project
        public void DeleteProject(int id)
        {
            var project = _repo.GetById(id);

            if (project == null)
                return;

            _repo.Delete(project);
            _repo.Save();
        }

        public ProjectDTO GetProjectById(int id)
{
    var project = _repo.GetById(id);

    if (project == null)
        return null;

    return new ProjectDTO
    {
        Id = project.Id,
        Name = project.Name,
        Description = project.Description,
        CreatedBy = project.CreatedBy ?? 0,
        CreatedAt = project.CreatedAt ?? DateTime.Now
    };
}

        public void AddMember(int projectId, int userId, string role)
{
    var member = new TbProjectMember
    {
        ProjectId = projectId,
        UserId = userId,
        Role = role
    };

    _repo.AddMember(member);
    _repo.Save();
}

public List<ProjectMemberDTO> GetMembersByProjectId(int projectId)
{
    var members = _repo.GetMembersByProjectId(projectId);

    return members.Select(m => new ProjectMemberDTO
{
    UserId = m.UserId ?? 0,
    Role = m.Role
}).ToList();
}

public void DeleteMember(int projectId, int userId)
{
    _repo.DeleteMember(projectId, userId);
    _repo.Save();
}

public void UpdateMemberRole(int projectId, int userId, string role)
{
    _repo.UpdateMemberRole(projectId, userId, role);
    _repo.Save();
}

public List<ProjectDTO> GetProjectsByManager(int userId)
{
    var projects = _repo.GetProjectsByManager(userId);

    return projects.Select(p => new ProjectDTO
    {
        Id = p.Id,
        Name = p.Name,
        Description = p.Description,
        CreatedBy = p.CreatedBy ?? 0,
        CreatedAt = p.CreatedAt ?? DateTime.Now
    }).ToList();
}


    }
}