using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace AdmIn.API.Hubs
{
    public class NotificationHub : Hub
    {
        // Hub intentionally minimal; server will send messages to specific users
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(System.Exception? exception)
        {
            return base.OnDisconnectedAsync(exception);
        }
    }
}
