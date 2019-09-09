using AI;
using Android.Bluetooth;
using FusionChess;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    public TouchInput touchInput;
    public BoardMask boardMask;
    public PieceBoard pieceBoard;
    public SelectBoard selectBoard;
    public Recorder recorder;
    public Selecter selecter;

    private int regretCounts = 0;
    private ChessPiece selectPiece = null;
    private ChessRecord autoMoveRecord = null;
    private object autoMoveLock = new object();

    public static PieceColor MyColor { get; private set; }
    public static PieceColor CurrentColor { get; private set; }

    private bool FusionModel
    {
        get
        {
            return GameParams.fusionMode;
        }
        set
        {
           GameParams.fusionMode = value;
        }
    }
    private bool HiddenModel
    {
        get
        {
            return GameParams.hiddenMode;
        }
        set
        {
            GameParams.hiddenMode = value;
            boardMask.SetMaskEnabled(value);
            UpdateBoardMask();
        }
    }

    public bool PVPModel { get; set; }
    public bool IsHost { get; set; }
    public bool IsPlaying { get; private set; }
    public bool CanRegret
    {
        get
        {
            return recorder.RecordsCount > 0;
        }
    }

    private HashSet<PiecePoint> SelectPoints { get; set; }
    private ChessPiece SelectPiece
    {
        set
        {
            selectPiece = value;
            UpdateSelects();
        }
    }

    void Awake()
    {
        Instance = this;
    }
    void Update()
    {
        UpdateAutoMoveRecord();
    }

    public void OnClick(PiecePoint point)
    {
        if (point != null && CurrentColor == MyColor)
        {
            point = MyColor == PieceColor.Red ? point : point.SymmetryPoint;
            if (selectPiece != null && SelectPoints.Contains(point))
            {
                var record = pieceBoard.GetChessRecord(selectPiece, point);
                MovePieceTest(record);
            }
            else
            {
                var piece = pieceBoard.GetPieceOnBoard(point);
                if (piece != null && piece.PieceInfo.PieceColor == CurrentColor)
                {
                    selecter.Select(piece);
                    PlaySound("capture");
                    return;
                }
                else if (selectPiece != null)
                    PlaySound("uncapture");
            }
        }
        else if (selectPiece != null)
            PlaySound("uncapture");
        selecter.UnSelect();
    }
    public void OnSelect(ChessPiece piece)
    {
        SelectPiece = piece;
    }
    public void OnStartAnimation()
    {
        touchInput.gameObject.SetActive(false);
        recorder.gameObject.SetActive(false);
    }
    public void OnStopAnimation()
    {
        touchInput.gameObject.SetActive(true);
        recorder.gameObject.SetActive(true);
        SwitchCurrentColor();
        UpdateBoardMask();

        if(regretCounts > 0)
            RegretPiece();
        else
        {
            CheckedOrWin();
            AutoMovePieceTest();
        }
    }

    public void NewGame(PieceColor myColor, bool fusionModel, bool hiddenModel)
    {
        IsPlaying = true;
        MyColor = myColor;
        CurrentColor = PieceColor.Red;
        FusionModel = fusionModel;

        touchInput.gameObject.SetActive(true);
        pieceBoard.ResetPieceBoard();
        recorder.ResetRecorder();
        selectBoard.UnSelect();
        selecter.UnSelect();

        HiddenModel = hiddenModel;
        PlaySound("newgame");

        AutoMovePieceTest();
    }
    public void Regret(PieceColor color)
    {
        if(CanRegret)
        {
            regretCounts = CurrentColor == color && recorder.RecordsCount > 1 ? 2 : 1;
            RegretPiece();
        }
        selecter.UnSelect();
    }
    public void Resign(PieceColor color)
    {
        var message = color == PieceColor.Red ? "resign_red" : "resign_black";
        new Toast(message).Show();
        PlaySound(color != MyColor ? "win" : "loss");
        GameOver();
    }
    public void Draw()
    {
        new Toast("win_draw").Show();
        PlaySound("win");
        GameOver();
    }

    public void MovePiece(string[] messages)
    {
        var record = DecodeRecord(messages);
        if (record != null)
            MovePiece(record);
    }
    public void UpdateSelects()
    {
        if (selectPiece == null)
        {
            SelectPoints = null;
            selectBoard.UnSelect();
        }
        else
        {
            SelectPoints = pieceBoard.GetActionPoints(selectPiece);
            selectBoard.Select(selectPiece.PiecePoint, SelectPoints);
        }
    }
    public void ColorHasWon(PieceColor color)
    {
        var message = color == PieceColor.Red ? "win_red" : "win_black";
        new Toast(message).Show();
        PlaySound(color == MyColor ? "win" : "loss");
        GameOver();
    }
    public void GameOver()
    {
        selecter.UnSelect();
        touchInput.gameObject.SetActive(false);
        IsPlaying = false;
    }

    private void PlaySound(string name)
    {
        Audio.Instance.PlaySound(name);
    }
    private void UpdateBoardMask()
    {
        if (HiddenModel)
        {
            var visiblePoints = pieceBoard.GetVisiblePoints(MyColor);
            var relativeVisiblePoints = new HashSet<PiecePoint>();
            foreach(var point in visiblePoints)
            {
                var relativePoint = MyColor == PieceColor.Red ? point : point.SymmetryPoint;
                relativeVisiblePoints.Add(relativePoint);
            }
            boardMask.VisiblePoints = relativeVisiblePoints;
        }
    }
    private void MovePieceTest(ChessRecord record)
    {
        if (pieceBoard.MovePieceIsOK(record))
        {
            if (PVPModel)
            {
                var message = EncodeRecord(record);
                Bluetooth.Instance.Send(message);
            }
            MovePiece(record);
        }
        else
        {
            new Toast("move_not").Show();
            PlaySound("illegal");
        }
    }
    private void MovePiece(ChessRecord record)
    {
        pieceBoard.MovePiece(record);
        recorder.PushRecord(record);
        PlaySound("move");
    }
    private void AutoMovePieceTest()
    {
        if (IsPlaying && !PVPModel && CurrentColor != MyColor)
        {
            Thread thread = new Thread(new ThreadStart(AutoMovePiece));
            thread.Start();
        }
    }
    private void AutoMovePiece()
    {
        var ai = new AiTree(pieceBoard.Board, CurrentColor);
        var record = ai.GetBestRecord(2);
        lock (autoMoveLock)
        {
            autoMoveRecord = record;
        }
    }
    private void RegretPiece()
    {
        regretCounts--;
        var record = recorder.PopRecord();
        pieceBoard.RegretPiece(record);
        PlaySound("regret");
    }
    private void SwitchCurrentColor()
    {
        CurrentColor = !CurrentColor;
        selecter.UnSelect();
    }
    private void CheckedOrWin()
    {
        var notColor = !CurrentColor;
        if (pieceBoard.ColorIsWin(notColor))
                ColorHasWon(notColor);
        else if (pieceBoard.ColorIsChecked(CurrentColor))
        {
            new Toast("checked").Show();
            PlaySound("check");
        }
    }

    private string EncodeRecord(ChessRecord record)
    {
        // TODO temp code.
        var sourcePoint = record.SelectPiece.PiecePoint;
        var targetPoint = record.TargetPiece.PiecePoint;
        var sourcePiece = pieceBoard.GetPieceOnBoard(sourcePoint);
        var selectPiece = record.SelectPiece;
        var index = GetBasePiecesIndex(sourcePiece, selectPiece);
        var message = string.Format("move {0} {1} {2} {3} {4}", sourcePoint.X, sourcePoint.Y, index, targetPoint.X, targetPoint.Y);
        return message;
    }
    private ChessRecord DecodeRecord(string[] messages)
    {
        if (messages[0] != "move")
            return null;
        var sourcePoint = new PiecePoint(int.Parse(messages[1]), int.Parse(messages[2]));
        var index = int.Parse(messages[3]);
        var selectPiece = pieceBoard.GetPieceOnBoard(sourcePoint).AllBasePieces[index];
        var targetPoint = new PiecePoint(int.Parse(messages[4]), int.Parse(messages[5]));
        var record = pieceBoard.GetChessRecord(selectPiece, targetPoint);
        return record;
    }
    private int GetBasePiecesIndex(Piece piece, ChessPiece selectPiece)
    {
        var defalutIndex = -1;
        var basePieces = piece.AllBasePieces;
        for(var i = 0; i< 3; i++)
        {
            var basePiece = basePieces[i];
            if(basePiece != null && basePiece.PieceType == selectPiece.PieceType)
                return i;
        }
        return defalutIndex;
    }

    private void UpdateAutoMoveRecord()
    {
        lock (autoMoveLock)
        {
            if(autoMoveRecord != null)
            {
                MovePiece(autoMoveRecord);
                autoMoveRecord = null;
            }
        }
    }
}
