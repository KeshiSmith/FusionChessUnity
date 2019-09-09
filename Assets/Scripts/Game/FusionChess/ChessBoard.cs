using System.Collections.Generic;
using UnityEngine;

namespace FusionChess
{
    public class ChessBoard
    {
        private static readonly PieceType[,] defaultBoard = new PieceType[,]
        {
            { PieceType.Blank,      PieceType.Blank,    PieceType.Blank,    PieceType.Blank,    PieceType.Blank },
            { PieceType.Chariot,    PieceType.Horse,    PieceType.Elephant, PieceType.Adviser,  PieceType.King  },
            { PieceType.Blank,      PieceType.Cannon,   PieceType.Blank,    PieceType.Blank,    PieceType.Blank },
            { PieceType.Pawn,       PieceType.Blank,    PieceType.Pawn,     PieceType.Blank,    PieceType.Pawn  },
        };

        public PieceInfo[,] BoardPieces { get; private set; }
        public ChessBoard()
        {
            BoardPieces = new PieceInfo[GameParams.BoardWidth, GameParams.BoardHeight];
        }
        public ChessBoard(PieceInfo[,] boardPieces)
        {
            BoardPieces = boardPieces;
        }

        public void ResetBoard()
        {
            for(int i = 0; i < GameParams.BoardHeight; i++)
            {
                var indexI = GetDefaultBoardIndex(i);
                var pieceColor = i < 5 ? PieceColor.Red : PieceColor.Black;
                for (int j = 0; j < GameParams.BoardWidth; j++)
                {
                    var indexJ = 4 - Mathf.Abs(j - 4);
                    var pieceType = defaultBoard[indexI, indexJ];
                    BoardPieces[j, i] = new PieceInfo(pieceType, pieceColor);
                }
            }
        }
        private int GetDefaultBoardIndex(int index)
        {
            switch (index)
            {
                case 0:
                case 9:
                    return 1;
                case 2:
                case 7:
                    return 2;
                case 3:
                case 6:
                    return 3;
                default:
                    return 0;
            }
        }

        public PieceInfo GetPieceInfo(PiecePoint point)
        {
            return BoardPieces[point.X, point.Y];
        }
        public void SetPieceInfo(PiecePoint point, PieceInfo info)
        {
            BoardPieces[point.X, point.Y] = info;
        }

        public void MovePiece(ChessRecord record)
        {
            MovePiece(record.SelectPiece, record.TargetPiece);
        }
        public void MovePiece(ChessPiece selectPiece, ChessPiece targetPiece)
        {
            var sourcePoint = selectPiece.PiecePoint;
            var sourcePieceInfo = GetPieceInfo(sourcePoint);
            var sourcePiece = new ChessPiece(sourcePoint, sourcePieceInfo);

            var newSourcePieceInfo = sourcePiece.Divide(selectPiece.PieceInfo);
            SetPieceInfo(sourcePoint, newSourcePieceInfo);
            var newTargetPieceInfo = selectPiece.EatOrFusePiece(targetPiece.PieceInfo);
            SetPieceInfo(targetPiece.PiecePoint, newTargetPieceInfo);
        }

        public void RegretPiece(ChessRecord record)
        {
            RegretPiece(record.SelectPiece, record.TargetPiece);
        }
        public void RegretPiece(ChessPiece selectPiece, ChessPiece targetPiece)
        {
            var sourcePoint = selectPiece.PiecePoint;
            var sourcePieceInfo = GetPieceInfo(sourcePoint);
            var sourcePiece = new ChessPiece(sourcePoint, sourcePieceInfo);

            var oldSourcePieceInfo = selectPiece.EatOrFusePiece(sourcePiece.PieceInfo);
            SetPieceInfo(sourcePoint, oldSourcePieceInfo);
            var oldTargetPieceInfo = targetPiece.PieceInfo;
            SetPieceInfo(targetPiece.PiecePoint, oldTargetPieceInfo);
        }

        public HashSet<PiecePoint> GetActionPoints(ChessPiece piece)
        {
            return piece.GetActionPoints(this);
        }
        public ChessRecord GetChessRecord(ChessPiece selectPiece, PiecePoint targetPoint)
        {
            var targetPieceInfo = GetPieceInfo(targetPoint);
            var targetPiece = new ChessPiece(targetPoint, targetPieceInfo);
            var chessRecord = new ChessRecord(selectPiece, targetPiece);
            return chessRecord;
        }
        public bool MovePieceIsOK(ChessRecord record, PieceColor currentColor)
        {
            MovePiece(record);
            bool isChecked = IsChecked(currentColor);
            RegretPiece(record);
            return !isChecked;
        }

        public bool IsWin(PieceColor color)
        {
            return IsWinOrChecked(true, color);
        }
        public bool IsChecked(PieceColor color)
        {
            return IsWinOrChecked(false, color);
        }

        public IEnumerable<ChessRecord> GetChessRecords(PieceColor color)
        {
            for (var i = 0; i < GameParams.BoardWidth; i++)
            {
                for (var j = 0; j < GameParams.BoardHeight; j++)
                {
                    var point = new PiecePoint(i, j);
                    var pieceInfo = GetPieceInfo(point);
                    if (pieceInfo.IsBlank() || pieceInfo.PieceColor != color)
                        continue;
                    var piece = new ChessPiece(point, pieceInfo);
                    var allBasePieces = piece.AllBasePieces;
                    foreach (var selectPiece in allBasePieces)
                    {
                        if (selectPiece == null)
                            continue;
                        var actionPoints = GetActionPoints(selectPiece);
                        foreach (var targetPoint in actionPoints)
                        {
                            var record = GetChessRecord(selectPiece, targetPoint);
                            yield return record;
                        }
                    }
                }
            }
        }
        private bool IsWinOrChecked(bool winModel, PieceColor color)
        {
            foreach(var record in GetChessRecords(!color))
            {
                if (winModel)
                {
                    if (MovePieceIsOK(record, !color))
                        return false;
                }
                else
                {
                    var targetPieceInfo = record.TargetPiece.PieceInfo;
                    if (targetPieceInfo.PieceType == PieceType.King && targetPieceInfo.PieceColor == color)
                        return true;
                }
            }
            return winModel;
        }
    }
}
