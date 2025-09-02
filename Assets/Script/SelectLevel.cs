using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectLevel : MonoBehaviour
{
    public string startLevelButtonTag = "StartLevel1Button";
    public string startLevel2ButtonTag = "StartLevel2Button";
    public string startLevelRandomButtonTag = "StartLevel3Button";

    void Start()
    {
        GameObject startButton = GameObject.FindGameObjectWithTag(startLevelButtonTag);
        if (startButton != null)
        {
            UnityEngine.UI.Button button = startButton.GetComponent<UnityEngine.UI.Button>();
            button.onClick.AddListener(LoadLevel1);
        }

        GameObject startButton2 = GameObject.FindGameObjectWithTag(startLevel2ButtonTag);
        if (startButton2 != null)
        {
            UnityEngine.UI.Button button2 = startButton2.GetComponent<UnityEngine.UI.Button>();
            button2.onClick.AddListener(LoadLevel2);
        }

        GameObject startButtonRandom = GameObject.FindGameObjectWithTag(startLevelRandomButtonTag);
        if (startButtonRandom != null)
        {
            UnityEngine.UI.Button button3 = startButtonRandom.GetComponent<UnityEngine.UI.Button>();
            button3.onClick.AddListener(LoadLevelRandom);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            LoadMainMenu();
        }
    }

    public void LoadLevel1()
    {
        SceneManager.LoadScene("Level1");
    }

    public void LoadLevel2()
    {
        SceneManager.LoadScene("Level2");
    }

    public void LoadLevelRandom()
    {
        SceneManager.LoadScene("LevelRandom");
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
