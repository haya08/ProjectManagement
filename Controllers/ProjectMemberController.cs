using Microsoft.AspNetCore.Mvc;
using ProjectManagement.BL.Interfaces;
using ProjectManagement.DTOs.ProjectMembers;
using ProjectManagement.Repositories.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProjectManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectMemberController : ControllerBase
    {
        private readonly IProjectMembers _projectMemberBL;

        public ProjectMemberController(IProjectMembers projectMemberBL)
        {
            _projectMemberBL = projectMemberBL;
        }

        [HttpPost]
        public IActionResult AddMember([FromBody] AddMemberDTO dto)
        {
            var result = _projectMemberBL.AddMember(dto);
            return StatusCode(int.Parse(result.StatusCode), result);
        }

        [HttpGet("{projectId}")]
        public IActionResult GetMembers(int projectId)
        {
            var result = _projectMemberBL.GetProjectMembers(projectId);
            return StatusCode(int.Parse(result.StatusCode), result);
        }

        [HttpPost("RemoveMember")]
        public IActionResult RemoveMember([FromBody] RemoveMemberDTO dto)
        {
            var result = _projectMemberBL.RemoveMember(dto.ProjectId, dto.UserId);
            return StatusCode(int.Parse(result.StatusCode), result);
        }
    }
}
