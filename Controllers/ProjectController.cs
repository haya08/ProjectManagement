using Microsoft.AspNetCore.Mvc;
using ProjectManagement.BL.Implementations;
using ProjectManagement.BL.Interfaces;
using ProjectManagement.DTOs.Projects;

namespace ProjectManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectBL _bl;

        public ProjectController(IProjectBL bl)
        {
            _bl = bl;
        }

        // Create Project
        [HttpPost]
        public IActionResult CreateProject([FromBody] CreateProjectDTO dto)
        {
            _bl.CreateProject(dto);
            return Ok("Project Created Successfully");
        }

        // Get All Projects
        [HttpGet]
        public IActionResult GetAllProjects()
        {
            var projects = _bl.GetAllProjects();
            return Ok(projects);
        }

        // Update Project
        [HttpPost("Update")]
        public IActionResult UpdateProject([FromBody] UpdateProjectDTO dto)
        {
            _bl.UpdateProject(dto);
            return Ok("Project Updated Successfully");
        }

        // Delete Project
        [HttpPost("Delete")]
        public IActionResult DeleteProject(int id)
        {
            _bl.DeleteProject(id);
            return Ok("Project Deleted Successfully");
        }

        [HttpGet("{id}")]
        public IActionResult GetProjectById(int id)
        {
            var project = _bl.GetProjectById(id);

            if (project == null)
                return NotFound();

            return Ok(project);
        }

        [HttpPost("{projectId}/members")]
        public IActionResult AddMember(int projectId, int userId, string role)
        {
            _bl.AddMember(projectId, userId, role);

            return Ok("Member Added Successfully");
        }

        [HttpGet("{projectId}/members")]
        public IActionResult GetMembers(int projectId)
        {
            var members = _bl.GetMembersByProjectId(projectId);

            return Ok(members);
        }

        [HttpPost("{projectId}/members/delete")]
        public IActionResult DeleteMember(int projectId, int userId)
        {
            _bl.DeleteMember(projectId, userId);

            return Ok("Member Deleted Successfully");
        }

        [HttpPost("{projectId}/members/update-role")]
        public IActionResult UpdateMemberRole(
            int projectId,
            int userId,
            string role)
        {
            _bl.UpdateMemberRole(projectId, userId, role);

            return Ok("Member Role Updated Successfully");
        }

        [HttpGet("GetByManager/{userId}")]
        public IActionResult GetProjectsByManager(int userId)
        {
            var projects = _bl.GetProjectsByManager(userId);

            return Ok(projects);
        }
    }
}