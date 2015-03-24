using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe.Server.Models
{
    /// <summary>
    /// Represents a user playing the game.
    /// </summary>
    public class Player
    {
        public Player(string name, string id)
        {
            this.Name = name;
            this.Id = id;
        }

        /// <summary>
        /// A user defined username to display as a friendly name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A unique ID to identify a player. Use the SignalR connection ID.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The unique ID to identify the game a player is playing; otherwise null.
        /// </summary>
        public string GameId { get; set; }

        public override string ToString()
        {
            return String.Format("(Id={0}, Name={1}, GameId={2})", 
                this.Id, this.Name, this.GameId);
        }
    }
}
