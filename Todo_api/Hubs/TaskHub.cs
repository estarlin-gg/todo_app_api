using Microsoft.AspNetCore.SignalR;

namespace Todo_api.Hubs
{
    public class TaskHub : Hub
    {
        public async Task SendTaskNotification(string message)
        {
            await Clients.All.SendAsync("ReceiveTaskNotification", message);
        }
    }
}
