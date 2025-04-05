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


    public delegate Figure UserSelectionOfReplacement(Pawn p);

    public class InputException(string message) : Exception(message);

    public class CheckMateException(string message) : Exception(message);

    public class StaleMateException(string message) : Exception(message);

    public class MovementException(string message) : Exception(message);

    public class ReplacementException(string message) : Exception(message);
}
