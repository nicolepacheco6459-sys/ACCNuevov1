using DG.Tweening;
using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [Header("Board")]
    public int width;
    public int height;

    public float tileSpacing = 1f;

    [Header("References")]
    public GameObject tilePrefab;
    public GameObject lightCellPrefab;
    public GameObject darkCellPrefab;
    public GameObject blockedCellPrefab;

    public Transform boardParent;

    [Header("Sprites")]
    public Sprite[] iconSprites;

    public Tile[,] grid;

    private Tile selectedTile;

    private HashSet<Vector2Int> blockedPositions = new HashSet<Vector2Int>();

    private bool isBusy;

    public void SetupBoard(LevelData levelData)
    {
        width = levelData.width;
        height = levelData.height;

        grid = new Tile[width, height];

        blockedPositions.Clear();

        foreach (Vector2Int pos in levelData.blockedCells)
        {
            blockedPositions.Add(pos);
        }

        GenerateBoard(levelData.iconCount);
    }

    void GenerateBoard(int iconCount)
    {
        float offsetX = (width - 1) / 2f;
        float offsetY = (height - 1) / 2f;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 position = new Vector3(
                    (x - offsetX) * tileSpacing,
                    (y - offsetY) * tileSpacing,
                    0
                );

                bool isBlocked = blockedPositions.Contains(new Vector2Int(x, y));

                GameObject selectedCell;

                if (isBlocked)
                {
                    selectedCell = blockedCellPrefab;
                }
                else
                {
                    bool isDark = (x + y) % 2 == 0;

                    selectedCell = isDark
                        ? darkCellPrefab
                        : lightCellPrefab;
                }

                Instantiate(
                    selectedCell,
                    position,
                    Quaternion.identity,
                    boardParent
                );

                if (isBlocked)
                    continue;

                SpawnTile(x, y, iconCount, position);
            }
        }

        RemoveStartingMatches(iconCount);
    }

    void SpawnTile(int x, int y, int iconCount, Vector3 position)
    {
        GameObject tileObj = Instantiate(
            tilePrefab,
            position,
            Quaternion.identity,
            boardParent
        );

        Tile tile = tileObj.GetComponent<Tile>();

        int randomID = Random.Range(0, iconCount);

        tile.Setup(
            x,
            y,
            randomID,
            iconSprites[randomID],
            this
        );

        grid[x, y] = tile;
    }

    void RemoveStartingMatches(int iconCount)
    {
        bool hasMatches = true;

        while (hasMatches)
        {
            hasMatches = false;

            List<Tile> matches = MatchFinder.FindMatches(grid, width, height);

            if (matches.Count > 0)
            {
                hasMatches = true;

                foreach (Tile tile in matches)
                {
                    int randomID = Random.Range(0, iconCount);

                    tile.iconID = randomID;

                    tile.GetComponent<SpriteRenderer>().sprite =
                        iconSprites[randomID];
                }
            }
        }
    }

    public void SelectTile(Tile tile)
    {
        if (isBusy)
            return;

        if (selectedTile == null)
        {
            selectedTile = tile;
            return;
        }

        if (selectedTile == tile)
        {
            selectedTile = null;
            return;
        }

        if (AreAdjacent(selectedTile, tile))
        {
            StartCoroutine(SwapTiles(selectedTile, tile));
        }
        else
        {
            selectedTile = tile;
        }
    }

    bool AreAdjacent(Tile a, Tile b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) == 1;
    }

    IEnumerator SwapTiles(Tile a, Tile b)
    {
        isBusy = true;

        Vector3 aPos = a.transform.position;
        Vector3 bPos = b.transform.position;

        a.transform.DOMove(bPos, 0.2f);
        b.transform.DOMove(aPos, 0.2f);

        yield return new WaitForSeconds(0.25f);

        int tempX = a.x;
        int tempY = a.y;

        grid[a.x, a.y] = b;
        grid[b.x, b.y] = a;

        a.x = b.x;
        a.y = b.y;

        b.x = tempX;
        b.y = tempY;

        List<Tile> matches = MatchFinder.FindMatches(grid, width, height);

        if (matches.Count > 0)
        {
            yield return StartCoroutine(ClearMatches(matches));
        }
        else
        {
            a.transform.DOMove(aPos, 0.2f);
            b.transform.DOMove(bPos, 0.2f);

            yield return new WaitForSeconds(0.25f);

            grid[a.x, a.y] = b;
            grid[b.x, b.y] = a;

            tempX = a.x;
            tempY = a.y;

            a.x = b.x;
            a.y = b.y;

            b.x = tempX;
            b.y = tempY;
        }

        selectedTile = null;

        isBusy = false;
    }

    IEnumerator ClearMatches(List<Tile> matches)
    {
        GameManager_MATCH3.Instance.AddScore(matches.Count * 50);

        foreach (Tile tile in matches)
        {
            grid[tile.x, tile.y] = null;

            Destroy(tile.gameObject);
        }

        yield return new WaitForSeconds(0.2f);

        yield return StartCoroutine(CollapseBoard());
    }

    IEnumerator CollapseBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] == null &&
                    !blockedPositions.Contains(new Vector2Int(x, y)))
                {
                    for (int ny = y + 1; ny < height; ny++)
                    {
                        if (grid[x, ny] != null)
                        {
                            Tile tileAbove = grid[x, ny];

                            grid[x, y] = tileAbove;
                            grid[x, ny] = null;

                            tileAbove.y = y;

                            tileAbove.transform.DOMove(
                                new Vector3(
                                    (tileAbove.x - (width - 1) / 2f) * tileSpacing,
                                    (y - (height - 1) / 2f) * tileSpacing,
                                    0
                                ),
                                0.2f
                            );

                            break;
                        }
                    }
                }
            }
        }

        yield return new WaitForSeconds(0.3f);

        RefillBoard();

        yield return new WaitForSeconds(0.3f);

        List<Tile> matches = MatchFinder.FindMatches(grid, width, height);

        if (matches.Count > 0)
        {
            yield return StartCoroutine(ClearMatches(matches));
        }
    }

    void RefillBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] == null &&
                    !blockedPositions.Contains(new Vector2Int(x, y)))
                {
                    Vector3 spawnPos = new Vector3(
                        (x - (width - 1) / 2f) * tileSpacing,
                        ((height + 1) - (height - 1) / 2f) * tileSpacing,
                        0
                    );

                    GameObject tileObj = Instantiate(
                        tilePrefab,
                        spawnPos,
                        Quaternion.identity,
                        boardParent
                    );

                    Tile tile = tileObj.GetComponent<Tile>();

                    int randomID = Random.Range(0, iconSprites.Length);

                    tile.Setup(
                        x,
                        y,
                        randomID,
                        iconSprites[randomID],
                        this
                    );

                    grid[x, y] = tile;

                    tile.transform.DOMove(
                        new Vector3(
                            (x - (width - 1) / 2f) * tileSpacing,
                            (y - (height - 1) / 2f) * tileSpacing,
                            0
                        ),
                        0.25f
                    );
                }
            }
        }
    }
}
