using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI References")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI movesText;
    public GameObject winPanel;
    public TextMeshProUGUI winText;

    [Header("Win Condition")]
    public BoxHolder winHolder; // The holder that needs all boxes to win
    public int totalBoxes = 3; // Total number of boxes in the game

    private float gameTime = 0f;
    private int moveCount = 0;
    private bool gameActive = true;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        winPanel.SetActive(false);
        UpdateUI();
    }

    void Update()
    {
        if (gameActive)
        {
            gameTime += Time.deltaTime;
            UpdateTimerUI();
        }
    }

    public void IncrementMoves()
    {
        if (gameActive)
        {
            moveCount++;
            UpdateMovesUI();
        }
    }

    public void CheckWinCondition(BoxHolder holder)
    {
        if (!gameActive) return;

        // Check if the win holder has all the boxes
        if (holder == winHolder && holder.boxes.Count == totalBoxes)
        {
            WinGame();
        }
    }

    void WinGame()
    {
        gameActive = false;

        // Format the time nicely
        int minutes = Mathf.FloorToInt(gameTime / 60f);
        int seconds = Mathf.FloorToInt(gameTime % 60f);

        winText.text = $"You Win!\n\nTime: {minutes}:{seconds:00}\nMoves: {moveCount}";
        winPanel.SetActive(true);

        Debug.Log("Game Won!");
    }

    void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(gameTime / 60f);
        int seconds = Mathf.FloorToInt(gameTime % 60f);
        timerText.text = $"Time: {minutes}:{seconds:00}";
    }

    void UpdateMovesUI()
    {
        movesText.text = $"Moves: {moveCount}";
    }

    void UpdateUI()
    {
        UpdateTimerUI();
        UpdateMovesUI();
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MainMenu()
    {
        // If you have a main menu scene, load it here
        // SceneManager.LoadScene("MainMenu");

        // For now, just reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Debug.Log("Main Menu button pressed (reloading scene for now)");
    }
}