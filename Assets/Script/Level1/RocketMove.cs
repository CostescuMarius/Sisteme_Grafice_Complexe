using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class RocketController : MonoBehaviour
{
    public float moveSpeed = 1f;
    public Tilemap tilemap;
    public GameObject levelCompletedPanel;
    public Button play;
    public Button restart;

    private Vector3 direction = Vector3.zero;
    private bool isMoving = false;
    private Queue<Command> commands = new Queue<Command>();

    public Button addCommandButton;
    public TMP_InputField commandInputField; 
    public int commandsLimit;

    void Start()
    {
        levelCompletedPanel.SetActive(false);
        // Exemplu de comenzi
        // commands.Enqueue(new Command("up"));
        // commands.Enqueue(new Command("right", "red_0"));

        if (play != null)
        {
            play.onClick.AddListener(OnButtonPlayClick);
        }

        if (restart != null)
        {
            restart.onClick.AddListener(OnButtonRestartClick);
        }

        if (addCommandButton != null)
        {
            addCommandButton.onClick.AddListener(OnAddCommandButtonClick);
        }
    }

    void OnButtonPlayClick()
    {
        StartCoroutine(ProcessCommands());
    }

    void OnButtonRestartClick()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        SceneManager.LoadScene(currentSceneName);
    }

    void OnAddCommandButtonClick()
    {
        if (commands.Count >= commandsLimit) {
            Debug.Log("Limit of commands exceded!");
            return;
        }

        string inputText = commandInputField.text.Trim();

        if (!string.IsNullOrEmpty(inputText))
        {
            AddCommand(inputText);
            commandInputField.text = "";
        }
    }

    void AddCommand(string input)
    {
        string[] parts = input.Split(' ');

        if (parts.Length == 1)
        {
            commands.Enqueue(new Command(parts[0]));
        }
        else if (parts.Length == 2)
        {
            commands.Enqueue(new Command(parts[0], parts[1]));
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Finish"))
        {
            levelCompletedPanel.SetActive(true);
            gameObject.SetActive(false);
            play.gameObject.SetActive(false);
            restart.gameObject.SetActive(false);
            addCommandButton.gameObject.SetActive(false);
            commandInputField.gameObject.SetActive(false);
        }
    }

    private IEnumerator ProcessCommands()
    {
        isMoving = true;
        while (commands.Count > 0 && isMoving)
        {
            Command currentCommand = commands.Peek();
            direction = GetDirection(currentCommand.action);
            commands.Dequeue();

            Command nextCommand;
            if(commands.Count > 0) {
                nextCommand = commands.Peek();
            } else {
                nextCommand = null;
            }

            if (nextCommand == null || (nextCommand != null && nextCommand.condition != null)) {
                while (true)
                {
                    Vector3 nextPosition = transform.position + direction;

                    if (!IsPositionInsideTilemap(nextPosition))
                    {
                        isMoving = false;
                        break;
                    }

                    if (nextCommand != null && nextCommand.condition != null && IsCurrentTileColor(nextCommand.condition))
                    {
                        direction = GetDirection(nextCommand.action);
                        nextPosition = transform.position + direction;
                        yield return MoveToTarget(nextPosition);
                        break;
                    }

                    yield return MoveToTarget(nextPosition);
                }
            }
            else {
                Vector3 nextPosition = transform.position + direction;
                yield return MoveToTarget(nextPosition);
            }
        }
    }

    private IEnumerator MoveToTarget(Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPosition;
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

    private bool IsPositionInsideTilemap(Vector3 position)
    {
        Vector3Int cellPosition = tilemap.WorldToCell(position);
        return tilemap.HasTile(cellPosition);
    }

    private bool IsCurrentTileColor(string color)
    {
        Vector3Int cellPosition = tilemap.WorldToCell(transform.position);
        TileBase tile = tilemap.GetTile(cellPosition);

        if (tile != null)
        {
            return tile.name.ToLower() == color.ToLower();
        }

        return false;
    }
}

public class Command
{
    public string action;
    public string condition;

    public Command(string action, string condition = null)
    {
        this.action = action;
        this.condition = condition;
    }
}
