namespace Todo_api.Models
{
    public class TodoTask<T>
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime FechaDeVencimiento { get; set; }
        public string Status { get; set; }
        public string Prioridad { get; set; }
        public T GenericValue { get; set; }



    }
}
