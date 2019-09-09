using FusionChess;
using UnityEngine;

public class Mark : MonoBehaviour
{
    private PiecePoint piecePoint;

    private PiecePoint PiecePoint
    {
        set
        {
            piecePoint = value;
            UpdatePosition();
        }
    }
    private PieceColor MyColor
    {
        get
        {
            return GameController.MyColor;
        }
    }

    public void InitPiecePoint(PiecePoint point)
    {
        PiecePoint = point;
    }
    private void UpdatePosition()
    {
        var point = MyColor == PieceColor.Red ? piecePoint : piecePoint.SymmetryPoint;
        var rectTransform = GetComponent<RectTransform>();
        var x = point.X * TouchInput.UnitSize;
        var y = point.Y * TouchInput.UnitSize;
        rectTransform.anchoredPosition = new Vector2(x, y);
    }
}
