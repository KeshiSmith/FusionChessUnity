using FusionChess;
using System.Collections.Generic;
using UnityEngine;

public class PieceBoard : MonoBehaviour
{
    public Transform pieceBox;
    public GameObject piecePrefab;

    private ChessBoard board = new ChessBoard();
    private Dictionary<PiecePoint, Piece> pieces = new Dictionary<PiecePoint, Piece>();

    private PieceColor MyColor
    {
        get
        {
            return GameController.MyColor;
        }
    }
    private PieceColor CurrentColor
    {
        get
        {
            return GameController.CurrentColor;
        }
    }

    public ChessBoard Board
    {
        get
        {
            return board;
        }
    }

    public void ResetPieceBoard()
    {
        board.ResetBoard();
        UpdateBoard();
    }

    public Piece GetPieceOnBoard(PiecePoint point)
    {
        if (pieces.ContainsKey(point))
            return pieces[point];
        return null;
    }

    public HashSet<PiecePoint> GetVisiblePoints(PieceColor color)
    {
        var visiblePoints = new HashSet<PiecePoint>();
        foreach (var piecePair in pieces)
        {
            var point = piecePair.Key;
            var piece = piecePair.Value.ChessPiece;
            if (piece.PieceColor == color)
            {
                visiblePoints.Add(point);
                visiblePoints.UnionWith(GetAllActionPoints(piece));
            }
            else
            {
                var actionPoints = GetAllActionPoints(piece);
                foreach(var actionPoint in actionPoints)
                {
                    if (pieces.ContainsKey(actionPoint) && pieces[actionPoint].PieceInfo.PieceColor == color)
                        visiblePoints.Add(point);
                }
            }
        }
        return visiblePoints;
    }
    private HashSet<PiecePoint> GetAllActionPoints(ChessPiece piece)
    {
        return piece.GetAllActionPoints(board, true);
    }

    public HashSet<PiecePoint> GetActionPoints(ChessPiece piece)
    {
        return board.GetActionPoints(piece);
    }
    public ChessRecord GetChessRecord(ChessPiece selectPiece, PiecePoint targetPoint)
    {
        return board.GetChessRecord(selectPiece, targetPoint);
    }
    public bool ColorIsWin(PieceColor color)
    {
        return board.IsWin(color);
    }
    public bool ColorIsChecked(PieceColor color)
    {
        return board.IsChecked(color);
    }
    public bool MovePieceIsOK(ChessRecord record)
    {
        return board.MovePieceIsOK(record, CurrentColor);
    }
    public void MovePiece(ChessRecord record)
    {
        UpdateRecord(record);
    }
    public void RegretPiece(ChessRecord record)
    {
        UpdateRecord(record, true);
    }

    private void UpdateRecord(ChessRecord record, bool regret = false)
    {
        var actionRecord = regret ? record.ReverseRecord : record;
        var sourcePoint = actionRecord.SelectPiece.PiecePoint;
        var oldSourcePiece = GetPieceOnBoard(sourcePoint);
        var targetPoint = actionRecord.TargetPiece.PiecePoint;
        var oldTargetPiece = GetPieceOnBoard(targetPoint);

        if (regret)
            board.RegretPiece(record);
        else
            board.MovePiece(record);

        var newSourcePieceInfo = board.GetPieceInfo(sourcePoint);
        if (newSourcePieceInfo.IsBlank())
        {
            DestroyPiece(sourcePoint);
            pieces.Remove(sourcePoint);
        }
        else
            oldSourcePiece.PieceInfo = newSourcePieceInfo;

        var newSelectPiece = CreatePiece(sourcePoint, record.SelectPiece.PieceInfo);
        pieces[targetPoint] = newSelectPiece;

        var newTargetPieceInfo = board.GetPieceInfo(targetPoint);
        var newTargetPiece = new ChessPiece(targetPoint, newTargetPieceInfo);
        newSelectPiece.UpdatePiece(newTargetPiece, oldTargetPiece);
    }
    private void UpdateBoard()
    {
        DestroyPieces();
        for (var x = 0; x < GameParams.BoardWidth; x++)
            for (var y = 0; y < GameParams.BoardHeight; y++)
            {
                var point = new PiecePoint(x, y);
                var info = board.GetPieceInfo(point);
                if (!info.IsBlank())
                {
                    var piece = CreatePiece(point, info);
                    pieces[point] = piece;
                }
            }
    }
    private Piece CreatePiece(ChessPiece chessPiece)
    {
        var pieceObj = Instantiate(piecePrefab, pieceBox.transform);
        var piece = pieceObj.transform.GetComponent<Piece>();
        piece.InitChessPiece(chessPiece);
        return piece;
    }
    private Piece CreatePiece(PiecePoint point, PieceInfo info)
    {
        return CreatePiece(new ChessPiece(point, info));
    }
    private void DestroyPiece(PiecePoint point)
    {
        if (pieces.ContainsKey(point))
            Destroy(pieces[point].gameObject);
    }
    private void DestroyPieces()
    {
        foreach (Transform piece in pieceBox.transform)
            Destroy(piece.gameObject);
        pieces.Clear();
    }
}
