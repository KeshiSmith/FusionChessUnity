namespace FusionChess
{
    public class ChessRecord
    {
        public ChessPiece SelectPiece { get; private set; }
        public ChessPiece TargetPiece { get; private set; }
        public ChessRecord ReverseRecord
        {
            get
            {
                return new ChessRecord(TargetPiece, SelectPiece);
            }
        }

        public ChessRecord(ChessPiece selectPiece, ChessPiece targetPiece)
        {
            SelectPiece = selectPiece;
            TargetPiece = targetPiece;
        }
    }
}
