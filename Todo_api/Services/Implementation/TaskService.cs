using Microsoft.EntityFrameworkCore;
using Todo_api.Context;
using Todo_api.Dtos;
using Todo_api.Models;
using Todo_api.Services.Abstraction;

namespace Todo_api.Services.Implementation
{
    public class TaskService : ITaskService
    {
        private readonly ToDoContext _toDoContext;

        public TaskService(ToDoContext toDoContext)
        {
            _toDoContext = toDoContext;
        }
        public async Task CreateTask(TodoTaskDto<string> taskDto)
        {
            var task = new TodoTask<string>
            {
                Title = taskDto.Title,
                Description = taskDto.Description,
                FechaDeVencimiento = taskDto.FechaDeVencimiento,
                Status = taskDto.Status,
                Prioridad = taskDto.Prioridad,
                GenericValue = taskDto.GenericValue
            };

            _toDoContext.Tasks.Add(task);
            await _toDoContext.SaveChangesAsync();
        }

        public async Task DeleteTask(int id)
        {
            try
            {
                var task = await _toDoContext.Tasks.FindAsync(id);
                if (task != null)
                {
                    _toDoContext.Remove(task);
                    await _toDoContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("tarea no encontrada", ex);

            }
        }

        public async Task<IEnumerable<TodoTask<string>>> GetAllTask()
        {
            return await _toDoContext.Tasks.ToListAsync();
        }

        public async Task UpdateTask(int id, TodoTaskDto<string> task)
        {
            var searchedTask = await _toDoContext.Tasks.FirstOrDefaultAsync(t => t.Id == id);

            if (searchedTask == null)
            {
                throw new KeyNotFoundException($"tarea no con el id {id} no encontrada.");
            }
            searchedTask.Title = task.Title;
            searchedTask.Description = task.Description;
            searchedTask.Status = task.Status;
            searchedTask.Prioridad = task.Prioridad;
            searchedTask.GenericValue = task.GenericValue;


            await _toDoContext.SaveChangesAsync();

        }
    }

}
