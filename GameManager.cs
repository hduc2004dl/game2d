using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [Header("Game State")]
    public int score = 0;
    public int keys = 0;
    public bool isPaused = false;
    
    [Header("UI References")]
    public UIManager uiManager;
    
    [Header("Player Reference")]
    public PlayerController player;
    
    void Awake()
    {
        // Singleton pattern with better error handling
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("GameManager initialized successfully");
        }
        else if (Instance != this)
        {
            Debug.Log("Duplicate GameManager detected, destroying...");
            Destroy(gameObject);
            return;
        }
    }
    
    void Start()
    {
        StartCoroutine(InitializeGameManager());
    }

    private System.Collections.IEnumerator InitializeGameManager()
    {
        // Wait a frame to ensure all objects are loaded
        yield return null;
        
        // Find UI Manager if not assigned
        if (uiManager == null)
        {
            uiManager = FindAnyObjectByType<UIManager>();
            if (uiManager == null)
            {
                Debug.LogWarning("UIManager not found in scene");
            }
        }
            
        // Find player if not assigned
        if (player == null)
        {
            player = FindAnyObjectByType<PlayerController>();
            if (player == null)
            {
                Debug.LogWarning("PlayerController not found in scene");
            }
        }
            
        // Initialize UI with null checks
        try
        {
            UpdateScoreUI();
            UpdateKeyUI();
            
            // Update health UI if player exists
            if (player != null)
            {
                UpdateHealthUI(player.maxHealth);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"GameManager initialization error: {e.Message}");
        }
        
        Debug.Log("GameManager initialization complete");
    }
    
    void Update()
    {
        // Pause functionality
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }
    
    public void AddScore(int points)
    {
        score += points;
        UpdateScoreUI();
    }
    
    public void AddKey()
    {
        keys++;
        UpdateKeyUI();
    }
    
    public bool UseKey()
    {
        if (keys > 0)
        {
            keys--;
            UpdateKeyUI();
            return true;
        }
        return false;
    }
    
    public void UpdateHealthUI(int health)
    {
        try
        {
            if (uiManager != null)
            {
                uiManager.UpdateHealth(health);
            }
            else
            {
                Debug.LogWarning("Cannot update Health UI - UIManager is null");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error updating Health UI: {e.Message}");
        }
    }
    
    public void UpdateScoreUI()
    {
        try
        {
            if (uiManager != null)
            {
                uiManager.UpdateScore(score);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error updating Score UI: {e.Message}");
        }
    }
    
    void UpdateKeyUI()
    {
        try
        {
            if (uiManager != null)
            {
                uiManager.UpdateKeys(keys);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error updating Key UI: {e.Message}");
        }
    }
    
    public void GameOver()
    {
        if (uiManager != null)
            uiManager.ShowGameOver();
        
        Time.timeScale = 0f;
        isPaused = true;
    }
    
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        
        if (uiManager != null)
            uiManager.ShowPauseMenu(isPaused);
    }
    
    public void LoadNextLevel()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
    }
    
    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0); // Assuming main menu is scene 0
    }
}
