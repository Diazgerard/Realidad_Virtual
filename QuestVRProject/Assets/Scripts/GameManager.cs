using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    public float gameTime = 60f;
    public int targetScore = 500;
    
    [Header("UI Elements")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI gameOverText;
    public Button restartButton;
    
    [Header("Game Objects")]
    public CubeSpawner cubeSpawner;
    public AudioSource backgroundMusic;
    public AudioSource gameOverSound;
    
    // Game State
    private int currentScore = 0;
    private float currentTime;
    private bool gameActive = false;
    private bool gameWon = false;
    
    // Events
    public System.Action<int> OnScoreChanged;
    public System.Action<float> OnTimeChanged;
    public System.Action OnGameStart;
    public System.Action OnGameEnd;
    
    void Start()
    {
        InitializeGame();
    }
    
    void Update()
    {
        if (gameActive)
        {
            UpdateTimer();
        }
    }
    
    void InitializeGame()
    {
        currentTime = gameTime;
        currentScore = 0;
        gameActive = false;
        gameWon = false;
        
        UpdateScoreUI();
        UpdateTimerUI();
        
        if (gameOverText) gameOverText.gameObject.SetActive(false);
        if (restartButton) restartButton.gameObject.SetActive(false);
        
        // Start game after 3 seconds
        StartCoroutine(StartGameCountdown());
    }
    
    IEnumerator StartGameCountdown()
    {
        if (gameOverText)
        {
            gameOverText.text = "3";
            gameOverText.gameObject.SetActive(true);
            yield return new WaitForSeconds(1f);
            
            gameOverText.text = "2";
            yield return new WaitForSeconds(1f);
            
            gameOverText.text = "1";
            yield return new WaitForSeconds(1f);
            
            gameOverText.text = "¡GO!";
            yield return new WaitForSeconds(1f);
            
            gameOverText.gameObject.SetActive(false);
        }
        
        StartGame();
    }
    
    public void StartGame()
    {
        gameActive = true;
        OnGameStart?.Invoke();
        
        if (cubeSpawner) cubeSpawner.StartSpawning();
        if (backgroundMusic) backgroundMusic.Play();
        
        Debug.Log("Game Started!");
    }
    
    void UpdateTimer()
    {
        currentTime -= Time.deltaTime;
        OnTimeChanged?.Invoke(currentTime);
        UpdateTimerUI();
        
        if (currentTime <= 0)
        {
            currentTime = 0;
            EndGame();
        }
    }
    
    public void AddScore(int points)
    {
        if (!gameActive) return;
        
        currentScore += points;
        OnScoreChanged?.Invoke(currentScore);
        UpdateScoreUI();
        
        // Check win condition
        if (currentScore >= targetScore && !gameWon)
        {
            gameWon = true;
            EndGame();
        }
        
        Debug.Log($"Score: {currentScore}");
    }
    
    void EndGame()
    {
        gameActive = false;
        OnGameEnd?.Invoke();
        
        if (cubeSpawner) cubeSpawner.StopSpawning();
        if (backgroundMusic) backgroundMusic.Stop();
        if (gameOverSound) gameOverSound.Play();
        
        ShowGameOverUI();
        
        Debug.Log("Game Ended!");
    }
    
    void ShowGameOverUI()
    {
        if (gameOverText)
        {
            if (gameWon)
            {
                gameOverText.text = $"¡GANASTE!\nPuntuación: {currentScore}\nTiempo restante: {currentTime:F1}s";
            }
            else
            {
                gameOverText.text = $"¡TIEMPO AGOTADO!\nPuntuación Final: {currentScore}";
            }
            gameOverText.gameObject.SetActive(true);
        }
        
        if (restartButton) restartButton.gameObject.SetActive(true);
    }
    
    public void RestartGame()
    {
        InitializeGame();
    }
    
    void UpdateScoreUI()
    {
        if (scoreText)
            scoreText.text = $"Puntuación: {currentScore}";
    }
    
    void UpdateTimerUI()
    {
        if (timerText)
            timerText.text = $"Tiempo: {currentTime:F1}s";
    }
    
    // Getters for other scripts
    public int GetCurrentScore() => currentScore;
    public float GetCurrentTime() => currentTime;
    public bool IsGameActive() => gameActive;
}