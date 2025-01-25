using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> CreateTask([FromBody] TodoTask<string> task)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _taskService.CreateTask(task);
                return CreatedAtAction(nameof(GetTasks), new { id = task.Id }, task); 
            }
            catch (Exception ex)
            {
               
                return StatusCode(500, $"Error al crear la tarea: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("final jeje");
            }
        }
        //[HttpPatch("{id}")]
        //public async Task EditTask 
    }
}

