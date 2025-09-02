using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShowSolutionButton : MonoBehaviour
{
    private RocketController rocketController;
    private LevelManager levelManager;
    public Button showSolutionButton;
    public TMP_Text solutionLabel;
    public GameObject solutionPanel;
    public Button continueButton;

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
            solutionPanel.SetActive(true);
            if (rocketController.GetScore() < 50)
            {
                solutionLabel.text = "Not enough score";
            }
            else
            {
                rocketController.updateScore(-50);
                solutionLabel.text = "Optimal Path: " + string.Join(" -> ", levelManager.GetOptimalPath());
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
