namespace FusionChess
{
    public class PieceType
    {
        public static readonly PieceType Blank = new PieceType(0);
        public static readonly PieceType King = new PieceType(1);
        public static readonly PieceType Adviser = new PieceType(2);
        public static readonly PieceType Elephant = new PieceType(3);
        public static readonly PieceType Pawn = new PieceType(4);
        public static readonly PieceType Pawn_Pawn = new PieceType(5);
        public static readonly PieceType Cannon = new PieceType(6);
        public static readonly PieceType Cannon_Pawn = new PieceType(7);
        public static readonly PieceType Cannon_Cannon = new PieceType(8);
        public static readonly PieceType Horse = new PieceType(9);
        public static readonly PieceType Horse_Pawn = new PieceType(10);
        public static readonly PieceType Horse_Horse = new PieceType(11);
        public static readonly PieceType Chariot = new PieceType(12);
        public static readonly PieceType Chariot_Pawn = new PieceType(13);
        public static readonly PieceType Chariot_Chariot = new PieceType(14);
        public static readonly PieceType Horse_Cannon = new PieceType(15);
        public static readonly PieceType Cannon_Chariot = new PieceType(16);
        public static readonly PieceType Horse_Chariot = new PieceType(17);

        private byte PieceId { get; set; }
        private PieceType(byte pieceId)
        {
            PieceId = pieceId;
        }

        public static bool operator ==(PieceType type1, PieceType type2)
        {
            return type1.PieceId == type2.PieceId;
        }
        public static bool operator !=(PieceType type1, PieceType type2)
        {
            return type1.PieceId != type2.PieceId;
        }
        public static implicit operator byte(PieceType type)
        {
            return type.PieceId;
        }

        // To fuse piece type.
        public static PieceType operator +(PieceType type1, PieceType type2)
        {
            return FusionRule.GetFusionType(type1, type2);
        }
        // To divide piece type.
        public static PieceType operator -(PieceType type1, PieceType type2)
        {
            if(type1 == type2)
                return Blank;
            var baseTypes = type1.BaseTypes;
            if (baseTypes != null && baseTypes[0] == type2)
                return baseTypes[1];
            return baseTypes[0];
        }

        public override bool Equals(object obj)
        {
            var type = obj as PieceType;
            return PieceId == type.PieceId;
        }
        public override int GetHashCode()
        {
            return -1953110752 + PieceId.GetHashCode();
        }

        public PieceType[] BaseTypes
        {
            get
            {
                return DivisionRule.GetBaseTypes(this);
            }
        }
    }
}
