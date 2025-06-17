using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Game UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI keyText;
    public Image[] healthHearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;
    
    [Header("Game Over")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;
    public Button restartButton;
    public Button mainMenuButton;
    
    [Header("Pause Menu")]
    public GameObject pausePanel;
    public Button resumeButton;
    public Button pauseRestartButton;
    public Button pauseMainMenuButton;
    
    void Start()
    {
        // Setup button listeners
        if (restartButton != null)
            restartButton.onClick.AddListener(() => GameManager.Instance.RestartGame());
            
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(() => GameManager.Instance.LoadMainMenu());
            
        if (resumeButton != null)
            resumeButton.onClick.AddListener(() => GameManager.Instance.TogglePause());
            
        if (pauseRestartButton != null)
            pauseRestartButton.onClick.AddListener(() => GameManager.Instance.RestartGame());
            
        if (pauseMainMenuButton != null)
            pauseMainMenuButton.onClick.AddListener(() => GameManager.Instance.LoadMainMenu());
        
        // Hide panels initially
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
    }
    
    public void UpdateScore(int score)
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score.ToString();
    }
    
    public void UpdateKeys(int keys)
    {
        if (keyText != null)
            keyText.text = "Keys: " + keys.ToString();
    }
    
    public void UpdateHealth(int health)
    {
        if (healthHearts == null || healthHearts.Length == 0) return;
        
        for (int i = 0; i < healthHearts.Length; i++)
        {
            if (i < health)
            {
                healthHearts[i].sprite = fullHeart;
                healthHearts[i].color = Color.white;
            }
            else
            {
                healthHearts[i].sprite = emptyHeart;
                healthHearts[i].color = Color.gray;
            }
        }
    }
    
    public void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            if (finalScoreText != null)
                finalScoreText.text = "Final Score: " + GameManager.Instance.score.ToString();
        }
    }
    
    public void ShowPauseMenu(bool show)
    {
        if (pausePanel != null)
            pausePanel.SetActive(show);
    }
}
