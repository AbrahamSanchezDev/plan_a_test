using UnityEngine;

/// <summary>
/// Manages the visibility of UI panels for different game states.
/// </summary>
/// <remarks>This class provides methods to toggle between the game panel and the game over panel. It is intended
/// to be used in Unity projects to control the user interface during gameplay.</remarks>
public class UIManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject gamePanel;
    public GameObject gameOverPanel;

    public void ShowGameOver()
    {
        gamePanel.SetActive(false);
        gameOverPanel.SetActive(true);
    }

    public void ShowGame()
    {
        gamePanel.SetActive(true);
        gameOverPanel.SetActive(false);
    }
}