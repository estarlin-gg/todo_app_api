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
        private readonly CacheService _cacheService;

        public TaskService(ToDoContext toDoContext, TaskQueueService taskQueueService, CacheService cacheService)
        {
            _toDoContext = toDoContext;
            _taskQueueService = taskQueueService;
            _cacheService = cacheService;
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


   
        public async Task<double> GetTaskCompletionRate()
        {
            
            return await _cacheService.GetOrAdd("taskCompletionRate", async () =>
            {
                var totalTasks = await _toDoContext.Tasks.CountAsync();
                var completedTasks = await _toDoContext.Tasks.CountAsync(t => t.Status == "Completed");
                return totalTasks == 0 ? 0 : (double)completedTasks / totalTasks * 100;
            });
        }


        public async Task<IEnumerable<object>> GetFilteredTasks(string status = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            
            string cacheKey = $"filteredTasks_{status}_{startDate?.ToString()}_{endDate?.ToString()}";

            return await _cacheService.GetOrAdd(cacheKey, async () =>
            {
                IQueryable<TodoTask<string>> query = _toDoContext.Tasks;

                if (!string.IsNullOrEmpty(status))
                {
                    query = query.Where(t => t.Status == status);
                }
                if (startDate.HasValue)
                {
                    query = query.Where(t => t.FechaDeVencimiento >= startDate.Value);
                }
                if (endDate.HasValue)
                {
                    query = query.Where(t => t.FechaDeVencimiento <= endDate.Value);
                }

                var tasks = await query.ToListAsync();
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
            });
        }
    }
}
