using FusionChess;
using System.Collections.Generic;
using UnityEngine;

public class SelectBoard : MonoBehaviour
{
    public Transform selectBox;
    public GameObject selectPrefab;

    private bool selectFlag
    {
        get
        {
            return GameParams.selectFlag;
        }
    }

    public void Select(PiecePoint piecePoint, HashSet<PiecePoint> selectPoints)
    {
        UpdateSelects(piecePoint, selectPoints);
    }
    public void UnSelect()
    {
        UpdateSelects();
    }

    private void DestroySelects()
    {
        foreach (Transform select in selectBox.transform)
            Destroy(select.gameObject);
    }
    private void CreateSelect(PiecePoint point)
    {
        var selectObj = Instantiate(selectPrefab, selectBox.transform);
        var select = selectObj.transform.GetComponent<Mark>();
        select.InitPiecePoint(point);
    }
    private void UpdateSelects(PiecePoint piecePoint = null, HashSet<PiecePoint> selectPoints = null)
    {
        DestroySelects();
        if (piecePoint != null)
            CreateSelect(piecePoint);
        if (selectFlag && selectPoints != null)
        {
            foreach (var point in selectPoints)
                CreateSelect(point);
        }
    }
}
