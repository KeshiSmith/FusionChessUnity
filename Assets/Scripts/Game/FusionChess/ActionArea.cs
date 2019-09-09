namespace FusionChess
{
    public class ActionArea
    {
        public int MinX { get; private set; }
        public int MaxX { get; private set; }
        public int MinY { get; private set; }
        public int MaxY { get; private set; }
        public ActionArea SymmetryArea
        {
            get
            {
                return new ActionArea(MinX, MaxX, GameParams.BoardHeight2 - MaxY, GameParams.BoardHeight2 - MinY);
            }
        }

        public ActionArea(int minX, int maxX, int minY, int maxY)
        {
            MinX = minX;
            MaxX = maxX;
            MinY = minY;
            MaxY = maxY;
        }

        public bool IsPointInArea(PiecePoint point)
        {
            return point.X >= MinX && point.X <= MaxX && point.Y >= MinY && point.Y <= MaxY;
        }
    }
}
