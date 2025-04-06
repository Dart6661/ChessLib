using MainClasses;

namespace ConsoleChess
{
    public class Game
    {
        public void Start()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            GameHandler gameHandler = new();
            gameHandler.SetFigureSelection(SelectFigure);
            DisplayField(gameHandler.field);
            while (true)
            {
                try
                {
                    string? coord1 = Console.ReadLine();
                    string? coord2 = Console.ReadLine();
                    int a = 0, b = 0, x = 0, y = 0;

                    ConverCoordinate(coord1, ref a, ref b);
                    ConverCoordinate(coord2, ref x, ref y);

                    gameHandler.MakeMove(a, b, x, y);

                    Console.Clear();
                    DisplayField(gameHandler.field);
                    DisplayMoves(gameHandler.field);
                }
                catch (InputException ex) { Console.WriteLine(ex.Message); } 
                catch (MovementException ex) { Console.WriteLine(ex.Message); }
                catch (ReplacementException ex) { Console.WriteLine(ex.Message); }
                catch (CheckMateException ex) { Console.WriteLine(ex.Message); break; }
                catch (StaleMateException ex) { Console.WriteLine(ex.Message); break; }
            }
            DisplayField(gameHandler.field);
        }

        public void ConverCoordinate(string? coord, ref int x, ref int y)
        {
            if (!IsValidCoordinate(coord))
                throw new InputException("invalid input");

            x = coord![0] - 'a';
            y = coord![1] - '1';
        }

        public bool IsValidCoordinate(string? coordinate) => (coordinate != null && coordinate.Length == 2 && coordinate[0] >= 'a' && coordinate[0] <= 'h' && coordinate[1] >= '1' && coordinate[1] <= '8');

        public void DisplayField(Field field)
        {
            int indent = 2;
            for (int y = 7; y >= 0; y--)
            {
                for (int x = 0; x < indent; x++) Console.Write(" ");
                Console.Write(y + 1);
                for (int x = 0; x < 8; x++)
                {
                    var f = field.GetCell(x, y);
                    //if (f != null) Console.Write("[" + Convert.ToString(f.Title)![0] + "]");
                    if (f != null) Console.Write("[" + GetFigureSymbol(f.Title, f.Color) + "]");
                    else Console.Write("[ ]");
                }
                Console.WriteLine();
            }
            for (int x = 0; x < indent + 1; x++) Console.Write(" ");
            for (int x = 0; x < 8; x++) Console.Write(" " + (char)('a' + x) + " ");
            Console.WriteLine();
        }

        public void DisplayMoves(Field field)
        {
            foreach (var move in field.GetMoves())
            {
                Console.Write($"{move.Key}: ");
                Console.Write("white moves: ");
                foreach ((int, int, int, int) coord in move.Value.GetWhiteMoves()) Console.Write($"{coord} ");
                Console.Write("black moves: ");
                foreach ((int, int, int, int) coord in move.Value.GetBlackMoves()) Console.Write($"{coord} ");
                Console.WriteLine();
            }
        } 

        public string GetFigureSymbol(Figures figure, Color color)
        {
            string symbol = figure switch
            {
                Figures.King => color == Color.White ? "♚" : "♔",
                Figures.Queen => color == Color.White ? "♛" : "♕",
                Figures.Rook => color == Color.White ? "♜" : "♖",
                Figures.Bishop => color == Color.White ? "♝" : "♗",
                Figures.Knight => color == Color.White ? "♞" : "♘",
                Figures.Pawn => color == Color.White ? "♟" : "♙",
                _ => throw new InputException("Invalid input"),
            };
            return symbol;
        }

        public void DisplayPlayerFigures(Player player)
        {
            for (int i = 0; i < player.CountFigures(); i++) 
                Console.WriteLine(player.GetFigures()[i]);
        }

        public Figure SelectFigure(Pawn p)
        {
            int x = p.A, y = p.B;
            Player owner = p.Owner;
            string? f = Console.ReadLine();
            Figure figure = f switch
            {
                "Q" => new Queen(x, y, owner),
                "R" => new Rook(x, y, owner),
                "B" => new Bishop(x, y, owner),
                "K" => new Knight(x, y, owner),
                _ => throw new InputException("Invalid input"),
            };
            return figure;
        }
    }
}
