using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe.Server.Models
{
    /// <summary>
    /// Represents a Tic-Tac-Toe board and where players have placed their pieces.
    /// </summary>
    public class Board
    {
        public string[,] Pieces { get; private set; }

        public Board()
        {
            this.Pieces = new string[3, 3];
        }

        public override string ToString()
        {
            return string.Join(", ", this.Pieces);
        }
    }
}
