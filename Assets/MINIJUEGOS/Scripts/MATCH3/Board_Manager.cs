using DG.Tweening;
using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    private Cell[,] cells; 

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
        // =========================
        // LIMPIAR TABLERO ANTERIOR
        // =========================

        foreach (Transform child in boardParent)
        {
            Destroy(child.gameObject);
        }

        // =========================
        // CREAR ARRAYS
        // =========================

        grid = new Tile[width, height];

        cells = new Cell[width, height];

        // =========================
        // GENERAR TABLERO
        // =========================

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Posición del tablero
                Vector3 position =
                    GetWorldPosition(x, y);

                // =========================
                // REVISAR BLOQUEADA
                // =========================

                bool isBlocked =
                    blockedPositions.Contains(
                        new Vector2Int(x, y)
                    );

                GameObject selectedCell;

                // =========================
                // ELEGIR CELDA
                // =========================

                if (isBlocked)
                {
                    selectedCell =
                        blockedCellPrefab;
                }
                else
                {
                    bool isDark =
                        (x + y) % 2 == 0;

                    selectedCell =
                        isDark
                        ? darkCellPrefab
                        : lightCellPrefab;
                }

                // =========================
                // CREAR CELDA
                // =========================

                GameObject cellObj =
                    Instantiate(
                        selectedCell,
                        position,
                        Quaternion.identity,
                        boardParent
                    );

                // =========================
                // GUARDAR CELL
                // =========================

                Cell cell =
                    cellObj.GetComponent<Cell>();

                cell.x = x;
                cell.y = y;

                cells[x, y] = cell;

                // =========================
                // SI BLOQUEADA
                // NO CREAR TILE
                // =========================

                if (isBlocked)
                    continue;

                // =========================
                // CREAR TILE
                // =========================

                SpawnTile(
                    x,
                    y,
                    iconCount,
                    position
                );
            }
        }

        // =========================
        // EVITAR MATCHES INICIALES
        // =========================

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

        tile.board = this;

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
        Debug.Log("CLICK TILE: " + tile.x + "," + tile.y);

        if (isBusy)
            return;

        // Primera selección
        if (selectedTile == null)
        {
            Debug.Log("PRIMERA SELECCION");

            selectedTile = tile;

            selectedTile.SetSelected(true);

            return;
        }

        // Misma pieza
        if (selectedTile == tile)
        {
            Debug.Log("DESELECCION");

            selectedTile.SetSelected(false);

            selectedTile = null;

            return;
        }

        // Son adyacentes
        if (AreAdjacent(selectedTile, tile))
        {
            Debug.Log("SWAP");

            selectedTile.SetSelected(false);

            StartCoroutine(SwapTiles(selectedTile, tile));
        }
        else
        {
            Debug.Log("CAMBIO SELECCION");

            selectedTile.SetSelected(false);

            selectedTile = tile;

            selectedTile.SetSelected(true);
        }
    }

    bool AreAdjacent(Tile a, Tile b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) == 1;
    }

    IEnumerator SwapTiles(Tile a, Tile b)
    {
        isBusy = true;

        // Guardar posiciones
        int ax = a.x;
        int ay = a.y;

        int bx = b.x;
        int by = b.y;

        Vector3 aWorldPos = a.transform.position;
        Vector3 bWorldPos = b.transform.position;

        // =========================
        // SWAP LOGICO PRIMERO
        // =========================

        grid[ax, ay] = b;
        grid[bx, by] = a;

        a.x = bx;
        a.y = by;

        b.x = ax;
        b.y = ay;

        // =========================
        // SWAP VISUAL
        // =========================

        a.transform.DOMove(bWorldPos, 0.2f);
        b.transform.DOMove(aWorldPos, 0.2f);

        yield return new WaitForSeconds(0.25f);

        // =========================
        // BUSCAR MATCHES
        // =========================

        List<Tile> matches =
            MatchFinder.FindMatches(
                grid,
                width,
                height
            );

        // =========================
        // SI HAY MATCH
        // =========================

        if (matches.Count > 0)
        {
            yield return StartCoroutine(
                ClearMatches(matches)
            );
        }
        else
        {
            // REGRESAR LOGICO
            grid[ax, ay] = a;
            grid[bx, by] = b;

            a.x = ax;
            a.y = ay;

            b.x = bx;
            b.y = by;

            // REGRESAR VISUAL
            a.transform.DOMove(aWorldPos, 0.2f);
            b.transform.DOMove(bWorldPos, 0.2f);

            yield return new WaitForSeconds(0.25f);
        }

        selectedTile = null;

        
    }

    IEnumerator ClearMatches(List<Tile> matches)
    {
        GameManager_MATCH3.Instance.AddScore(
            matches.Count * 50
        );

        // =========================
        // ELIMINAR MATCHES
        // =========================

        foreach (Tile tile in matches)
        {
            if (tile == null)
                continue;

            int x = tile.x;
            int y = tile.y;

            // Limpiar grid PRIMERO
            grid[x, y] = null;

            // Destruir visualmente
            Destroy(tile.gameObject);
        }

        // Esperar frame real
        yield return null;

        // Esperar poquito visualmente
        yield return new WaitForSeconds(0.2f);

        // =========================
        // COLAPSAR
        // =========================

        yield return StartCoroutine(
            CollapseBoard()
        );
    }

    IEnumerator CollapseBoard()
    {
        bool movedTile = true;

        // =========================
        // HACER CAER TODO
        // =========================

        while (movedTile)
        {
            movedTile = false;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height - 1; y++)
                {
                    // espacio vacío
                    if (grid[x, y] == null &&
                        !blockedPositions.Contains(
                            new Vector2Int(x, y)))
                    {
                        // buscar arriba
                        for (int ny = y + 1; ny < height; ny++)
                        {
                            if (grid[x, ny] != null)
                            {
                                Tile fallingTile =
                                    grid[x, ny];

                                // mover grid
                                grid[x, y] = fallingTile;
                                grid[x, ny] = null;

                                // actualizar coords
                                fallingTile.y = y;
                                fallingTile.x = x;

                                // posición visual
                                Vector3 position =
                                    GetWorldPosition(x, y);
                                

                                // mover visual
                                fallingTile.transform.DOMove(
                                    position,
                                    0.2f
                                );

                                movedTile = true;

                                break;
                            }
                        }
                    }
                }
            }

            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(0.3f);

        // =========================
        // RELLENAR
        // =========================

        RefillBoard();

        yield return new WaitForSeconds(0.4f);

        // =========================
        // BUSCAR NUEVOS MATCHES
        // =========================

        List<Tile> matches =
            MatchFinder.FindMatches(
                grid,
                width,
                height
            );

        if (matches.Count > 0)
        {
            yield return StartCoroutine(
                ClearMatches(matches)
            );
        }
        else
        {
            isBusy = false;
        }
    }

    void RefillBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] == null)
                {
                    Vector3 targetPos =
                        GetWorldPosition(x, y);

                    Vector3 spawnPos =
                        targetPos + Vector3.up * 5f;

                    GameObject tileObj =
                        Instantiate(
                            tilePrefab,
                            spawnPos,
                            Quaternion.identity,
                            boardParent
                        );

                    Tile tile =
                        tileObj.GetComponent<Tile>();

                    int randomID =
                        Random.Range(
                            0,
                            iconSprites.Length
                        );

                    tile.Setup(
                        x,
                        y,
                        randomID,
                        iconSprites[randomID],
                        this
                    );

                    tile.board = this;

                    grid[x, y] = tile;

                    tile.transform.DOMove(
                        targetPos,
                        0.2f
                    );
                }
            }
        }
    }
    Vector3 GetWorldPosition(int x, int y)
    {
        float offsetX =
            (width - 1) / 2f;

        float offsetY =
            (height - 1) / 2f;

        return new Vector3(
            (x - offsetX) * tileSpacing,
            (y - offsetY) * tileSpacing,
            0
        );
    }
}
