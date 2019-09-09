using FusionChess;
using System.Collections.Generic;
using UnityEngine;

public class Recorder : MonoBehaviour
{
    public Transform recordBox;
    public GameObject recordPrefab;

    private Stack<ChessRecord> chessRecords = new Stack<ChessRecord>();

    public int RecordsCount
    {
        get
        {
            return chessRecords.Count;
        }
    }

    public void ResetRecorder()
    {
        chessRecords.Clear();
        UpdateSelects();
    }
    public void PushRecord(ChessRecord record)
    {
        chessRecords.Push(record);
        UpdateSelects();
    }
    public ChessRecord PopRecord()
    {
        var record = chessRecords.Pop();
        UpdateSelects();
        return record;
    }

    private void DestroyRecords()
    {
        foreach (Transform select in recordBox.transform)
            Destroy(select.gameObject);
    }
    private void UpdateSelects()
    {
        DestroyRecords();
        if(chessRecords.Count > 0)
        {
            var record = chessRecords.Peek();
            CreateRecord(record.SelectPiece.PiecePoint);
            CreateRecord(record.TargetPiece.PiecePoint);
        }
    }
    private void CreateRecord(PiecePoint point)
    {
        var recordObj = Instantiate(recordPrefab, recordBox.transform);
        var record = recordObj.transform.GetComponent<Mark>();
        record.InitPiecePoint(point);
    }
}
