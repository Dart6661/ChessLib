using System.Collections.Generic;

namespace MainClasses
{
    public enum Figures
    {
        King,
        Queen,
        Rook,
        Bishop,
        Knight,
        Pawn
    }


    public enum Color
    {
        White,
        Black
    }


    public class ChessMove
    {
        internal List<(int, int, int, int)> whiteMoves = [];
        internal List<(int, int, int, int)> blackMoves = [];

        internal void SetCoordinates(List<(int, int, int, int)> moves, Color color)
        {
            if (color == Color.White) whiteMoves = moves;
            else blackMoves = moves;
        }

        public Color ColorOfMovingPlayer() => (whiteMoves.Count == 0 || whiteMoves.Count != 0 && blackMoves.Count != 0) ? Color.White : Color.Black;
        
        public IReadOnlyList<(int, int, int, int)> GetWhiteMoves() => whiteMoves.AsReadOnly();

        public IReadOnlyList<(int, int, int, int)> GetBlackMoves() => blackMoves.AsReadOnly();
    }


    public delegate Figure UserSelectionOfReplacement(Pawn p);

    public class InputException(string message) : Exception(message);

    public class CheckMateException(string message) : Exception(message);

    public class StaleMateException(string message) : Exception(message);

    public class MovementException(string message) : Exception(message);

    public class ReplacementException(string message) : Exception(message);
}
