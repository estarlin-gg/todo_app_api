using System.Collections.Concurrent;

namespace Todo_api.Services.Implementation
{
    public class TaskQueueService
    {
        private readonly ConcurrentQueue<Func<Task>> _taskQueue = new();
        private bool _isProcessing;

        public void EnqueueTask(Func<Task> task)
        {
            _taskQueue.Enqueue(task);
            ProcessQueue();
        }

        private async void ProcessQueue()
        {
            if (_isProcessing) return;

            _isProcessing = true;

            while (_taskQueue.TryDequeue(out var task))
            {
                await ProcessTask(task);
            }

            _isProcessing = false;
        }

        private async Task ProcessTask(Func<Task> task)
        {
            try

            {
                await task();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en la tarea: {ex.Message}");
            }
        }
    }
}
