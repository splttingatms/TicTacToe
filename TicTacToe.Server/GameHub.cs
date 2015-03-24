using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using TicTacToe.Server.Models;

namespace TicTacToe.Server
{
    public class GameHub : Hub
    {
        /// <summary>
        /// The starting point for a client looking to join a new game.
        /// </summary>
        /// <param name="username">The friendly name that the user has chosen.</param>
        /// <returns>A Task to track the asynchronous method execution.</returns>
        public async Task FindGame(string username)
        {
            Player joiningPlayer = 
                GameState.Instance.CreatePlayer(username, this.Context.ConnectionId);

            this.Clients.Caller.playerJoined();
            await Task.Factory.StartNew(() => { });
        }

        /// <summary>
        /// A player that is leaving should end all games and notify the opponent.
        /// </summary>
        /// <param name="stopCalled"></param>
        /// <returns></returns>
        public override async Task OnDisconnected(bool stopCalled)
        {
            Player leavingPlayer = GameState.Instance.GetPlayer(playerId: this.Context.ConnectionId);

            Player opponent;
            Game ongoingGame = GameState.Instance.GetGame(leavingPlayer, out opponent);
            if (ongoingGame != null)
            {
                this.Clients.Group(ongoingGame.Id).opponentLeft();
                GameState.Instance.RemoveGame(ongoingGame.Id);
            }
            
            await base.OnDisconnected(stopCalled);
        }
    }
}