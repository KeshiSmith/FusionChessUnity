using System.Collections.Generic;

namespace FusionChess
{
    public class ActionWayData
    {
        public Direction[][] WayDirections { get; private set; }
        public Direction[] EyeDirections { get; private set; }
        public bool IsStepPiece { get; private set; }
        public bool IsCannon { get; private set; }
        public bool IsKing { get; private set; }

        public ActionWayData(
            Direction[][] wayDirections,
            Direction[] eyeDirections =null,
            bool isStepPiece = true,
            bool isCannon = false,
            bool isKing = false)
        {
            WayDirections = wayDirections;
            EyeDirections = eyeDirections;
            IsStepPiece = isStepPiece;
            IsCannon = isCannon;
            IsKing = isKing;
        }
    }

    public class ActionWay
    {
        private enum LegalFlag
        {
            None = 0x00,
            Blank = 0x01,
            Eatting = 0x02,
            Fusion = 0x04,
        }

        private ActionWayData ActionWayData { get; set; }

        public ActionWay(ActionWayData actionWayData = null)
        {
            ActionWayData = actionWayData;
        }

        // addtion 为 true 时, 返回结果包括 马腿 象眼 炮架.
        public HashSet<PiecePoint> GetActionPoints(ChessBoard board, ChessPiece piece, bool addition = false)
        {
            if (ActionWayData != null)
                return GetActionPoints(
                    board,
                    piece,
                    ActionWayData.WayDirections,
                    ActionWayData.EyeDirections,
                    ActionWayData.IsStepPiece,
                    ActionWayData.IsCannon,
                    ActionWayData.IsKing,
                    addition);

            var actionPoints = new HashSet<PiecePoint>();
            var baseTypes = piece.BaseTypes ?? new PieceType[0];
            foreach (var type in baseTypes)
            {
                var actionWay = ActionWayRule.GetActionWay(type);
                actionPoints.UnionWith(actionWay.GetActionPoints(board, piece, addition));
            }
            return actionPoints;
        }

        private static HashSet<PiecePoint> GetActionPoints(
            ChessBoard board,
            ChessPiece piece,
            Direction[][] wayDirections,
            Direction[] eyeDirections = null,
            bool isStepPiece = true,
            bool isCannon = false,
            bool isKing = false,
            bool addition = false)
        {
            var actionWays = new HashSet<PiecePoint>();
            if (eyeDirections == null)
                actionWays.UnionWith(GetActionPointsAroundEye(
                    board,
                    piece,
                    wayDirections[0],
                    piece.PiecePoint,
                    isStepPiece,
                    isCannon,
                    isKing,
                    addition));
            else for (var i = 0; i < eyeDirections.Length; i++)
            {
                var eyePoint = piece.PiecePoint.GetPointInDirection(eyeDirections[i]);
                var flag = GetLegalFlag(board, piece, eyePoint);
                if (addition && IsLegal(flag)) actionWays.Add(eyePoint);
                if (IsLegalPoint(flag))
                {
                    if (addition) actionWays.Add(eyePoint);
                    actionWays.UnionWith(GetActionPointsAroundEye(
                        board,
                        piece,
                        wayDirections[i],
                        eyePoint,
                        true,
                        false,
                        false,
                        addition));
                }
            }
            return actionWays;
        }
        private static HashSet<PiecePoint> GetActionPointsAroundEye(
            ChessBoard board,
            ChessPiece piece,
            Direction[] wayDirections,
            PiecePoint eyePoint,
            bool isStepPiece = true,
            bool isCannon = false,
            bool isKing = false,
            bool addition = false)
        {
            var actionWays = new HashSet<PiecePoint>();
            foreach (var wayDirection in wayDirections)
            {
                var wayPoint = eyePoint ?? piece.PiecePoint;
                do
                {
                    wayPoint = wayPoint.GetPointInDirection(wayDirection);
                    var flag = GetLegalFlag(board, piece, wayPoint);
                    if (!IsLegal(flag)|| (flag == LegalFlag.Eatting && isCannon && !addition))
                        break;
                    actionWays.Add(wayPoint);
                    if(IsLegalPointNotBlank(flag))
                        break;
                } while (!isStepPiece);
                if (isCannon)
                {
                    do
                        wayPoint = wayPoint.GetPointInDirection(wayDirection);
                    while (IsLegalPoint(board, piece, wayPoint));
                    if (IsLegalPointNotBlank(board, piece, wayPoint))
                        actionWays.Add(wayPoint);
                }
            }
            if (isKing)
            {
                var pieceInfo = new PieceInfo();
                var wayPoint = piece.PiecePoint;
                var direction = wayPoint.Y < 5 ? Direction.Up : Direction.Down;
                do
                {
                    wayPoint = wayPoint.GetPointInDirection(direction);
                    if (!ActionAreaRule.FullArea.IsPointInArea(wayPoint))
                        break;
                    pieceInfo = board.GetPieceInfo(wayPoint);
                } while (pieceInfo.IsBlank());
                if (pieceInfo.PieceType == PieceType.King)
                    actionWays.Add(wayPoint);
            }
            return actionWays;
        }

        private static LegalFlag GetLegalFlag(ChessBoard board, ChessPiece piece, PiecePoint point)
        {
            LegalFlag flag = LegalFlag.None;
            if (piece.ActionArea.IsPointInArea(point))
            {
                var wayPieceInfo = board.GetPieceInfo(point);
                if (wayPieceInfo.IsBlank())
                    flag = LegalFlag.Blank;
                else if (piece.CanEat(wayPieceInfo))
                    flag = LegalFlag.Eatting;
                else if (piece.CanFuse(wayPieceInfo))
                    flag = LegalFlag.Fusion;
            }
            return flag;
        }
        private static bool IsLegal(LegalFlag flag)
        {
            return flag != LegalFlag.None;
        }
        private static bool IsLegalPoint(LegalFlag flag)
        {
            return flag == LegalFlag.Blank;
        }
        private static bool IsLegalPoint(ChessBoard board, ChessPiece piece, PiecePoint point)
        {
            var flag = GetLegalFlag(board, piece, point);
            return IsLegalPoint(flag);
        }
        private static bool IsLegalPointNotBlank(LegalFlag flag)
        {
            return flag == LegalFlag.Eatting || flag == LegalFlag.Fusion;
        }
        private static bool IsLegalPointNotBlank(ChessBoard board, ChessPiece piece, PiecePoint point)
        {
            var flag = GetLegalFlag(board, piece, point);
            return IsLegalPointNotBlank(flag);
        }
    }
}
