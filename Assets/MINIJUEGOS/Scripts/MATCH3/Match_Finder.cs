using System.Collections.Generic;
using UnityEngine;

public static class MatchFinder
{
    public static List<Tile> FindMatches(Tile[,] grid, int width, int height)
    {
        List<Tile> matches = new List<Tile>();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width - 2; x++)
            {
                Tile a = grid[x, y];
                Tile b = grid[x + 1, y];
                Tile c = grid[x + 2, y];

                if (a != null && b != null && c != null)
                {
                    if (a.iconID == b.iconID && b.iconID == c.iconID)
                    {
                        if (!matches.Contains(a)) matches.Add(a);
                        if (!matches.Contains(b)) matches.Add(b);
                        if (!matches.Contains(c)) matches.Add(c);
                    }
                }
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height - 2; y++)
            {
                Tile a = grid[x, y];
                Tile b = grid[x, y + 1];
                Tile c = grid[x, y + 2];

                if (a != null && b != null && c != null)
                {
                    if (a.iconID == b.iconID && b.iconID == c.iconID)
                    {
                        if (!matches.Contains(a)) matches.Add(a);
                        if (!matches.Contains(b)) matches.Add(b);
                        if (!matches.Contains(c)) matches.Add(c);
                    }
                }
            }
        }

        return matches;
    }
}
