using Microsoft.AspNetCore.Mvc;
using Todo_api.Dtos;
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
        public async Task<IActionResult> GetTasks()
        {
            var tasks = await _taskService.GetAllTasks();
            return Ok(tasks);
        }

        [HttpPost]
        public IActionResult CreateTask([FromBody] TodoTaskDto<string> task)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            _taskService.EnqueueTaskCreation(task);
            return Ok("Tarea encolada para creación.");
        }

        [HttpPatch("{id}")]
        public IActionResult EditTask(int id, [FromBody] TodoTaskDto<string> taskDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            _taskService.EnqueueTaskUpdate(id, taskDto);
            return Ok("Tarea encolada para actualización.");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteTask(int id)
        {
            _taskService.EnqueueTaskDeletion(id);
            return Ok("Tarea encolada para eliminación.");
        }
    }
}
