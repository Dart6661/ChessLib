namespace MainClasses
{
    public class Field
    {
        private readonly Figure?[,] field = new Figure?[8, 8];
        internal UserSelectionOfReplacement SelectFigure = null!;

        internal Field(Player whitePlayer, Player blackPlayer)
        {
            List<(int, int, Player)> initialPositions = [
                (0, 1,  whitePlayer),
                (7, 6, blackPlayer)
            ];

            foreach (var (figurePosition, pawnPosition, player) in initialPositions)
            {
                InitializeFigure<Rook>(0, figurePosition, player);
                InitializeFigure<Knight>(1, figurePosition, player);
                InitializeFigure<Bishop>(2, figurePosition, player);
                InitializeFigure<Queen>(3, figurePosition, player);
                InitializeFigure<King>(4, figurePosition, player);
                InitializeFigure<Bishop>(5, figurePosition, player);
                InitializeFigure<Knight>(6, figurePosition, player);
                InitializeFigure<Rook>(7, figurePosition, player);

                for (int x = 0; x < 8; x++)
                    InitializeFigure<Pawn>(x, pawnPosition, player);
            }
        }

        private void InitializeFigure<T>(int x, int y, Player player) where T : Figure
        {
            field[x, y] = Activator.CreateInstance(typeof(T), x, y, player) as T;
            player.AddFigure(field[x, y]!);
        }

        public void Move(Figure figure, int x, int y)
        {
            if (x < 0 || x >= 8 || y < 0 || y >= 8)
                throw new InputException("Invalid input");

            Figure? f = GetCell(x, y);
            MoveAction? moveAction = figure.CheckMovement(x, y, f, this);
            moveAction?.ExecuteMove(figure, x, y, this);
        }

        public Figure?[,] GetField()
        {
            Figure?[,] fieldCopy = new Figure?[8, 8];
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++) 
                    fieldCopy[i, j] = field[i, j];
            return fieldCopy;
        }

        public Figure? GetCell(int x, int y)
        {
            if (x < 0 || x >= 8 || y < 0 || y >= 8)
                throw new InputException("Invalid input");

            return  field[x, y];
        }

        internal void Reposition(int a, int b, int x, int y)
        {
            Figure f = GetCell(a, b)!;
            f.Shift(x, y);
            ChangeCell(x, y, GetCell(a, b));
            ChangeCell(a, b, null);
        }

        internal void ChangeCell(int x, int y, Figure? f) => field[x, y] = f;

        public static List<(int, int)> GetPath(int a, int b, int x, int y)
        {
            if (a < 0 || a >= 8 || b < 0 || b >= 8 || x < 0 || x >= 8 || y < 0 || y >= 8)
                throw new InputException("Invalid input");

            List<(int, int)> cells = [];
            if (a == x || b == y)
            {
                int direction = 1;
                int size = Math.Abs(a + b - x - y);
                if (a - x != 0)
                {
                    if (a - x > 0) direction = -1;
                    for (int i = 1; i < size; i++) cells.Add((a + i * direction, b));
                }
                else
                {
                    if (b - y > 0) direction = -1;
                    for (int i = 1; i < size; i++) cells.Add((a, b + i * direction));
                }
            }
            else if (Math.Abs(a - x) == Math.Abs(b - y))
            {
                int direction = 1;
                int size = Math.Abs(a - x);
                if (a - x == b - y)
                {
                    if (a - x > 0) direction = -1;
                    for (int i = 1; i < size; i++) cells.Add((a + i * direction, b + i * direction));
                }
                else
                {
                    if (a - x > 0) direction = -1;
                    for (int i = 1; i < size; i++) cells.Add((a + i * direction, b - i * direction));
                }
            }
            return cells;
        }

        public Figure GetKing(Color color)
        {
            Figure? figure = null;
            foreach (Figure? f in field)
            {
                if (f != null && f is King && f.Color == color)
                {
                    figure = GetCell(f.A, f.B);
                    break;
                }
            }
            return figure!;
        }

        internal bool SelectionMethodIsSet()
        {
            if (SelectFigure == null) 
                throw new ReplacementException("Selection method not assigned");
            return true;
        }
    }


    public static class MoveValidator
    {
        public static bool IsValidMove(int a, int b, int x, int y, Color playerColor, Field field)
        {
            if (a < 0 || a >= 8 || b < 0 || b >= 8 || x < 0 || x >= 8 || y < 0 || y >= 8)
                throw new InputException("Invalid input");

            if (!CheckMovementOnField(a, b, x, y, playerColor, field))
                throw new MovementException("Invalid move");

            return true;
        }

        public static bool CheckMovementOnField(int a, int b, int x, int y, Color movingPlayerColor, Field field)
        {
            if (a < 0 || a >= 8 || b < 0 || b >= 8 || x < 0 || x >= 8 || y < 0 || y >= 8)
                throw new InputException("Invalid input");

            Figure? figure1 = field.GetCell(a, b);
            Figure? figure2 = field.GetCell(x, y);
            if (figure1 != null && figure1.Color == movingPlayerColor && figure1.CheckMovement(x, y, figure2, field) != null)
                return true;
            return false;
        }

        public static bool CellIsSafe(int x, int y, Color color, Field field)
        {
            if (x < 0 || x >= 8 || y < 0 || y >= 8)
                throw new InputException("Invalid input");

            foreach (Figure? f in field.GetField())
            {
                if (f != null && color != f.Color && f.CheckTake(x, y))
                {
                    List<(int x, int y)> cells = Field.GetPath(f.A, f.B, x, y);
                    if (cells.Count == 0 || cells.All(cell => field.GetCell(cell.x, cell.y) == null))
                        return false;
                }
            }
            return true;
        }

        public static bool FigureIsSafeAfterMove(int x, int y, Figure figure, Field field)
        {
            if (x < 0 || x >= 8 || y < 0 || y >= 8)
                throw new InputException("Invalid input");

            foreach (Figure? f in field.GetField())
            {
                if (f != null && figure.Color != f.Color && f.CheckTake(x, y))
                {
                    List<(int x, int y)> cells = Field.GetPath(f.A, f.B, x, y);
                    if (cells.Count == 0 || cells.All(cell => field.GetCell(cell.x, cell.y) == null || field.GetCell(cell.x, cell.y) == figure))
                        return false;
                }
            }
            return true;
        }

        public static bool CellIsSafeAfterMove(int x, int y, Figure figure, int a, int b, Field field)
        {
            if (a < 0 || a >= 8 || b < 0 || b >= 8 || x < 0 || x >= 8 || y < 0 || y >= 8)
                throw new InputException("Invalid input");

            foreach (Figure? f in field.GetField())
            {
                if (f != null && figure.Color != f.Color && f.CheckTake(x, y) && f != field.GetCell(a, b))
                {
                    List<(int x, int y)> cells = Field.GetPath(f.A, f.B, x, y);
                    if (cells.Count == 0 || cells.All(cell => (field.GetCell(cell.x, cell.y) == null || field.GetCell(cell.x, cell.y) == figure) && (cell.x != a || cell.y != b)))
                        return false;
                }
            }
            return true;
        }

        public static bool IsEndOfGame(Player defendingPlayer, Field field)
        {
            foreach (Figure f in defendingPlayer.GetFigures())
            {
                for (int i = 0; i < 8; i++)
                    for (int j = 0; j < 8; j++)
                        if (CheckMovementOnField(f.A, f.B, i, j, f.Color, field)) return false;
            }
            Figure king = field.GetKing(defendingPlayer.Color);
            if (CellIsSafe(king.A, king.B, king.Color, field))
                throw new StaleMateException("The player is stalemated");
            else throw new CheckMateException("The player is checkmated");
        }

        public static bool IsValidCastling(King king, int x, Field field)
        {
            if (x < 0 || x >= 8)
                throw new InputException("Invalid input");

            int direction = (king.A < x) ? 1 : -1;
            int side = (direction == 1) ? 3 : -4;
            Figure? f = field.GetCell(king.A + side, king.B);
            if (f != null && f.Title == Figures.Rook && king.Color == f.Color)
            {
                Rook rook = (Rook)f;
                if (rook.AmountMovesOfFigure == 0 && IsValidMove(rook.A, rook.B, king.A + direction, king.B, rook.Color, field))
                    return true;
            }
            return false;
        }

        public static bool IsValidTakeOnPassage(Pawn p, int x, Field field)
        {
            Figure? f = (p.A < x) ? field.GetCell(p.A + 1, p.B) : field.GetCell(p.A - 1, p.B);
            if (f != null && f.Title == Figures.Pawn && f.AmountMovesOfFigure == 1)
                return true;
            return false;
        }
    }
}