using FusionChess;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchInput : MonoBehaviour
{
    public const int UnitSize = 70;

    public RectTransform canvas, touchArea;
    private float startX, startY;

    void Awake()
    {
        canvas = transform.Find("../../../").GetComponent<RectTransform>();
        touchArea = transform.Find("TouchArea").GetComponent<RectTransform>();
    }
    void Start()
    {
        startX = (canvas.rect.width - touchArea.rect.width) / 2 + touchArea.localPosition.x;
        startY = (canvas.rect.height - touchArea.rect.height) / 2 + touchArea.localPosition.y;
    }

    public void ClickInArea(BaseEventData data)
    {
        // 处理点击数据为棋盘坐标
        var pointerData = (PointerEventData)data;
        var screenPosition = pointerData.position;
        var guiPoint = ScreenToGuiPoint(screenPosition);
        var piecePoint = GuiToPiecePoint(guiPoint);
        // 点击响应函数
        OnClick(piecePoint);
    }
    public void ClickOutArea(BaseEventData data)
    {
        // 点击响应函数
        OnClick(null);
    }

    private Vector2 ScreenToGuiPoint(Vector2 position)
    {
        var x = position.x / Screen.width * canvas.sizeDelta.x;
        var y = position.y / Screen.height * canvas.sizeDelta.y;
        return new Vector2(x, y);
    }
    private PiecePoint GuiToPiecePoint(Vector2 position)
    {
        var x = (int)(position.x - startX) / UnitSize;
        var y = (int)(position.y - startY) / UnitSize;
        return new PiecePoint(x, y);
    }
    private void OnClick(PiecePoint point)
    {
        GameController.Instance.OnClick(point);
    }
}
