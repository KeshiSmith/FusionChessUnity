using System.Collections.Generic;

namespace FusionChess
{
    public class GameParams
    {
        // Board Size (Count From One)
        public const int BoardWidth = 9;
        public const int BoardHeight = 10;
        // Board Size (Count From Zero)
        public const int BoardWidth2 = BoardWidth - 1;
        public const int BoardHeight2 = BoardHeight - 1;
        // Game Mode
        public static bool fusionMode = false;
        public static bool hiddenMode = false;
        // flag
        public static bool animeFlag = true;
        public static bool selectFlag = true;
    }

    public class ActionAreaRule
    {
        public static readonly ActionArea FullArea = new ActionArea(0, GameParams.BoardWidth2, 0, GameParams.BoardHeight2);
        private static readonly ActionArea KingAndAdviserArea = new ActionArea(3, 5, 0, 2);
        private static readonly ActionArea ElephantArea = new ActionArea(0, GameParams.BoardWidth2, 0, 4);

        private static readonly Dictionary<PieceType, ActionArea> ActionAreaRules
            = new Dictionary<PieceType, ActionArea>()
        {
            { PieceType.King, KingAndAdviserArea },
            { PieceType.Adviser, KingAndAdviserArea },
            { PieceType.Elephant, ElephantArea },
        };

        public static ActionArea GetActionArea(ChessPiece piece)
        {
            ActionArea relativeArea;
            var pieceType = piece.PieceType;
            if (ActionAreaRules.ContainsKey(pieceType))
                relativeArea = ActionAreaRules[pieceType];
            else if (piece.CantainsPieceType(PieceType.Pawn))
                relativeArea = GetRelativeAreaOfPawn(piece);
            else
                relativeArea = FullArea;
            var actionArea = piece.IsRed() ? relativeArea : relativeArea.SymmetryArea;
            return actionArea;
        }

        private static ActionArea GetRelativeAreaOfPawn(ChessPiece piece)
        {
            var y = piece.RelativePoint.Y;
            var minY = piece.HasPassedRiver() ? y : y + 1;
            var relativeArea = new ActionArea(0, GameParams.BoardWidth2, minY, GameParams.BoardHeight2);
            return relativeArea;
        }
    }

    public class ActionWayRule
    {
        private static readonly ActionWay DefaultActionWay = new ActionWay();
        private static readonly Direction[] NormalDirection
            = new Direction[] { Direction.Left, Direction.Up, Direction.Right, Direction.Down };
        private static readonly Direction[] SlopeDirection
            = new Direction[] { Direction.UpperLeft, Direction.UpperRight, Direction.LowerRight, Direction.LowerLeft };
        private static readonly Dictionary<PieceType, ActionWay> ActionWayRules
        = new Dictionary<PieceType, ActionWay>()
        {
            { PieceType.King, new ActionWay(
                new ActionWayData(
                    new Direction[][] { NormalDirection },
                    null,
                    true,
                    false,
                    true)) },
            { PieceType.Adviser, new ActionWay(
                new ActionWayData(
                    new Direction[][] { SlopeDirection })) },
            { PieceType.Elephant, new ActionWay(
                new ActionWayData(
                    new Direction[][] {
                        new Direction[] { Direction.UpperLeft },
                        new Direction[] { Direction.UpperRight },
                        new Direction[] { Direction.LowerRight },
                        new Direction[] { Direction.LowerLeft } },
                    SlopeDirection)) },
            { PieceType.Horse, new ActionWay(
                new ActionWayData(
                    new Direction[][] {
                        new Direction[] { Direction.LowerLeft, Direction.UpperLeft },
                        new Direction[] { Direction.UpperLeft, Direction.UpperRight },
                        new Direction[] { Direction.UpperRight, Direction.LowerRight },
                        new Direction[] { Direction.LowerRight, Direction.LowerLeft } },
                    NormalDirection)) },
            { PieceType.Chariot, new ActionWay(
                new ActionWayData(
                    new Direction[][] { NormalDirection },
                    null,
                    false )) },
            { PieceType.Cannon, new ActionWay(
                new ActionWayData(
                    new Direction[][] { NormalDirection },
                    null,
                    false,
                    true )) },
            { PieceType.Pawn, new ActionWay(
                new ActionWayData(
                    new Direction[][] {NormalDirection} )) }
        };

        public static ActionWay GetActionWay(PieceType type)
        {
            if (ActionWayRules.ContainsKey(type))
                return ActionWayRules[type];
            return DefaultActionWay;
        }
    }

    public class DivisionRule
    {
        private static readonly Dictionary<PieceType, PieceType[]> DivisionRules
            = new Dictionary<PieceType, PieceType[]>()
        {
            { PieceType.Horse_Horse, new PieceType[] { PieceType.Horse, PieceType.Horse, } },
            { PieceType.Horse_Chariot, new PieceType[] { PieceType.Horse, PieceType.Chariot, } },
            { PieceType.Horse_Cannon, new PieceType[] { PieceType.Horse, PieceType.Cannon, } },
            { PieceType.Horse_Pawn, new PieceType[] { PieceType.Horse, PieceType.Pawn, } },
            { PieceType.Chariot_Chariot, new PieceType[] { PieceType.Chariot, PieceType.Chariot, } },
            { PieceType.Chariot_Pawn, new PieceType[] { PieceType.Chariot, PieceType.Pawn, } },
            { PieceType.Cannon_Chariot, new PieceType[] { PieceType.Cannon, PieceType.Chariot, } },
            { PieceType.Cannon_Cannon, new PieceType[] { PieceType.Cannon, PieceType.Cannon, } },
            { PieceType.Cannon_Pawn, new PieceType[] { PieceType.Cannon, PieceType.Pawn, } },
            { PieceType.Pawn_Pawn, new PieceType[] { PieceType.Pawn, PieceType.Pawn, } },
        };

        public static bool CanDivide(PieceType type)
        {
            return GameParams.fusionMode && DivisionRules.ContainsKey(type);
        }
        public static PieceType[] GetBaseTypes(PieceType type)
        {
            if (CanDivide(type))
                return DivisionRules[type];
            return null;
        }
    }

    public class FusionRule
    {
        private static readonly Dictionary<PieceType, Dictionary<PieceType, PieceType>> FusionRules
            = new Dictionary<PieceType, Dictionary<PieceType, PieceType>>()
        {
            { PieceType.Horse, new Dictionary<PieceType, PieceType>() {
                    { PieceType.Horse, PieceType.Horse_Horse },
                    { PieceType.Chariot, PieceType.Horse_Chariot },
                    { PieceType.Cannon, PieceType.Horse_Cannon },
                    { PieceType.Pawn, PieceType.Horse_Pawn }, } },
            { PieceType.Chariot, new Dictionary<PieceType, PieceType>() {
                    { PieceType.Horse, PieceType.Horse_Chariot },
                    { PieceType.Chariot, PieceType.Chariot_Chariot },
                    { PieceType.Cannon, PieceType.Cannon_Chariot },
                    { PieceType.Pawn, PieceType.Chariot_Pawn }, } },
            { PieceType.Cannon, new Dictionary<PieceType, PieceType>() {
                    { PieceType.Horse, PieceType.Horse_Cannon },
                    { PieceType.Chariot, PieceType.Cannon_Chariot },
                    { PieceType.Cannon, PieceType.Cannon_Cannon },
                    { PieceType.Pawn, PieceType.Cannon_Pawn }, } },
            { PieceType.Pawn, new Dictionary<PieceType, PieceType>() {
                    { PieceType.Horse, PieceType.Horse_Pawn },
                    { PieceType.Chariot, PieceType.Chariot_Pawn },
                    { PieceType.Cannon, PieceType.Cannon_Pawn },
                    { PieceType.Pawn, PieceType.Pawn_Pawn }, } },
        };

        public static bool CanFuse(PieceType type1, PieceType type2)
        {
            return GameParams.fusionMode && FusionRules.ContainsKey(type1) && FusionRules[type1].ContainsKey(type2);
        }
        public static PieceType GetFusionType(PieceType type1, PieceType type2)
        {
            if (CanFuse(type1, type2))
                return FusionRules[type1][type2];
            return null;
        }
    }

    public class ValueRule
    {
        private static readonly Dictionary<PieceType, int[,]> ValueRules
            = new Dictionary<PieceType, int[,]>()
        {
            { PieceType.King, new int[,] { 
                {0, 0, 0, 8888, 8888, 8888, 0, 0, 0},
                {0, 0, 0, 8888, 8888, 8888, 0, 0, 0},
                {0, 0, 0, 8888, 8888, 8888, 0, 0, 0},
                {0, 0, 0,    0,    0,    0, 0, 0, 0},
                {0, 0, 0,    0,    0,    0, 0, 0, 0},

                {0, 0, 0,    0,    0,    0, 0, 0, 0},
                {0, 0, 0,    0,    0,    0, 0, 0, 0},
                {0, 0, 0, 8888, 8888, 8888, 0, 0, 0},
                {0, 0, 0, 8888, 8888, 8888, 0, 0, 0},
                {0, 0, 0, 8888, 8888, 8888, 0, 0, 0} } },
            { PieceType.Adviser, new int[,]{
                {0, 0, 0,20, 0,20, 0, 0, 0},
                {0, 0, 0, 0,23, 0, 0, 0, 0},
                {0, 0, 0,20, 0,20, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0},

                {0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0,20, 0,20, 0, 0, 0},
                {0, 0, 0, 0,23, 0, 0, 0, 0},
                {0, 0, 0,20, 0,20, 0, 0, 0}, } },
            { PieceType.Elephant, new int[,]{
                {0, 0,20, 0, 0, 0,20, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0,23, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0,20, 0, 0, 0,20, 0, 0},

                {0, 0,20, 0, 0, 0,20, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0},
                {18,0, 0, 0,23, 0, 0, 0,18},
                {0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0,20, 0, 0, 0,20, 0, 0}, } },
            { PieceType.Horse, new int[,] {
                {88, 85, 90, 88, 90, 88, 90, 85, 88},
                {85, 90, 92, 93, 78, 93, 92, 90, 85},
                {93, 92, 94, 95, 92, 95, 94, 92, 93},
                {92, 94, 98, 95, 98, 95, 98, 94, 92},
                {90, 98,101,102,103,102,101, 98, 90},

                {90,100, 99,103,104,103, 99,100, 90},
                {93,108,100,107,100,107,100,108, 93},
                {92, 98, 99,103, 99,103, 99, 98, 92},
                {90, 96,103, 97, 94, 97,103, 96, 90},
                {90, 90, 90, 96, 90, 96, 90, 90, 90}, } },
            { PieceType.Cannon, new int[,]{
                { 96,  96,  97, 99,  99, 99,  97,  96,  96},
                { 96,  97,  98, 98,  98, 98,  98,  97,  96},
                { 97,  96, 100, 99, 101, 99, 100,  96,  97},
                { 96,  96,  96, 96,  96, 96,  96,  96,  96},
                { 95,  96,  99, 96, 100, 96,  99,  96,  95},

                { 96,  96,  96, 96, 100, 96,  96,  96,  96},
                { 96,  99,  99, 98, 100, 98,  99,  99,  96},
                { 97,  97,  96, 91,  92, 91,  96,  97,  97},
                { 98,  98,  96, 92,  89, 92,  96,  98,  98},
                {100, 100,  96, 91,  90, 91,  96, 100, 100}, } },
            { PieceType.Chariot, new int[,] {
                {194, 206, 204, 212, 200, 212, 204, 206, 194},
                {200, 208, 206, 212, 200, 212, 206, 208, 200},
                {198, 208, 204, 212, 212, 212, 204, 208, 198},
                {204, 209, 204, 212, 214, 212, 204, 209, 204},
                {208, 212, 212, 214, 215, 214, 212, 212, 208},
                
                {208, 211, 211, 214, 215, 214, 211, 211, 208},
                {206, 213, 213, 216, 216, 216, 213, 213, 206},
                {206, 208, 207, 214, 216, 214, 207, 208, 206},
                {206, 212, 209, 216, 233, 216, 209, 212, 206},
                {206, 208, 207, 213, 214, 213, 207, 208, 206}, } },
            { PieceType.Pawn, new int[,] {
                { 0,  0,  0,  0,  0,  0,  0,  0,  0},
                { 0,  0,  0,  0,  0,  0,  0,  0,  0},
                { 0,  0,  0,  0,  0,  0,  0,  0,  0},
                { 7,  0,  7,  0, 15,  0,  7,  0,  7},
                { 7,  0, 13,  0, 16,  0, 13,  0,  7},
                
                {14, 18, 20, 27, 29, 27, 20, 18, 14},
                {19, 23, 27, 29, 30, 29, 27, 23, 19},
                {19, 24, 32, 37, 37, 37, 32, 24, 19},
                {19, 24, 34, 42, 44, 42, 34, 24, 19},
                { 9,  9,  9, 11, 13, 11,  9,  9,  9}, } }
        };


        public static int GetValueOfRecord(ChessRecord record)
        {
            var selectPiece = record.SelectPiece;
            var selectPieceInfo = selectPiece.PieceInfo;
            var selectPieceColor = selectPiece.PieceColor;

            var targetPiece = record.TargetPiece;
            var targetPiecePoint = targetPiece.PiecePoint;
            var targetPieceType = targetPiece.PieceType;
            var targetPieceColor = targetPiece.PieceColor;

            var newSelectPiece = new ChessPiece(targetPiecePoint, selectPieceInfo);

            var locValue = GetValueOfPiece(newSelectPiece) - GetValueOfPiece(selectPiece);
            var eatValue = targetPieceType != PieceType.Blank && selectPieceColor != targetPieceColor ? GetValueOfPiece(targetPiece) : 0;
            return locValue + eatValue;
        }

        private static int GetValueOfType(PieceType type, PiecePoint point)
        {
            if (ValueRules.ContainsKey(type))
                return ValueRules[type][point.Y, point.X];
            var baseTypes = type.BaseTypes;
            return GetValueOfType(baseTypes[0], point) + GetValueOfType(baseTypes[1], point);
        }
        private static int GetValueOfPiece(ChessPiece piece)
        {
            var type = piece.PieceType;
            var point = piece.PieceColor == PieceColor.Red ? piece.PiecePoint : piece.PiecePoint.SymmetryPoint;
            return GetValueOfType(type, point);
        }
    }
}

