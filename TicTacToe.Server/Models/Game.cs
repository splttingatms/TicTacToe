using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe.Server.Models
{
    public class Game
    {
        public Game(Player player1, Player player2, string id)
        {
            this.Player1 = player1;
            this.Player2 = player2;
            this.Id = id;
        }

        /// <summary>
        /// A unique identifier for this game. Also used as the group name.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// One of two partipants of the game.
        /// </summary>
        public Player Player1 { get; set; }

        /// <summary>
        /// One of two participants of the game.
        /// </summary>
        public Player Player2 { get; set; }

        public override string ToString()
        {
            return String.Format("(Id={0}, Player1={1}, Player2={2})",
                this.Id, this.Player1, this.Player2);
        }
    }
}
