using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CommandBox : MonoBehaviour
{
    public TMP_Text commandText;
    public GameObject directionMenu;
    public GameObject conditionMenu;

    private string direction = "";
    private string condition = "";
    private Button mainButton;

    public Button upButton;
    public Button downButton;
    public Button leftButton;
    public Button rightButton;
    public Button redButton;
    public Button whiteButton;

    void Start()
    {
        mainButton = GetComponent<Button>();

        directionMenu.SetActive(false);
        conditionMenu.SetActive(false);

        upButton.onClick.AddListener(() => OnDirectionButtonClick("up"));
        downButton.onClick.AddListener(() => OnDirectionButtonClick("down"));
        leftButton.onClick.AddListener(() => OnDirectionButtonClick("left"));
        rightButton.onClick.AddListener(() => OnDirectionButtonClick("right"));
        
        redButton.onClick.AddListener(() => OnConditionButtonClick("red_0"));
        whiteButton.onClick.AddListener(() => OnConditionButtonClick(""));
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 0 este pentru click stâng
        {
            if (IsMouseOverButton())
            {
                ToggleDirectionMenu();
            }
        }
        else if (Input.GetMouseButtonDown(1)) // 1 este pentru click dreapta
        {
            if (IsMouseOverButton())
            {
                ToggleConditionMenu();
            }
        }
    }

    bool IsMouseOverButton()
    {
        RectTransform rt = mainButton.GetComponent<RectTransform>();
        Vector2 localMousePosition = rt.InverseTransformPoint(Input.mousePosition);
        return rt.rect.Contains(localMousePosition);
    }

    void ToggleDirectionMenu()
    {
        if (directionMenu.activeSelf) 
        {
            directionMenu.SetActive(false);
        }
        else
        {
            directionMenu.SetActive(true);
        }

        conditionMenu.SetActive(false);
    }

    void ToggleConditionMenu()
    {
        if (conditionMenu.activeSelf)
        {
            conditionMenu.SetActive(false);
        }
        else
        {
            conditionMenu.SetActive(true);
        }

        directionMenu.SetActive(false);
    }



    // Functie pentru click stanga pe butonul de directie
    void OnDirectionButtonClick(string dir)
    {
        directionMenu.SetActive(false);

        direction = dir;

        TMP_Text directionButtonText = null;

        switch (dir)
        {
            case "up":
                directionButtonText = upButton.GetComponentInChildren<TMP_Text>();
                break;
            case "down":
                directionButtonText = downButton.GetComponentInChildren<TMP_Text>();
                break;
            case "left":
                directionButtonText = leftButton.GetComponentInChildren<TMP_Text>();
                break;
            case "right":
                directionButtonText = rightButton.GetComponentInChildren<TMP_Text>();
                break;
        }

        mainButton.GetComponentInChildren<TMP_Text>().text = directionButtonText.text;
    }

    // Functie pentru click dreapta pe butonul de conditie
    void OnConditionButtonClick(string cond)
    {
        conditionMenu.SetActive(false);
 
        condition = cond;

        switch (cond)
        {
            case "red_0":
                mainButton.GetComponent<Image>().color = Color.red;
                break;
            case "":
                mainButton.GetComponent<Image>().color = Color.white;
                break;
            default:
                mainButton.GetComponent<Image>().color = Color.white;
                break;
        }
    }

    public (string direction, string condition) getCommand() {
        return (direction, condition);
    }
}
