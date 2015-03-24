using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;

namespace TicTacToe.Server
{
    public class GameHub : Hub
    {
        public async Task FindGame(string username)
        {
            this.Clients.Caller.playerJoined();
            await Task.Factory.StartNew(() => { });
        }
    }
}