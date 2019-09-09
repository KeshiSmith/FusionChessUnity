namespace FusionChess
{
    public class PieceColor
    {
        public static readonly PieceColor Red = new PieceColor(true);
        public static readonly PieceColor Black = new PieceColor(false);

        private bool IsRed{ get; set; }
        private PieceColor(bool isRed)
        {
            IsRed = isRed;
        }

        public static PieceColor operator!(PieceColor color)
        {
            return color.IsRed ? Black : Red;
        }
        public static bool operator == (PieceColor color1, PieceColor color2)
        {
            return color1.IsRed == color2.IsRed;
        }
        public static bool operator != (PieceColor color1, PieceColor color2)
        {
            return color1.IsRed != color2.IsRed;
        }

        public override bool Equals(object obj)
        {
            var color = obj as PieceColor;
            return IsRed == color.IsRed;
        }
        public override int GetHashCode()
        {
            return -1736238580 + IsRed.GetHashCode();
        }
    }
}
