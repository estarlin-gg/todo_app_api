using Todo_api.Models;

namespace Todo_api.Factories
{
    public static class TodoTaskFactory
    {
        public static TodoTask<string> CreateHighPriorityTask(string title, string description, string genericValue)
        {
            return new TodoTask<string>
            {
                Title = title,
                Description = description,
                FechaDeVencimiento = DateTime.Now.AddDays(3), 
                Status = "Pendiente",
                Prioridad = "Alta",
                GenericValue = genericValue
            };
        }

        public static TodoTask<string> CreateLowPriorityTask(string title, string description, string genericValue)
        {
            return new TodoTask<string>
            {
                Title = title,
                Description = description,
                FechaDeVencimiento = DateTime.Now.AddDays(10), 
                Status = "Pendiente",
                Prioridad = "Baja",
                GenericValue = genericValue
            };
        }
    }
}
