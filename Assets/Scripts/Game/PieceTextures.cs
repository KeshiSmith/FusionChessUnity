using FusionChess;
using UnityEngine;

public class PieceTextures : MonoBehaviour
{
    public static PieceTextures Instance { get; private set; }

    public Texture[] redPieceTextures;
    public Texture[] blackPieceTextures;

    void Awake()
    {
        Instance = this;
    }

    public Texture GetPieceTexture(PieceInfo info)
    {
        int index = info.PieceType;
        return info.PieceColor == PieceColor.Red ? redPieceTextures[index] : blackPieceTextures[index];
    }
}
