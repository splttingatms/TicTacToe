using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicTacToe.Server.Models;

namespace TicTacToe.Server
{
    /// <summary>
    /// This class can statically persist a collection of players and
    /// matches that each of the players are playing using the singleton pattern.
    /// The singleton pattern restricts the instantiation of the class to one object.
    /// </summary>
    public class GameState
    {
        /// <summary>
        /// Singleton instance that defers initialization until access time.
        /// </summary>
        private readonly static Lazy<GameState> instance =
            new Lazy<GameState>(() => new GameState(GlobalHost.ConnectionManager.GetHubContext<GameHub>()));

        /// <summary>
        /// A reference to all players. Key is the username of the player.
        /// Note that this collection is concurrent to handle multiple threads.
        /// </summary>
        private readonly ConcurrentDictionary<string, Player> players =
            new ConcurrentDictionary<string, Player>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// A reference to all games. Key is the group name of the game.
        /// Note that this collection uses a concurrent dictionary to handle multiple threads.
        /// </summary>
        private readonly ConcurrentDictionary<string, Game> games =
            new ConcurrentDictionary<string, Game>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// A queue of games that do not have a full party yet. Players looking for games 
        /// should join these pending games.
        /// </summary>
        private readonly ConcurrentQueue<Game> pendingGames =
            new ConcurrentQueue<Game>();

        private GameState(IHubContext context)
        {
            this.Clients = context.Clients;
            this.Groups = context.Groups;
        }

        public static GameState Instance
        {
            get { return instance.Value; }
        }

        public IHubConnectionContext<dynamic> Clients { get; set; }

        public IGroupManager Groups { get; set; }

        public Player CreatePlayer(string username, string connectionId)
        {
            var player = new Player(username, connectionId);
            this.players[username] = player;

            return player;
        }
    }
}
