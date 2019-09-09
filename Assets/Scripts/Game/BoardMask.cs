using FusionChess;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class BoardMask : MonoBehaviour
{
    public RawImage boardBackground, boardMask;
    public Mask mask;

    private const int Width = 9;
    private const int Height = 10;

    private static readonly Color Gray = new Color(0f, 0f, 0f, 0.2f);
    private static readonly Color Trans = new Color(0f, 0f, 0f, 0f);

    private HashSet<PiecePoint> visiblePoints = new HashSet<PiecePoint>();

    public HashSet<PiecePoint> VisiblePoints
    {
        set
        {
            visiblePoints = value;
            UpdateVisibleArea();
        }
    }

    public void SetMaskEnabled(bool enabled)
    {
        gameObject.SetActive(enabled);
        boardMask.enabled = enabled;
        mask.enabled = enabled;
    }
    public void UpdateVisibleArea()
    {
        var backgroundTexture = new Texture2D(Width, Height);
        var maskTexture = new Texture2D(Width, Height);
        for (var i = 0; i < Width; i++)
            for (var j = 0; j < Height; j++)
            {
                var point = new PiecePoint(i, j);
                if (visiblePoints.Contains(point))
                {
                    backgroundTexture.SetPixel(i, j, Trans);
                    maskTexture.SetPixel(i, j, Gray);
                }
                else
                {
                    backgroundTexture.SetPixel(i, j, Gray);
                    maskTexture.SetPixel(i, j, Trans);
                }
            }
        backgroundTexture.filterMode = FilterMode.Bilinear;
        boardBackground.texture = backgroundTexture;
        backgroundTexture.Apply();
        maskTexture.filterMode = FilterMode.Point;
        boardMask.texture = maskTexture;
        maskTexture.Apply();
    }
}
