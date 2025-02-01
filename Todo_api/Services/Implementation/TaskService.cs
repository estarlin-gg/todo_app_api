using Microsoft.EntityFrameworkCore;
using Todo_api.Context;
using Todo_api.Dtos;
using Todo_api.Models;
using Todo_api.Services.Abstraction;
using Todo_api.Delegates;

namespace Todo_api.Services.Implementation;

public class TaskService : ITaskService
{
    private readonly ToDoContext _toDoContext;
    private readonly TaskValidator _taskValidator;
    private readonly TaskNotifier _taskNotifier;
    private readonly TaskDueDateCalculator _dueDateCalculator;

    public TaskService(ToDoContext toDoContext)
    {
        _toDoContext = toDoContext;
        _taskValidator = task => !string.IsNullOrWhiteSpace(task.Description) && task.FechaDeVencimiento > DateTime.Now;
        _taskNotifier = message => Console.WriteLine($"[Notificación]: {message}");
        _dueDateCalculator = dueDate => (dueDate - DateTime.Now).Days;
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

        if (!_taskValidator(task))
            throw new ArgumentException("Tarea inválida.");

        _toDoContext.Tasks.Add(task);
        await _toDoContext.SaveChangesAsync();
        _taskNotifier($"Tarea '{task.Title}' creada.");
    }

    public async Task DeleteTask(int id)
    {
        var task = await _toDoContext.Tasks.FindAsync(id);
        if (task != null)
        {
            _toDoContext.Tasks.Remove(task);
            await _toDoContext.SaveChangesAsync();
            _taskNotifier($"Tarea '{task.Title}' eliminada.");
        }
        else
        {
            throw new KeyNotFoundException($"Tarea con ID {id} no encontrada.");
        }
    }

    public async Task<IEnumerable<object>> GetAllTask()
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
            DiasRestantes = _dueDateCalculator(task.FechaDeVencimiento)
        }).ToList();
    }

    public async Task UpdateTask(int id, TodoTaskDto<string> taskDto)
    {
        var searchedTask = await _toDoContext.Tasks.FirstOrDefaultAsync(t => t.Id == id);
        if (searchedTask == null)
            throw new KeyNotFoundException($"Tarea con ID {id} no encontrada.");

        searchedTask.Title = taskDto.Title;
        searchedTask.Description = taskDto.Description;
        searchedTask.Status = taskDto.Status;
        searchedTask.Prioridad = taskDto.Prioridad;
        searchedTask.GenericValue = taskDto.GenericValue;

        await _toDoContext.SaveChangesAsync();
    }
}
