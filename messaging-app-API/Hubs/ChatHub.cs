using messaging_app_API.Services;
using Microsoft.AspNetCore.SignalR;

namespace messaging_app_API.Hubs
{
    public class ChatHub : Hub 
    {
        private readonly ChatService _chatService;
        public ChatHub(ChatService chatService)
        {
            _chatService = chatService;
        }
        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Come2Chat");
            await Clients.Caller.SendAsync("User Connected");
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Come2Chat");
            await base.OnDisconnectedAsync(exception);
        }
    }

}
