using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.BL.Implementations;
using ProjectManagement.BL.Interfaces;
using ProjectManagement.DTOs;
using ProjectManagement.DTOs.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProjectManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TaskController : ControllerBase
    {
        private readonly ITask _taskBL;

        public TaskController(ITask taskBL)
        {
            _taskBL = taskBL;
        }

        // GET: api/<TaskController>
        [HttpGet]
        public IActionResult Get([FromQuery] TaskQueryDTO query)
        {
            var response = _taskBL.GetAllTasks(query);
            return StatusCode(int.Parse(response.StatusCode), response);
        }

        // GET api/<TaskController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var response = _taskBL.GetTaskById(id);
            return StatusCode(int.Parse(response.StatusCode), response);
        }

        // GET api/<TaskController>/5
        [HttpGet("GetByProjectId/{id}")]
        public IActionResult GetByProjectId(int id)
        {
            var tasks = _taskBL.GetTasksByProjectId(id);
            return Ok(tasks);
        }

        // POST api/<TaskController>
        [HttpPost]
        public IActionResult Post([FromBody] CreateTaskDTO task)
        {
            if (!ModelState.IsValid)
            {
                var result = new ApiResponse
                {
                    Data = null,
                    Errors = new List<object> { ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList() },
                    StatusCode = "400"
                };
                return StatusCode(int.Parse(result.StatusCode), result);
            }
            var response = _taskBL.CreateTask(task);
            return StatusCode(int.Parse(response.StatusCode), response);
        }

        // POST api/<TaskController>
        [HttpPost]
        [Route("Update")]
        public IActionResult Update([FromBody] UpdateTaskDTO task)
        {
            if (!ModelState.IsValid)
            {
                var result = new ApiResponse
                {
                    Data = null,
                    Errors = new List<object> { ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList() },
                    StatusCode = "400"
                };
                return StatusCode(int.Parse(result.StatusCode), result);
            }
            var response = _taskBL.UpdateTask(task);
            return StatusCode(int.Parse(response.StatusCode), response);
        }

        // POST api/<TaskController>
        [HttpPost]
        [Route("Delete")]
        public IActionResult Delete([FromBody] int id)
        {
            var response = _taskBL.DeleteTask(id);
            return StatusCode(int.Parse(response.StatusCode), response);
        }
    }
}
