using UnityEngine;

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