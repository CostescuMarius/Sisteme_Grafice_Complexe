using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Linq;

public class PathFinder
{
    private Tilemap tilemap;
    private Vector3 start;
    private Vector3 finish;

    Vector3Int minBounds;
    Vector3Int maxBounds;

    public PathFinder(Tilemap tilemap, Vector3 start, Vector3 finish)
    {
        this.tilemap = tilemap;
        this.start = start;
        this.finish = finish;

        minBounds = new Vector3Int(
            Mathf.Min(tilemap.WorldToCell(start).x, tilemap.WorldToCell(finish).x),
            Mathf.Min(tilemap.WorldToCell(start).y, tilemap.WorldToCell(finish).y),
            0
        );

        maxBounds = new Vector3Int(
            Mathf.Max(tilemap.WorldToCell(start).x, tilemap.WorldToCell(finish).x),
            Mathf.Max(tilemap.WorldToCell(start).y, tilemap.WorldToCell(finish).y),
            0
        );


        // for (int x = minBounds.x; x <= maxBounds.x; x++)
        // {
        //     for (int y = minBounds.y; y <= maxBounds.y; y++)
        //     {
        //         Vector3Int cellPosition = new Vector3Int(x, y, 0);
        //         TileBase tile = tilemap.GetTile(cellPosition);

        //         if (tile != null)
        //         {
        //             Debug.Log($"Tile at ({x}, {y}): {tile.name}");
        //         }
        //         else
        //         {
        //             Debug.Log($"Tile at ({x}, {y}): Empty");
        //         }
        //     }
        // }
    }

    public List<List<string>> FindAllPaths()
    {
        Queue<(Vector3 position, List<string> path, HashSet<Vector3Int> visited)> queue =
            new Queue<(Vector3, List<string>, HashSet<Vector3Int>)>();
        List<List<string>> allPaths = new List<List<string>>();
        HashSet<Vector3Int> initialVisited = new HashSet<Vector3Int>();
        Vector3Int startCell = tilemap.WorldToCell(start);
        initialVisited.Add(startCell);  // Marchez start-ul ca vizitat
        queue.Enqueue((start, new List<string>(), initialVisited));

        Vector3[] directions = { Vector3.up / 2, Vector3.down / 2, Vector3.left / 2, Vector3.right / 2 };
        string[] directionNames = { "up", "down", "left", "right" };

        int limit = 0;
        int queueCountMax = 0;

        while (queue.Count > 0 && limit < 1000000)
        {
            if (queue.Count > queueCountMax)
            {
                queueCountMax = queue.Count;
            }
            limit++;

            var (currentPos, path, visited) = queue.Dequeue();

            if (tilemap.WorldToCell(currentPos) == tilemap.WorldToCell(finish))
            {
                allPaths.Add(new List<string>(path));
                continue;
            }

            for (int i = 0; i < directions.Length; i++)
            {
                Vector3 nextPos = currentPos + directions[i];
                Vector3Int nextCell = tilemap.WorldToCell(nextPos);

                if (IsPositionInsideTilemap(nextPos) && !visited.Contains(nextCell))  // Verifică dacă nu a fost deja vizitată
                {
                    List<string> newPath = new List<string>(path) { directionNames[i] };

                    if (CountDirectionChanges(newPath) > 4)
                    {
                        continue;
                    }

                    HashSet<Vector3Int> newVisited = new HashSet<Vector3Int>(visited);
                    newVisited.Add(nextCell);  // Marchez noua poziție ca vizitată
                    queue.Enqueue((nextPos, newPath, newVisited));
                }
            }
        }

        return allPaths;
    }



    public List<List<string>> SortPathsByDirectionChanges(List<List<string>> allPaths)
    {
        // Ordonați căile în funcție de numărul de schimbări de direcție
        var pathsWithDirectionChanges = allPaths
            .Select(path => new 
            { 
                Path = path, 
                DirectionChanges = CountDirectionChanges(path) 
            })
            .OrderBy(p => p.DirectionChanges)
            .ToList();

        // Returnăm lista de căi sortate
        return pathsWithDirectionChanges
            .Select(p => p.Path)
            .ToList();
    }


    public List<string> FindOptimalPath(List<List<string>> allPaths)
    {
        foreach (var path in allPaths)
        {
            if (IsValidPath(path))
            {
                return path;
            }
        }

        return null;
    }

    private bool IsValidPath(List<string> path)
    {
        string previousDirection = null;
        int consecutiveCount = 0;

        for (int i = 0; i < path.Count; i++)
        {
            string currentDirection = path[i];

            if (currentDirection != previousDirection)
            {
                if (consecutiveCount >= 1)
                {
                    // Verifică dacă pe tile-ul curent există "red_0"
                    if (!IsTileRed(path, i))
                    {
                        return false;
                    }
                }

                consecutiveCount = 0;
            }
            else
            {
                consecutiveCount++;
            }

            previousDirection = currentDirection;
        }

        return true; // Calea este validă
    }

    private bool IsTileRed(List<string> path, int index)
    {
        Vector3 currentPos = start;
        Vector3[] directions = { Vector3.up / 2, Vector3.down / 2, Vector3.left / 2, Vector3.right / 2 };
        string[] directionNames = { "up", "down", "left", "right" };
        
        // Parcurge pașii căii până la index-ul curent
        for (int i = 0; i < index; i++)
        {
            currentPos += directions[Array.IndexOf(directionNames, path[i])];
        }

        Vector3Int cell = tilemap.WorldToCell(currentPos);
        TileBase tile = tilemap.GetTile(cell);
        
        return tile != null && tile.name == "red_0";
    }


    public int CountDirectionChanges(List<string> path)
    {
        if (path.Count < 2)
        {
            return 0;
        }
        int changes = 0;
        for (int i = 1; i < path.Count; i++)
        {
            if (path[i] != path[i - 1])
            {
                changes++;
            }
        }
        return changes;
    }


    private bool IsPositionInsideTilemap(Vector3 position)
    {
        Vector3Int cellPosition = tilemap.WorldToCell(position);
        return tilemap.HasTile(cellPosition);
    }

    private bool IsCurrentTileColor(Vector3 position, string color)
    {
        Vector3Int cellPosition = tilemap.WorldToCell(position);
        TileBase tile = tilemap.GetTile(cellPosition);

        return tile != null && tile.name.ToLower() == color.ToLower();
    }

    private Vector3 GetDirection(string action)
    {
        switch (action)
        {
            case "up": return Vector3.up / 2;
            case "down": return Vector3.down / 2;
            case "left": return Vector3.left / 2;
            case "right": return Vector3.right / 2;
            default: return Vector3.zero;
        }
    }
}
