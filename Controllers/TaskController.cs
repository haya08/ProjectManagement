using Microsoft.AspNetCore.Mvc;
using ProjectManagement.BL.Implementations;
using ProjectManagement.BL.Interfaces;
using ProjectManagement.DTOs.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProjectManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITask _taskBL;

        public TaskController(ITask taskBL)
        {
            _taskBL = taskBL;
        }

        // GET: api/<TaskController>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_taskBL.GetAllTasks());
        }

        // GET api/<TaskController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var task = _taskBL.GetTaskById(id);
            if (task == null) return NotFound();
            return Ok(task);
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
            try
            {
                _taskBL.CreateTask(task);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST api/<TaskController>
        [HttpPost]
        [Route("Update")]
        public IActionResult Update([FromBody] UpdateTaskDTO task)
        {
            try
            {
                _taskBL.UpdateTask(task);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST api/<TaskController>
        [HttpPost]
        [Route("Delete")]
        public IActionResult Delete([FromBody] int id)
        {
            try
            {
                _taskBL.DeleteTask(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
