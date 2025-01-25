using Microsoft.EntityFrameworkCore;
using Todo_api.Context;
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
        public async Task CreateTask(TodoTask<string> task)
        {
            _toDoContext.Tasks.Add(task);
            await _toDoContext.SaveChangesAsync();
        }

        public async Task DeleteTask(int id)
        {
            try
            {
                var task = _toDoContext.Tasks.FindAsync(id);
                if (task != null)
                {
                    _toDoContext.Remove(task);
                    await _toDoContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("tarea no encontrada");

            }
        }

        public async Task<IEnumerable<TodoTask<string>>> GetAllTask()
        {
            return await _toDoContext.Tasks.ToListAsync();
        }

        public async Task UpdateTask(int id, TodoTask<string> task)
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


            await _toDoContext.SaveChangesAsync();

        }
    }

}
