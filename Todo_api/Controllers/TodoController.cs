using Microsoft.AspNetCore.Mvc;
using Todo_api.Dtos;
using Todo_api.Models;
using Todo_api.Services.Abstraction;

namespace Todo_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly ITaskService _taskService;
        public TodoController(ITaskService taskService)
        {
            _taskService = taskService;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoTask<string>>>> GetTasks()
        {
            try
            {
                var tasks = await _taskService.GetAllTask();
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener las tareas: {ex.Message}");
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] TodoTaskDto<string> task)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _taskService.CreateTask(task);
                return Ok(task); 
            }
            catch (Exception ex)
            {
               
                return StatusCode(500, $"Error al crear la tarea: {ex.Message}");
            }
           
        }
        [HttpPatch("{id}")]
        public async Task<IActionResult> EditTask([FromBody] TodoTaskDto<string> todo, int id)
        {
            try
            {
                await _taskService.UpdateTask(id, todo);
                return Ok();
            }
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }
    }
}

