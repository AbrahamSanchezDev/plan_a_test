using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI movesText;

    private int currentScore = 0;
    private int movesRemaining = 5;

    void Start()
    {
        InitializeGame();
    }

    void InitializeGame()
    {
        currentScore = 0;
        movesRemaining = 5;
        gameOverPanel.SetActive(false);
        UpdateUI();
    }

    void UpdateUI()
    {
        scoreText.text = $"{currentScore}";
        movesText.text = $"{movesRemaining}";
    }
}
