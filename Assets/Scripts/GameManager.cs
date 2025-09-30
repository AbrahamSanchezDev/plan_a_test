using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    // For testing in Task 2
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
        makeMoveButton.onClick.AddListener(SimulateMove); 
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
        currentScore += fixedPointsPerMove;
        UpdateUI();

        if (movesRemaining <= 0)
        {
            EndGame();
        }
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
}