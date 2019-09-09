using FusionChess;
using UnityEngine;
using UnityEngine.UI;

public class Selecter : MonoBehaviour
{
    public RectTransform pointer;
    public RawImage[] images;

    private Piece selectPiece;
    private ChessPiece[] selectPieces;
    private int pointerIndex;

    private PieceColor CurrentColor
    {
        get
        {
            return GameController.CurrentColor;
        }
    }
    private Piece SelectPiece
    {
        set
        {
            selectPiece = value;
            SelectPieces = selectPiece == null ? new ChessPiece[3] : selectPiece.AllBasePieces;
        }
    }
    private ChessPiece[] SelectPieces
    {
        set
        {
            selectPieces = value;
            for(var i=0; i<3; i++)
            {
                var piece = selectPieces[i];
                var info = piece == null ? (i != 1? null : new PieceInfo(PieceType.Blank, CurrentColor)) : piece.PieceInfo;
                UpdateImage(i, info);
            }
            PointerIndex = selectPieces[0] == null ? 1 : 0;
        }
    }

    public int PointerIndex
    {
        set
        {
            pointerIndex = value;
            UpdatePointer();
            OnSelect(GetSelectPiece());
        }
    }

    public void Select(Piece piece)
    {
        SelectPiece = piece;
        SelectPieces = piece.AllBasePieces;
        pointer.gameObject.SetActive(true);
    }
    public void UnSelect()
    {
        SelectPiece = null;
        pointer.gameObject.SetActive(false);
        OnSelect(null);
    }

    private void UpdatePointer()
    {
        pointer.anchoredPosition = new Vector2(pointerIndex * TouchInput.UnitSize, 0);
    }
    private void UpdateImage(int index, PieceInfo info)
    {
        var isNotNull = info != null;
        if (isNotNull)
            images[index].texture = PieceTextures.Instance.GetPieceTexture(info);
        images[index].gameObject.SetActive(isNotNull);
    }
    private ChessPiece GetSelectPiece()
    {
        return selectPieces[pointerIndex];
    }
    private void OnSelect(ChessPiece piece)
    {
            GameController.Instance.OnSelect(piece);
    }
}
