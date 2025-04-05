namespace MainClasses
{
    public class GameHandler
    {
        public Player whitePlayer = new(Color.White);
        public Player blackPlayer = new(Color.Black);
        public Field field;


        public GameHandler()
        {
            field = new Field(whitePlayer, blackPlayer);
        }

        public void MakeMove(int a, int b, int x, int y)
        {
            Player movingPlayer = GetMovingPlayer();
            Player defendingPlayer = GetDefendingPlayer();
            MoveValidator.IsValidMove(a, b, x, y, movingPlayer.Color, field);
            Figure f = field.GetCell(a, b)!;
            field.Move(f, x, y);
            movingPlayer.IncrementMoves();
            MoveValidator.IsEndOfGame(defendingPlayer, field);
        }

        public void SetFigureSelection(UserSelectionOfReplacement method) => field.SelectFigure = method;

        public Player GetMovingPlayer() => whitePlayer.AmountMovesOfPlayer == blackPlayer.AmountMovesOfPlayer ? whitePlayer : blackPlayer;

        public Player GetDefendingPlayer() => whitePlayer.AmountMovesOfPlayer > blackPlayer.AmountMovesOfPlayer ? whitePlayer : blackPlayer;
    }
}
