using System;

namespace FusionChess
{
    public class PiecePoint
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public PiecePoint SymmetryPoint
        {
            get
            {
                return new PiecePoint(GameParams.BoardWidth2 - X, GameParams.BoardHeight2 - Y);
            }
        }

        public PiecePoint(int x = 0, int y = 0)
        {
            X = x;
            Y = y;
        }

        public PiecePoint GetPointInDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.Left:
                    return GetRelativePoint(-1, 0);
                case Direction.UpperLeft:
                    return GetRelativePoint(-1, 1);
                case Direction.Up:
                    return GetRelativePoint(0, 1);
                case Direction.UpperRight:
                    return GetRelativePoint(1, 1);
                case Direction.Right:
                    return GetRelativePoint(1, 0);
                case Direction.LowerRight:
                    return GetRelativePoint(1, -1);
                case Direction.Down:
                    return GetRelativePoint(0, -1);
                case Direction.LowerLeft:
                    return GetRelativePoint(-1, -1);
                default:
                    throw new Exception();
            }
        }
        public PiecePoint GetRelativePoint(int x, int y)
        {
            return new PiecePoint(X + x, Y + y);
        }

        public override bool Equals(object obj)
        {
            var point = obj as PiecePoint;
            return X == point.X && Y == point.Y;
        }
        public override int GetHashCode()
        {
            var hashCode = -169841274;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            return hashCode;
        }
    }
}
