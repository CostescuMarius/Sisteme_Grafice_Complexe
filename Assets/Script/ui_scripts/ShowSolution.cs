using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class ShowSolutionButton : MonoBehaviour
{
    private RocketController rocketController;
    private LevelManager levelManager;
    public Button showSolutionButton;
    public TMP_Text solutionLabel;
    public GameObject solutionPanel;
    public Button continueButton;

    public Tilemap tilemap;
    public TileBase solutionTileUp;
    public TileBase solutionTileDown;
    public TileBase solutionTileLeft;
    public TileBase solutionTileRight;    

    void Start()
    {
        rocketController = FindFirstObjectByType<RocketController>();
        levelManager = FindFirstObjectByType<LevelManager>();

        if (showSolutionButton != null)
        {
            showSolutionButton.onClick.AddListener(OnShowSolutionClick);
        }

        if (continueButton != null)
        {
            continueButton.onClick.AddListener(OnContinueClick);
        }

        if (solutionPanel != null)
        {
            solutionPanel.SetActive(false);
        }
    }

    void OnShowSolutionClick()
    {
        if (rocketController != null && levelManager != null && solutionLabel != null && solutionPanel != null)
        {
            // if (rocketController.GetScore() < 50)
            // {
            //     solutionLabel.text = "Not enough score";
            //     solutionPanel.SetActive(true);
            // }
            // else
            // {
            //     showSolutionButton.gameObject.SetActive(false);
            //     rocketController.updateScore(-50);
            //     HighlightPath(levelManager.GetOptimalPath());
            //     //solutionLabel.text = "Optimal Path: " + string.Join(" -> ", levelManager.GetOptimalPath());
            // }

            HighlightPath(levelManager.GetOptimalPath());
        }
    }

    void HighlightPath(List<string> path)
    {
        if (tilemap == null || path == null || path.Count == 0) return;

        Vector3 current = levelManager.GetStartPosition();
        Vector3[] directions = { Vector3.up / 2, Vector3.down / 2, Vector3.left / 2, Vector3.right / 2 };
        string[] directionNames = { "up", "down", "left", "right" };

        Vector3Int currentTilePosition;
        TileBase currentTile;

        foreach (var step in path)
        {
            int idx = System.Array.IndexOf(directionNames, step);
            if (idx >= 0)
            {
                current += directions[idx];
                currentTilePosition = tilemap.WorldToCell(current);
                currentTile = tilemap.GetTile(currentTilePosition);
                //Debug.Log(directions[idx] + "   " + tilemap.HasTile(currentTilePosition) + "   " + currentTilePosition);
                if (tilemap.HasTile(currentTilePosition) && currentTile.name.ToLower() != "red_0")
                {
                    if (directionNames[idx] == "up") {
                        tilemap.SetTile(currentTilePosition, solutionTileUp);
                    }
                    else if (directionNames[idx] == "down") {
                        tilemap.SetTile(currentTilePosition, solutionTileDown);
                    }
                    else if (directionNames[idx] == "left") {
                        tilemap.SetTile(currentTilePosition, solutionTileLeft);
                    }
                    else {
                        tilemap.SetTile(currentTilePosition, solutionTileRight);
                    }
                }
            }
        }
    }

    void OnContinueClick()
    {
        if (solutionPanel != null)
        {
            solutionPanel.SetActive(false);
        }
    }
}
