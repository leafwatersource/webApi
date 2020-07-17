using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PmWebApi.SignalR
{
    public class RateFactotyChatHub : Hub
    {
        public static List<string> aaa = new List<string>();
        public async Task SendMessage(string user, string message)
        {
            aaa.Add(Context.ConnectionId);
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task OnConnected()
        {
            await base.OnConnectedAsync();
        }

        public async Task OnDisconnected()
        {
            //custom logic here
            Exception e = null;
            await base.OnDisconnectedAsync(e);
        }
    }
}
