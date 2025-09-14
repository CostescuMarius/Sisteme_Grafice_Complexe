using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

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
        GenerateLevel();
    }

    public void GenerateLevel() {
        do {
            rocketPosition = GetRandomValidPosition();
            blackHolePosition = GetRandomValidPosition();
        } while (rocketPosition == blackHolePosition);

        rocket.transform.position = tilemap.GetCellCenterWorld(rocketPosition);
        blackHole.transform.position = tilemap.GetCellCenterWorld(blackHolePosition);

        pathFinder = new PathFinder(tilemap, tilemap.GetCellCenterWorld(rocketPosition), tilemap.GetCellCenterWorld(blackHolePosition));
        allPaths = pathFinder.FindAllPaths();
        sortedPaths = pathFinder.SortPathsByDirectionChanges(allPaths);

        optimalPath = null;
        int limit = 0;
        while (optimalPath == null && limit < 100000)
        {
            limit++;
            RemovePreviousObstacles();
            clearSolution();
            AddRandomObstacles(Random.Range(2, 5));
            optimalPath = pathFinder.FindOptimalPath(sortedPaths);
        }

        int maxCommands = pathFinder.CountDirectionChanges(optimalPath);
        RocketController rocketController = rocket.GetComponent<RocketController>();

        if (rocketController != null)
        {
            rocketController.SetMaxCommands(maxCommands);
            rocketController.SetInitialRocketPosition(rocket.transform.position);
            rocketController.resetButtons();
            //rocketController.resetCommandBoxes();
        }

        //Debug.Log("Path: " + string.Join(" -> ", optimalPath));
    }

    void AddRandomObstacles(int count)
    {
        placedObstacles.Clear();
        HashSet<int> usedRows = new HashSet<int>();
        HashSet<int> usedCols = new HashSet<int>();

        for (int i = 0; i < count; i++)
        {
            Vector3Int obstaclePosition;
            int attempts = 0;
            do
            {
                obstaclePosition = GetRandomValidPosition();
                attempts++;
                // Evităm pozițiile pe aceeași linie sau coloană
            } while ((usedRows.Contains(obstaclePosition.y) || usedCols.Contains(obstaclePosition.x) || IsPositionOccupied(obstaclePosition)) && attempts < 100);

            if (attempts < 100) // Ne asigurăm că nu intrăm într-un loop infinit
            {
                tilemap.SetTile(obstaclePosition, redTile);
                placedObstacles.Add(obstaclePosition);
                usedRows.Add(obstaclePosition.y);
                usedCols.Add(obstaclePosition.x);

                Debug.Log("X: " + obstaclePosition.y + ";     Y: " + obstaclePosition.y);
            }
        }
    }

    // void AddRandomObstacles(int count)
    // {
    //     placedObstacles.Clear();
    //     for (int i = 0; i < count; i++)
    //     {
    //         Vector3Int obstaclePosition = GetRandomValidPosition();

    //         if (!IsPositionOccupied(obstaclePosition))
    //         {
    //             tilemap.SetTile(obstaclePosition, redTile);
    //             placedObstacles.Add(obstaclePosition);
    //         }
    //     }
    // }

    void RemovePreviousObstacles()
    {
        foreach (var position in placedObstacles)
        {
            tilemap.SetTile(position, spaceTile);
        }
        placedObstacles.Clear();
    }

    void clearSolution()
    {
        TileBase eachTile;
        foreach (var position in tilemap.cellBounds.allPositionsWithin)
        {
            if (tilemap.HasTile(position)) {
                eachTile = tilemap.GetTile(position);

                //Debug.Log(eachTile.name.ToLower() + " contains space_solution: " + eachTile.name.ToLower().Contains("space-solution"));

                if (eachTile.name.ToLower().Contains("space-solution")) {
                    tilemap.SetTile(position, spaceTile);
                }
            }
        }
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
        while (!tilemap.HasTile(randomPosition) || (tilemap.HasTile(randomPosition) && !tilemap.GetTile(randomPosition).name.ToLower().Contains("space")));

        return randomPosition;
    }

    public List<string> GetOptimalPath() {
        return optimalPath;
    }

    public Vector3 GetStartPosition() {
        return rocket.transform.position;
    }
}