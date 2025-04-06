namespace MainClasses
{
    abstract class MoveAction
    {
        internal abstract void ExecuteMove(Figure figure, int x, int y, Field field);
    }


    internal class RegularMoveAction : MoveAction
    {
        internal override void ExecuteMove(Figure figure, int x, int y, Field field)
        {
            field.AddMove([(figure.A, figure.B, x, y)]);
            field.GetCell(x, y)?.RemoveFromPlayer();
            field.Reposition(figure.A, figure.B, x, y);
        }
    }


    internal class CastligMoveAction : MoveAction
    {
        internal override void ExecuteMove(Figure figure, int x, int y, Field field)
        {
            int direction = (figure.A < x) ? figure.A + 1 : figure.A - 1;
            int side = (direction == figure.A + 1) ? figure.A + 3 : figure.A - 4;
            field.AddMove([(figure.A, figure.B, x, y), (side, figure.B, direction, figure.B)]);
            field.Reposition(figure.A, figure.B, x, y);
            field.Reposition(side, figure.B, direction, figure.B);
        }
    }


    internal class TakeOnPassageMoveAction : MoveAction
    {
        internal override void ExecuteMove(Figure figure, int x, int y, Field field)
        {
            int side = (figure.A < x) ? figure.A + 1 : figure.A - 1;
            field.AddMove([(figure.A, figure.B, x, y)]);
            field.GetCell(side, figure.B)?.RemoveFromPlayer();
            field.ChangeCell(side, figure.B, null);
            field.Reposition(figure.A, figure.B, x, y);
        }
    }


    internal class ReplacementMoveAction : MoveAction
    {
        internal override void ExecuteMove(Figure figure, int x, int y, Field field)
        {
            Figure f = field.SelectFigure((Pawn)figure);
            if (f.Title != Figures.Queen && f.Title != Figures.Rook && f.Title != Figures.Bishop && f.Title != Figures.Knight)
                throw new ReplacementException("The choice is incorrect");

            field.AddMove([(f.A, f.B, x, y)]);
            Player player = figure.Owner;
            for (int i = 0; i < player.CountFigures(); i++)
                if (player.GetFigures()[i] == field.GetCell(figure.A, figure.B))
                {
                    player.ReplaceFigure(f, i);
                    field.ChangeCell(f.A, f.B, f);
                    break;
                }

            field.Reposition(f.A, f.B, x, y);
        }
    }
}
