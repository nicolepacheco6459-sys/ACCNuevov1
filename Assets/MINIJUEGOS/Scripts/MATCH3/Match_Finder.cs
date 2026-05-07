using System.Collections.Generic;
using UnityEngine;

public static class MatchFinder
{
    public static List<Tile> FindMatches(
        Tile[,] grid,
        int width,
        int height)
    {
        List<Tile> matches = new List<Tile>();

        // =========================
        // HORIZONTAL
        // =========================

        for (int y = 0; y < height; y++)
        {
            int matchCount = 1;

            for (int x = 0; x < width - 1; x++)
            {
                Tile current = grid[x, y];
                Tile next = grid[x + 1, y];

                if (current != null &&
                    next != null &&
                    current.iconID == next.iconID)
                {
                    matchCount++;
                }
                else
                {
                    if (matchCount >= 3)
                    {
                        for (int i = 0; i < matchCount; i++)
                        {
                            Tile match =
                                grid[x - i, y];

                            if (!matches.Contains(match))
                                matches.Add(match);
                        }
                    }

                    matchCount = 1;
                }
            }

            // Final fila
            if (matchCount >= 3)
            {
                for (int i = 0; i < matchCount; i++)
                {
                    Tile match =
                        grid[(width - 1) - i, y];

                    if (!matches.Contains(match))
                        matches.Add(match);
                }
            }
        }

        // =========================
        // VERTICAL
        // =========================

        for (int x = 0; x < width; x++)
        {
            int matchCount = 1;

            for (int y = 0; y < height - 1; y++)
            {
                Tile current = grid[x, y];
                Tile next = grid[x, y + 1];

                if (current != null &&
                    next != null &&
                    current.iconID == next.iconID)
                {
                    matchCount++;
                }
                else
                {
                    if (matchCount >= 3)
                    {
                        for (int i = 0; i < matchCount; i++)
                        {
                            Tile match =
                                grid[x, y - i];

                            if (!matches.Contains(match))
                                matches.Add(match);
                        }
                    }

                    matchCount = 1;
                }
            }

            // Final columna
            if (matchCount >= 3)
            {
                for (int i = 0; i < matchCount; i++)
                {
                    Tile match =
                        grid[x, (height - 1) - i];

                    if (!matches.Contains(match))
                        matches.Add(match);
                }
            }
        }

        return matches;
    }
}
