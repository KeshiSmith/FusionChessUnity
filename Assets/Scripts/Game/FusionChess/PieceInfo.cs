namespace FusionChess
{
    public class PieceInfo
    {
        public PieceType PieceType { get; set; }
        public PieceColor PieceColor { get; set; }

        public PieceInfo(PieceType type = null, PieceColor color = null)
        {
            PieceType = type ?? PieceType.Blank;
            PieceColor = color ?? PieceColor.Red;
        }

        public static bool CanEat(PieceInfo info1, PieceInfo info2)
        {
            return info1.PieceColor != info2.PieceColor;
        }
        public static bool CanFuse(PieceInfo info1, PieceInfo info2)
        {
            return FusionRule.CanFuse(info1.PieceType, info2.PieceType);
        }

        // To fuse piece info.
        public static PieceInfo operator +(PieceInfo info1, PieceInfo info2)
        {
            var type = info1.PieceType + info2.PieceType;
            return new PieceInfo(type, info1.PieceColor);
        }
        // To divide piece info.
        public static PieceInfo operator -(PieceInfo info1, PieceInfo info2)
        {
            var type = info1.PieceType - info2.PieceType;
            return new PieceInfo(type, info1.PieceColor);
        }
        // To eat or fuse piece info.
        public static PieceInfo operator *(PieceInfo info1, PieceInfo info2)
        {

            if (info2.IsBlank() || CanEat(info1, info2))
                return info1;
            if (CanFuse(info1, info2))
                return info1 + info2;
            return info1;
        }

        public bool IsBlank()
        {
            return PieceType == PieceType.Blank;
        }
    }
}
