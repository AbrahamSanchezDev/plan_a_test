using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Manages the overall game state, including initialization, score tracking, move management, and game-over handling.
/// </summary>
/// <remarks>This class implements a singleton pattern, providing a globally accessible instance via the <see
/// cref="Instance"/> property. It handles the game's lifecycle, including starting, resetting, and ending the game, as
/// well as updating the user interface to reflect the current score and remaining moves. The class also provides
/// methods for interacting with the game state, such as adding score, using moves, and checking if a move can be
/// made.</remarks>
public class GameManager : MonoBehaviour
{
    // Singleton pattern for easy access
    public static GameManager Instance { get; private set; }


    [Header("UI References")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI movesText;

    [Header("Game Settings")]
    public int initialMoves = 5;

    private int currentScore = 0;
    private int movesRemaining = 0;
    private bool isGameActive = true;


    [Header("Buttons")]
    [SerializeField]
    private Button replayButton;
    [SerializeField]
    private Button makeMoveButton;

    [Header("Debug")]
    // For testing in Task 2 - will be removed in Task 3
    [SerializeField]
    private int fixedPointsPerMove = 10;



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
        InitializeGame();

        replayButton.onClick.AddListener(InitializeGame);
        // For testing in Task 2
        //makeMoveButton.onClick.AddListener(SimulateMove);
    }

    public void InitializeGame()
    {
        currentScore = 0;
        movesRemaining = initialMoves;
        isGameActive = true;
        gameOverPanel.SetActive(false);
        UpdateUI();
    }

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = $"{currentScore}";
        if (movesText != null)
            movesText.text = $"{movesRemaining}";
    }

    // For testing in Task 2 - will be removed in Task 3
    public void SimulateMove()
    {
        if (!isGameActive)
        {
            Debug.Log("Game is over. Please restart to play again.");
            return;
        }

        movesRemaining--;
        // Fixed points for testing
        AddScore(fixedPointsPerMove);

        CheckForEndGame();
    }

    public void AddScore(int points)
    {
        currentScore += points;
        UpdateUI();
    }

    public void UseMove()
    {
        movesRemaining--;
        UpdateUI();
    }

    private void CheckForEndGame()
    {
        if (movesRemaining <= 0)
        {
            EndGame();
        }
    }

    void EndGame()
    {
        isGameActive = false;
        gameOverPanel.SetActive(true);
    }

    public bool CanMakeMove()
    {
        return isGameActive && movesRemaining > 0;
    }
    public int GetMovesRemaining()
    {
        return movesRemaining;
    }
    public void ShowGameOver()
    {
        EndGame();
    }
    public void ResetGame()
    {
        InitializeGame();
        // Reset grid
        if (GridManager.Instance != null)
        {
            GridManager.Instance.ResetGrid();
        }
    }
}