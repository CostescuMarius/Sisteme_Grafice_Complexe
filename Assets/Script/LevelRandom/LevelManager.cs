using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public Tilemap tilemap;
    public GameObject rocket;
    public GameObject blackHole;
    public TileBase redTile;
    public TileBase spaceTile;

    private PathFinder pathFinder;
    private List<List<string>> allPaths;
    private List<List<string>> sortedPaths;
    private List<string> optimalPath;

    private List<Vector3Int> placedObstacles = new List<Vector3Int>();

    Vector3Int rocketPosition;
    Vector3Int blackHolePosition;

    void Start()
    {
        //rocketController = rocket.GetComponent<RocketController>();

        do {
            rocketPosition = GetRandomValidPosition();
            blackHolePosition = GetRandomValidPosition();
        } while (rocketPosition == blackHolePosition);

        rocket.transform.position = tilemap.GetCellCenterWorld(rocketPosition);
        blackHole.transform.position = tilemap.GetCellCenterWorld(blackHolePosition);

        pathFinder = new PathFinder(tilemap, tilemap.GetCellCenterWorld(rocketPosition), tilemap.GetCellCenterWorld(blackHolePosition));
        allPaths = pathFinder.FindAllPaths();
        sortedPaths = pathFinder.SortPathsByDirectionChanges(allPaths);

        // optimalPath = pathFinder.FindOptimalPath(sortedPaths);

        // Debug.Log("Path: " + string.Join(" -> ", optimalPath));

        // Vector3Int redtilePosition = GetRandomValidPosition();
        // tilemap.SetTile(redtilePosition, redTile);

        optimalPath = null;
        int limit = 0;
        while (optimalPath == null && limit < 100000)
        {
            limit++;
            RemovePreviousObstacles();
            AddRandomObstacles(Random.Range(2, 5));
            optimalPath = pathFinder.FindOptimalPath(sortedPaths);
        }

        int maxCommands = pathFinder.CountDirectionChanges(optimalPath);
        RocketController rocketController = rocket.GetComponent<RocketController>();

        if (rocketController != null)
        {
            rocketController.SetMaxCommands(maxCommands);
            rocketController.SetInitialRocketPosition(rocket.transform.position);
        }

        //Debug.Log("Path: " + string.Join(" -> ", optimalPath));
    }

    void AddRandomObstacles(int count)
    {
        placedObstacles.Clear();
        for (int i = 0; i < count; i++)
        {
            Vector3Int obstaclePosition = GetRandomValidPosition();
            if (!IsPositionOccupied(obstaclePosition))
            {
                tilemap.SetTile(obstaclePosition, redTile);
                placedObstacles.Add(obstaclePosition);
            }
        }
    }

    void RemovePreviousObstacles()
    {
        foreach (var position in placedObstacles)
        {
            tilemap.SetTile(position, spaceTile);
        }
        placedObstacles.Clear();
    }

    bool IsPositionOccupied(Vector3Int position)
    {
        return position == tilemap.WorldToCell(rocket.transform.position) ||
               position == tilemap.WorldToCell(blackHole.transform.position);
    }

    Vector3Int GetRandomValidPosition()
    {
        BoundsInt bounds = tilemap.cellBounds;
        Vector3Int randomPosition;

        do
        {
            int x = Random.Range(bounds.xMin, bounds.xMax);
            int y = Random.Range(bounds.yMin, bounds.yMax);
            randomPosition = new Vector3Int(x, y, 0);
        }
        while (!tilemap.HasTile(randomPosition)); // Ne asigurăm că poziția e validă

        return randomPosition;
    }
}