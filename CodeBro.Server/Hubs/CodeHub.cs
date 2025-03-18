using Microsoft.AspNetCore.SignalR;

namespace CodeBro.Server.Hubs
{
    public class CodeHub : Hub
    {
        public async Task SendCodeChange(string sessionId, string code)
        {
            await Clients.Group(sessionId).SendAsync("ReceiveCodeChange", code);
        }

        public async Task JoinSession(string sessionId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);
            await Clients.Group(sessionId).SendAsync("UserJoined", Context.ConnectionId);
        }
    }
}
