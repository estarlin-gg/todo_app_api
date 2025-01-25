using Todo_api.Models;
namespace Todo_api.Services.Abstraction
{
    public interface ITaskService
    {
        Task CreateTask(TodoTask<string> task);
        Task<IEnumerable<TodoTask<string>>>GetAllTask();

        Task UpdateTask(int id,TodoTask<string> task);

        Task DeleteTask(int id);    
    }
}
