namespace Todo_api.Services.Abstraction
{
    public interface IAuthService
    {
        Task<string> Login(string username, string password);  
        Task<string> Register(string username, string password);
    }
}
