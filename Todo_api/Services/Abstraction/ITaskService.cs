using Todo_api.Dtos;

namespace Todo_api.Services.Abstraction
{
    public interface ITaskService
    {
        Task<IEnumerable<object>> GetAllTasks();
        void EnqueueTaskCreation(TodoTaskDto<string> task);
        void EnqueueTaskUpdate(int id, TodoTaskDto<string> task);
        void EnqueueTaskDeletion(int id);
    }
}
