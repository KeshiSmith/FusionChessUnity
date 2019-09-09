using System.Collections.Generic;

namespace FusionChess
{
    public class ChessPiece
    {
        public PiecePoint PiecePoint { get; set; }
        public PieceInfo PieceInfo { get; set; }
        public PieceType[] BaseTypes
        {
            get
            {
                return PieceType.BaseTypes;
            }
        }
        public ChessPiece[] BasePieces
        {
            get
            {
                var baseTypes = BaseTypes;
                if (baseTypes == null)
                    return null;
                var basePieces = new ChessPiece[baseTypes.Length];
                for (int i = 0; i < baseTypes.Length; i++)
                    basePieces[i] = new ChessPiece(PiecePoint, baseTypes[i], PieceColor);
                return basePieces;
            }
        }
        public ChessPiece[] AllBasePieces
        {
            get
            {
                var allPieces = new ChessPiece[3];
                allPieces[1] = this;
                var basePieces = BasePieces;
                if(basePieces != null)
                {
                    allPieces[0] = this;
                    allPieces[1] = basePieces[0];
                    allPieces[2] = basePieces[1];
                }
                return allPieces;
            }
        }

        public ActionArea ActionArea
        {
            get
            {
                var actionArea = ActionAreaRule.GetActionArea(this);
                return actionArea;
            }
        }
        public ActionArea RelativeArea
        {
            get
            {
                var actionArea = ActionArea;
                var relativeArea = IsRed() ? actionArea : actionArea.SymmetryArea;
                return relativeArea;
            }
        }
        public PiecePoint RelativePoint
        {
            get
            {
                var relativePoint = IsRed() ? PiecePoint : PiecePoint.SymmetryPoint;
                return relativePoint;
            }
        }
        public PieceType PieceType
        {
            get
            {
                return PieceInfo.PieceType;
            }
        }
        public PieceColor PieceColor
        {
            get
            {
                return PieceInfo.PieceColor;
            }
        }

        public ChessPiece(PiecePoint point, PieceInfo info)
        {
            PiecePoint = point;
            PieceInfo = info;
        }
        public ChessPiece(PiecePoint point, PieceType type, PieceColor color) : this(point, new PieceInfo(type, color))
        {
        }

        public bool IsRed()
        {
            return PieceColor == PieceColor.Red;
        }
        public bool HasPassedRiver()
        {
            return RelativePoint.Y > 4;
        }
        public bool CantainsPieceType(PieceType type)
        {
            var baseTypes = BaseTypes;
            return (PieceType == type || (baseTypes != null && (baseTypes[0] == type || baseTypes[1] == type)));
        }
        public bool CanEat(PieceInfo info)
        {
            return PieceInfo.CanEat(PieceInfo, info);
        }
        public bool CanFuse(PieceInfo info)
        {
            return PieceInfo.CanFuse(PieceInfo, info);
        }
        public PieceInfo Divide(PieceInfo info)
        {
            return PieceInfo - info;
        }
        public PieceInfo EatOrFusePiece(PieceInfo info)
        {
            return PieceInfo * info;
        }
        public HashSet<PiecePoint> GetActionPoints(ChessBoard board, bool addition = false)
        {
            var actionWay = ActionWayRule.GetActionWay(PieceType);
            return actionWay.GetActionPoints(board, this, addition);
        }
        public HashSet<PiecePoint> GetAllActionPoints(ChessBoard board, bool addition = false)
        {
            var actionPoints = new HashSet<PiecePoint>();
            var basePieces = BasePieces;
            if (basePieces != null)
                foreach (var piece in basePieces)
                    actionPoints.UnionWith(piece.GetActionPoints(board, addition));
            else
                actionPoints = GetActionPoints(board, addition);
            return actionPoints;
        }
    }
}
