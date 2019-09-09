using FusionChess;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Piece : MonoBehaviour
{
    private const int frameCount = 10;

    public RawImage image;
    public RectTransform rectTransform;

    // 动画效果所需的参数
    private static bool AnimeFlag
    {
        get
        {
            return GameParams.animeFlag;
        }
    }
    private static Piece OldTargetPiece { get; set; }

    private PieceColor MyColor
    {
        get
        {
            return GameController.MyColor;
        }
    }

    public ChessPiece ChessPiece { get; private set; }

    public PiecePoint PiecePoint
    {
        get
        {
            return ChessPiece.PiecePoint;
        }
        set
        {
            ChessPiece.PiecePoint = value;
            UpdatePosition();
        }
    }
    public PieceInfo PieceInfo
    {
        get
        {
            return ChessPiece.PieceInfo;
        }
        set
        {
            ChessPiece.PieceInfo = value;
            UpdateImage();
        }
    }
    public ChessPiece[] AllBasePieces
    {
        get
        {
            return ChessPiece.AllBasePieces;
        }
    }

    public void InitChessPiece(ChessPiece piece)
    {
        ChessPiece = piece;
        UpdateImage();
        UpdatePosition();
    }
    public void InitChessPiece(PiecePoint point, PieceInfo info)
    {
        var piece = new ChessPiece(point, info);
        InitChessPiece(piece);
    }
    public void UpdatePiece(ChessPiece newTargetPiece, Piece oldTargetPiece)
    {
        ChessPiece = newTargetPiece;
        OldTargetPiece = oldTargetPiece;
        StartCoroutine("MovePiece");
    }

    private IEnumerator MovePiece()
    {
        OnStartAnimation();
        var oldAnchoredPosition = rectTransform.anchoredPosition;
        var newAnchoredPosition = GetAnchoredPosition();
        // 棋子移动动画
        if (AnimeFlag)
        {
            for (int i = 1; i < frameCount; i++)
            {
                var anchoredPosition = Vector2.Lerp(oldAnchoredPosition, newAnchoredPosition, i / (float)frameCount);
                rectTransform.anchoredPosition = anchoredPosition;
                yield return new WaitForSecondsRealtime(0.01f);
            }
        }
        rectTransform.anchoredPosition = newAnchoredPosition;
        OnStopAnimation();
        yield return null;
    }
    private void OnStartAnimation()
    {
        GameController.Instance.OnStartAnimation();
    }
    private void OnStopAnimation()
    {
        if(OldTargetPiece != null)
            Destroy(OldTargetPiece.gameObject);
        UpdateImage();
        GameController.Instance.OnStopAnimation();
    }

    private void UpdateImage()
    {
        image.texture = PieceTextures.Instance.GetPieceTexture(PieceInfo);
    }
    private void UpdatePosition()
    {
        var anchoredPosition = GetAnchoredPosition();
        rectTransform.anchoredPosition = anchoredPosition;
    }
    private Vector2 GetAnchoredPosition()
    {
        var point = ChessPiece.PiecePoint;
        point = MyColor == PieceColor.Red ? point : point.SymmetryPoint;
        var x = point.X * TouchInput.UnitSize;
        var y = point.Y * TouchInput.UnitSize;
        return new Vector2(x, y);
    }
}
