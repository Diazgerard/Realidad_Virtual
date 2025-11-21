using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class VRUIManager : MonoBehaviour
{
    [Header("UI Canvas")]
    public Canvas gameUI;
    public float uiDistance = 2f;
    public bool followPlayer = true;
    public float followSpeed = 2f;
    
    [Header("UI Elements")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI instructionsText;
    public Button restartButton;
    public Button menuButton;
    
    [Header("Progress Bar")]
    public Slider progressBar;
    public TextMeshProUGUI progressText;
    
    [Header("Combo System")]
    public TextMeshProUGUI comboText;
    public Image comboBackground;
    public float comboDisplayTime = 2f;
    
    [Header("Score Popup")]
    public GameObject scorePopupPrefab;
    public Transform popupParent;
    
    // Private variables
    private Camera playerCamera;
    private GameManager gameManager;
    private Vector3 initialUIPosition;
    private int currentCombo = 0;
    private Coroutine comboCoroutine;
    
    void Start()
    {
        Initialize();
    }
    
    void Update()
    {
        if (followPlayer && playerCamera != null)
        {
            UpdateUIPosition();
        }
    }
    
    void Initialize()
    {
        // Encontrar componentes necesarios
        playerCamera = Camera.main;
        if (playerCamera == null)
        {
            playerCamera = FindObjectOfType<Camera>();
        }
        
        gameManager = FindObjectOfType<GameManager>();
        
        // Configurar UI inicial
        SetupInitialUI();
        
        // Suscribirse a eventos del GameManager
        if (gameManager != null)
        {
            gameManager.OnScoreChanged += UpdateScore;
            gameManager.OnTimeChanged += UpdateTimer;
            gameManager.OnGameStart += OnGameStart;
            gameManager.OnGameEnd += OnGameEnd;
        }
        
        // Configurar botones
        if (restartButton) restartButton.onClick.AddListener(RestartGame);
        if (menuButton) menuButton.onClick.AddListener(ShowMenu);
        
        // Posición inicial de la UI
        if (gameUI) initialUIPosition = gameUI.transform.position;
    }
    
    void SetupInitialUI()
    {
        // Configurar Canvas para VR
        if (gameUI)
        {
            gameUI.renderMode = RenderMode.WorldSpace;
            gameUI.worldCamera = playerCamera;
            
            // Posicionar UI frente al jugador
            if (playerCamera)
            {
                Vector3 forward = playerCamera.transform.forward;
                forward.y = 0; // Mantener UI a nivel del jugador
                forward.Normalize();
                
                gameUI.transform.position = playerCamera.transform.position + forward * uiDistance;
                gameUI.transform.LookAt(playerCamera.transform);
                gameUI.transform.Rotate(0, 180, 0); // Girar para que mire al jugador
            }
        }
        
        // Mostrar instrucciones iniciales
        ShowInstructions();
        
        // Ocultar elementos de fin de juego
        if (gameOverText) gameOverText.gameObject.SetActive(false);
        if (restartButton) restartButton.gameObject.SetActive(false);
        if (comboText) comboText.gameObject.SetActive(false);
        if (comboBackground) comboBackground.gameObject.SetActive(false);
    }
    
    void UpdateUIPosition()
    {
        if (gameUI && playerCamera)
        {
            Vector3 targetPosition = CalculateUIPosition();
            gameUI.transform.position = Vector3.Lerp(gameUI.transform.position, targetPosition, followSpeed * Time.deltaTime);
            
            // Hacer que la UI siempre mire al jugador
            Vector3 lookDirection = playerCamera.transform.position - gameUI.transform.position;
            lookDirection.y = 0;
            gameUI.transform.rotation = Quaternion.LookRotation(-lookDirection);
        }
    }
    
    Vector3 CalculateUIPosition()
    {
        Vector3 forward = playerCamera.transform.forward;
        forward.y = 0;
        forward.Normalize();
        
        return playerCamera.transform.position + forward * uiDistance + Vector3.up * 0.5f;
    }
    
    void ShowInstructions()
    {
        if (instructionsText)
        {
            instructionsText.text = "¡Agarra los cubos con tus manos!\n\n" +
                                  "Verde = 10 puntos\n" +
                                  "Azul = 20 puntos\n" +
                                  "Rojo = 30 puntos\n" +
                                  "Dorado = 50 puntos\n\n" +
                                  "¡El juego comenzará en breve!";
            instructionsText.gameObject.SetActive(true);
        }
    }
    
    void OnGameStart()
    {
        if (instructionsText) instructionsText.gameObject.SetActive(false);
        ResetCombo();
    }
    
    void OnGameEnd()
    {
        // Los elementos de fin de juego se muestran desde GameManager
    }
    
    void UpdateScore(int newScore)
    {
        if (scoreText)
        {
            scoreText.text = $"Puntuación: {newScore}";
        }
        
        // Actualizar barra de progreso
        if (progressBar && gameManager)
        {
            float progress = (float)newScore / gameManager.targetScore;
            progressBar.value = progress;
            
            if (progressText)
            {
                progressText.text = $"{newScore}/{gameManager.targetScore}";
            }
        }
        
        // Incrementar combo
        IncrementCombo();
    }
    
    void UpdateTimer(float timeRemaining)
    {
        if (timerText)
        {
            timerText.text = $"Tiempo: {timeRemaining:F1}s";
            
            // Cambiar color cuando queda poco tiempo
            if (timeRemaining < 10f)
            {
                timerText.color = Color.red;
            }
            else if (timeRemaining < 30f)
            {
                timerText.color = Color.yellow;
            }
            else
            {
                timerText.color = Color.white;
            }
        }
    }
    
    void IncrementCombo()
    {
        currentCombo++;
        
        if (currentCombo > 1) // Solo mostrar combo a partir del segundo cubo consecutivo
        {
            ShowCombo();
        }
        
        // Reset combo timer
        if (comboCoroutine != null)
        {
            StopCoroutine(comboCoroutine);
        }
        comboCoroutine = StartCoroutine(ComboTimer());
    }
    
    void ShowCombo()
    {
        if (comboText && comboBackground)
        {
            comboText.text = $"COMBO x{currentCombo}!";
            comboText.gameObject.SetActive(true);
            comboBackground.gameObject.SetActive(true);
            
            // Efecto de escala
            StartCoroutine(ComboScaleEffect());
        }
    }
    
    IEnumerator ComboScaleEffect()
    {
        Vector3 originalScale = comboText.transform.localScale;
        Vector3 targetScale = originalScale * 1.2f;
        
        // Scale up
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 4f;
            comboText.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }
        
        // Scale down
        t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 4f;
            comboText.transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            yield return null;
        }
    }
    
    IEnumerator ComboTimer()
    {
        yield return new WaitForSeconds(comboDisplayTime);
        ResetCombo();
    }
    
    void ResetCombo()
    {
        currentCombo = 0;
        if (comboText) comboText.gameObject.SetActive(false);
        if (comboBackground) comboBackground.gameObject.SetActive(false);
    }
    
    public void ShowScorePopup(Vector3 worldPosition, int points)
    {
        if (scorePopupPrefab && popupParent)
        {
            GameObject popup = Instantiate(scorePopupPrefab, popupParent);
            popup.transform.position = worldPosition;
            
            TextMeshProUGUI popupText = popup.GetComponent<TextMeshProUGUI>();
            if (popupText)
            {
                popupText.text = $"+{points}";
            }
            
            // Animar popup
            StartCoroutine(AnimateScorePopup(popup));
        }
    }
    
    IEnumerator AnimateScorePopup(GameObject popup)
    {
        Vector3 startPos = popup.transform.position;
        Vector3 endPos = startPos + Vector3.up * 2f;
        
        TextMeshProUGUI text = popup.GetComponent<TextMeshProUGUI>();
        Color startColor = text.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);
        
        float duration = 1.5f;
        float t = 0;
        
        while (t < 1)
        {
            t += Time.deltaTime / duration;
            
            popup.transform.position = Vector3.Lerp(startPos, endPos, t);
            text.color = Color.Lerp(startColor, endColor, t);
            
            yield return null;
        }
        
        Destroy(popup);
    }
    
    void RestartGame()
    {
        if (gameManager)
        {
            gameManager.RestartGame();
        }
    }
    
    void ShowMenu()
    {
        // Implementar menú principal
        Debug.Log("Show Menu - To be implemented");
    }
    
    void OnDestroy()
    {
        // Desuscribirse de eventos
        if (gameManager != null)
        {
            gameManager.OnScoreChanged -= UpdateScore;
            gameManager.OnTimeChanged -= UpdateTimer;
            gameManager.OnGameStart -= OnGameStart;
            gameManager.OnGameEnd -= OnGameEnd;
        }
    }
}