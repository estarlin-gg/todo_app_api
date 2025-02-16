using Microsoft.EntityFrameworkCore;
using Todo_api.Context;
using Todo_api.Dtos;
using Todo_api.Models;
using Todo_api.Services.Abstraction;
using Todo_api.Factories;

namespace Todo_api.Services.Implementation
{
    public class TaskService : ITaskService
    {
        private readonly ToDoContext _toDoContext;
        private readonly TaskQueueService _taskQueueService;

        public TaskService(ToDoContext toDoContext, TaskQueueService taskQueueService)
        {
            _toDoContext = toDoContext;
            _taskQueueService = taskQueueService;
        }

        public async Task<IEnumerable<object>> GetAllTasks()
        {
            var tasks = await _toDoContext.Tasks.ToListAsync();
            return tasks.Select(task => new
            {
                task.Id,
                task.Title,
                task.Description,
                task.Status,
                task.Prioridad,
                task.GenericValue,
                DiasRestantes = (task.FechaDeVencimiento - DateTime.Now).Days
            });
        }

        public void EnqueueTaskCreation(TodoTaskDto<string> taskDto)
        {
            _taskQueueService.EnqueueTask(async () =>
            {
                var task = TodoTaskFactory.CreateLowPriorityTask(taskDto.Title, taskDto.Description, taskDto.GenericValue);
                _toDoContext.Tasks.Add(task);
                await _toDoContext.SaveChangesAsync();
            });
        }

        public void EnqueueTaskUpdate(int id, TodoTaskDto<string> taskDto)
        {
            _taskQueueService.EnqueueTask(async () =>
            {
                var searchedTask = await _toDoContext.Tasks.FindAsync(id);
                if (searchedTask != null)
                {
                    searchedTask.Title = taskDto.Title;
                    searchedTask.Description = taskDto.Description;
                    searchedTask.Status = taskDto.Status;
                    searchedTask.Prioridad = taskDto.Prioridad;
                    searchedTask.GenericValue = taskDto.GenericValue;

                    await _toDoContext.SaveChangesAsync();
                }
            });
        }

        public void EnqueueTaskDeletion(int id)
        {
            _taskQueueService.EnqueueTask(async () =>
            {
                var task = await _toDoContext.Tasks.FindAsync(id);
                if (task != null)
                {
                    _toDoContext.Tasks.Remove(task);
                    await _toDoContext.SaveChangesAsync();
                }
            });
        }
    }
}
