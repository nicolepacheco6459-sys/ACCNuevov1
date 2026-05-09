using UnityEngine;

[System.Serializable]
public class LevelData
{
    public int width;
    public int height;

    public int iconCount;

    public int targetScore;

    public Vector2Int[] blockedCells;
}