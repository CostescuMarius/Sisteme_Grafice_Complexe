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
    public bool loopLevel;
    public Button play;
    public Button restart;

    private Vector3 direction = Vector3.zero;
    private bool isMoving = false;
    private Queue<Command> commands;

    private int maxCommands = 5;
    private Vector3 initialRocketPosition = new Vector3(0, 0, 0);


    public Vector3 finishPosition;

    public List<CommandBox> commandBoxes;

    private int score;
    public TMP_Text scoreText;

    void Start()
    {
        play.gameObject.SetActive(true);
        restart.gameObject.SetActive(false);

        score = 0;

        levelCompletedPanel.SetActive(false);

        if (play != null)
        {
            play.onClick.AddListener(OnButtonPlayClick);
        }

        if (restart != null)
        {
            restart.onClick.AddListener(OnButtonRestartClick);
        }

        GameObject finishObject = GameObject.FindGameObjectWithTag("Finish");
        if (finishObject != null)
        {
            finishPosition = finishObject.transform.position;
        }
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Select Level");
        }
    }

    public void resetButtons() {
        play.gameObject.SetActive(true);
        restart.gameObject.SetActive(false);
    }

    public void resetCommandBoxes() {
        foreach (var commandBox in commandBoxes)
        {
            commandBox.resetAspect();
        }
    }

    void OnButtonPlayClick()
    {
        play.gameObject.SetActive(false);
        restart.gameObject.SetActive(true);
        
        commands = new Queue<Command>();

        bool hasValidCommands = false;

        foreach (var commandBox in commandBoxes)
        {
            if (commandBox.gameObject.activeSelf) {
                var (direction, condition) = commandBox.getCommand();
                if (!string.IsNullOrEmpty(direction))
                {
                    AddCommand(direction, condition);
                    hasValidCommands = true;
                }
            }
        }

        // foreach (var command in commands)
        // {
        //     Debug.Log("Direcție: " + command.action + ", Condiție: " + (command.condition ?? "Fără condiție"));
        // }

        if (hasValidCommands)
        {
            StartCoroutine(ProcessCommands());
        }
        else
        {
            Debug.Log("No valid commands to process!");
        }
    }

    void OnButtonRestartClick()
    {
        play.gameObject.SetActive(true);
        restart.gameObject.SetActive(false);
        // string currentSceneName = SceneManager.GetActiveScene().name;

        // SceneManager.LoadScene(currentSceneName);
        StopAllCoroutines();
        transform.position = initialRocketPosition;
        isMoving = false; 
    }

    void AddCommand(string direction, string condition = null)
    {
        if (condition == "" || condition == null) {
            commands.Enqueue(new Command(direction));
        } else {
            commands.Enqueue(new Command(direction, condition));
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Finish"))
        {
            if (loopLevel) 
            {
                updateScore(10 * maxCommands);

                StopAllCoroutines();
                LevelManager levelManager = FindFirstObjectByType<LevelManager>();
                if (levelManager != null)
                {
                    levelManager.GenerateLevel();
                }
            }
            else 
            {
                updateScore(100);
                levelCompletedPanel.SetActive(true);
                gameObject.SetActive(false);
                play.gameObject.SetActive(false);
                restart.gameObject.SetActive(false);
            }
        }
    }

    public void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }

    public int GetScore()
    {
        return score;
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
                        Debug.Log(IsPositionInsideTilemap(nextPosition));
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
            else if(nextCommand != null && nextCommand.condition == null){
                Vector3 nextPosition = transform.position + direction;
                if (!IsPositionInsideTilemap(nextPosition))
                {
                    isMoving = false;
                    break;
                }
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

    public void SetMaxCommands(int max)
    {
        maxCommands = max + 1;

        // Dezactivează CommandBox-urile care depășesc limita
        for (int i = 0; i < commandBoxes.Count; i++)
        {
            if (i < maxCommands)
            {
                commandBoxes[i].gameObject.SetActive(true);
            }
            else
            {
                commandBoxes[i].gameObject.SetActive(false);
            }
        }
    }

    public void SetInitialRocketPosition(Vector3 pos) {
        initialRocketPosition = pos;
    }

    public void updateScore(int addScore) {
        score += addScore;
        UpdateScoreText();
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

