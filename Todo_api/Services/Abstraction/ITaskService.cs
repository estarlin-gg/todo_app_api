using Todo_api.Dtos;
using Todo_api.Models;
namespace Todo_api.Services.Abstraction
{
    public interface ITaskService
    {


        Task CreateTask(TodoTaskDto<string> task);
        Task<IEnumerable<TodoTask<string>>> GetAllTask();

        Task UpdateTask(int id, TodoTaskDto<string> task);

        Task DeleteTask(int id);    
    }
}
