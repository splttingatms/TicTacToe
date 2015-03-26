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
        /// <summary>
        /// The number of pieces that have been placed on the board.
        /// </summary>
        private int totalPiecesPlaced;

        public Board()
        {
            // TODO: Make the dimensions of the board constants
            this.Pieces = new string[3, 3];
        }

        /// <summary>
        /// Represents the pieces on the board.
        /// Must be publicly accessible to allow serialization.
        /// </summary>
        public string[,] Pieces { get; private set; }


        /// <summary>
        /// Determines whether there are three pieces in a row that match.
        /// Possible configurations are either horizontal, vertical, or the diagonals.
        /// </summary>
        public bool IsThreeInRow
        {
            get
            {
                // Check all rows
                for (int row = 0; row < this.Pieces.GetLength(0); row++)
                {
                    if (!string.IsNullOrWhiteSpace(Pieces[row, 0]) &&
                        Pieces[row, 0] == Pieces[row, 1] &&
                        Pieces[row, 1] == Pieces[row, 2])
                    {
                        return true;
                    }
                }

                // Check all columns
                for (int col = 0; col < this.Pieces.GetLength(1); col++)
                {
                    if (!string.IsNullOrWhiteSpace(Pieces[0, col]) &&
                        Pieces[0, col] == Pieces[1, col] &&
                        Pieces[1, col] == Pieces[2, col])
                    {
                        return true;
                    }
                }

                // Check forward-diagonal
                if (!string.IsNullOrWhiteSpace(Pieces[1, 1]) &&
                    Pieces[2, 0] == Pieces[1, 1] &&
                    Pieces[1, 1] == Pieces[0, 2])
                {
                    return true;
                }

                // Check backward-diagonal
                if (!string.IsNullOrWhiteSpace(Pieces[1, 1]) &&
                    Pieces[0, 0] == Pieces[1, 1] &&
                    Pieces[1, 1] == Pieces[2, 2])
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Returns whether there are any positions left on the board.
        /// </summary>
        public bool AreSpacesLeft
        {
            get
            {
                return this.totalPiecesPlaced < this.Pieces.Length;
            }
        }

        public void PlacePiece(int row, int col, string pieceToPlace)
        {
            this.Pieces[row, col] = pieceToPlace;
            this.totalPiecesPlaced++;
        }

        public override string ToString()
        {
            return string.Join(", ", this.Pieces);
        }
    }
}
