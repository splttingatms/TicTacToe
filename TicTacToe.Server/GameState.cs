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
        /// A reference to all players. Key is the unique ID of the player.
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
        /// A queue of players that are waiting for an opponent.
        /// </summary>
        private readonly ConcurrentQueue<Player> waitingPlayers =
            new ConcurrentQueue<Player>();

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
            this.players[connectionId] = player;

            return player;
        }

        /// <summary>
        /// Retrieves the player that has the given ID.
        /// </summary>
        /// <param name="playerId">The unique identifier of the player to find.</param>
        /// <returns>The found player; otherwise null.</returns>
        public Player GetPlayer(string playerId)
        {
            Player foundPlayer;
            if (!this.players.TryGetValue(playerId, out foundPlayer))
            {
                return null;
            }

            return foundPlayer;
        }

        /// <summary>
        /// Retrieves the game that the given player is playing in.
        /// </summary>
        /// <param name="playerId">The player in the game.</param>
        /// <param name="opponent">The opponent of the player if there is one; otherwise null.</param>
        /// <returns>The game that the specified player is a member of if game is found; otherwise null.</returns>
        public Game GetGame(Player player, out Player opponent)
        {
            opponent = null;
            Game foundGame = this.games.Values.FirstOrDefault(g => g.Id == player.GameId);

            if (foundGame == null)
            {
                return null;
            }

            opponent = (player.Id == foundGame.Player1.Id) ?
                foundGame.Player2 :
                foundGame.Player1;

            return foundGame;
        }

        /// <summary>
        /// Retrieves a game waiting for players.
        /// </summary>
        /// <returns>Returns a pending game if any; otherwise returns null.</returns>
        public Player GetWaitingOpponent()
        {
            Player foundPlayer;
            if (!this.waitingPlayers.TryDequeue(out foundPlayer))
            {
                return null;
            }

            return foundPlayer;
        }

        /// <summary>
        /// Forgets the specified game. Use if the game is over.
        /// No need to manually remove a user from a group when the connection ends.
        /// </summary>
        /// <param name="gameId">The unique identifier of the game.</param>
        /// <returns>A task to track the asynchronous method execution.</returns>
        public void RemoveGame(string gameId)
        {
            // Remove the game
            Game foundGame;
            if (!this.games.TryRemove(gameId, out foundGame))
            {
                throw new InvalidOperationException("Game not found.");
            }

            // Remove the players, best effort
            Player foundPlayer;
            this.players.TryRemove(foundGame.Player1.Id, out foundPlayer);
            this.players.TryRemove(foundGame.Player2.Id, out foundPlayer);
        }

        /// <summary>
        /// Adds specified player to the waiting pool.
        /// </summary>
        /// <param name="player">The player to add to waiting pool.</param>
        public void AddToWaitingPool(Player player)
        {
            this.waitingPlayers.Enqueue(player);
        }

        /// <summary>
        /// Determines if the username is already taken, ignoring case.
        /// </summary>
        /// <param name="username">The username to check.</param>
        /// <returns>true if another player shares the same username; otherwise false.</returns>
        public bool IsUsernameTaken(string username)
        {
            return this.players.Values.FirstOrDefault(player => player.Name.Equals(username, StringComparison.InvariantCultureIgnoreCase)) != null;
        }

        /// <summary>
        /// Creates a new pending game which will be waiting for more players.
        /// </summary>
        /// <param name="joiningPlayer">The first player to enter the game.</param>
        /// <returns>The newly created game in a pending state.</returns>
        public async Task<Game> CreateGame(Player firstPlayer, Player secondPlayer)
        {
            // Define the new game and add to waiting pool
            Game game = new Game(firstPlayer, secondPlayer);
            this.games[game.Id] = game;

            // Create a new group to manage communication using ID as group name
            await this.Groups.Add(firstPlayer.Id, groupName: game.Id);
            await this.Groups.Add(secondPlayer.Id, groupName: game.Id);

            return game;
        }
    }
}
