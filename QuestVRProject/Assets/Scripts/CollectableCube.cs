using UnityEngine;

public enum CubeType
{
    Green = 10,   // Verde - 10 puntos
    Blue = 20,    // Azul - 20 puntos
    Red = 30,     // Rojo - 30 puntos
    Gold = 50     // Dorado - 50 puntos (raro)
}

public class CollectableCube : MonoBehaviour
{
    [Header("Cube Settings")]
    public CubeType cubeType = CubeType.Green;
    public float floatSpeed = 1f;
    public float floatAmplitude = 0.5f;
    public float rotationSpeed = 30f;
    public float lifeTime = 15f;
    
    [Header("Visual Effects")]
    public ParticleSystem collectEffect;
    public Material[] cubeMaterials; // 0=Verde, 1=Azul, 2=Rojo, 3=Dorado
    
    [Header("Audio")]
    public AudioSource collectSound;
    public AudioClip[] collectSounds; // Sonidos diferentes por tipo
    
    // Private variables
    private Vector3 startPosition;
    private float floatTimer = 0f;
    private bool isCollected = false;
    private Rigidbody rb;
    private MeshRenderer meshRenderer;
    private GameManager gameManager;
    
    void Start()
    {
        Initialize();
    }
    
    void Initialize()
    {
        startPosition = transform.position;
        rb = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
        gameManager = FindObjectOfType<GameManager>();
        
        // Set random cube type based on rarity
        SetRandomCubeType();
        
        // Apply visual settings
        ApplyVisualSettings();
        
        // Set physics
        if (rb)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
        }
        
        // Auto destroy after lifetime
        Destroy(gameObject, lifeTime);
    }
    
    void Update()
    {
        if (!isCollected)
        {
            AnimateCube();
        }
    }
    
    void SetRandomCubeType()
    {
        float randomValue = Random.Range(0f, 100f);
        
        // Rareza de los cubos
        if (randomValue < 5f) // 5% Gold
        {
            cubeType = CubeType.Gold;
        }
        else if (randomValue < 20f) // 15% Red
        {
            cubeType = CubeType.Red;
        }
        else if (randomValue < 50f) // 30% Blue
        {
            cubeType = CubeType.Blue;
        }
        else // 50% Green
        {
            cubeType = CubeType.Green;
        }
    }
    
    void ApplyVisualSettings()
    {
        if (meshRenderer && cubeMaterials != null)
        {
            int materialIndex = 0;
            switch (cubeType)
            {
                case CubeType.Green: materialIndex = 0; break;
                case CubeType.Blue: materialIndex = 1; break;
                case CubeType.Red: materialIndex = 2; break;
                case CubeType.Gold: materialIndex = 3; break;
            }
            
            if (materialIndex < cubeMaterials.Length)
            {
                meshRenderer.material = cubeMaterials[materialIndex];
            }
        }
        
        // Tamaño especial para cubo dorado
        if (cubeType == CubeType.Gold)
        {
            transform.localScale = Vector3.one * 1.2f;
            rotationSpeed *= 1.5f;
        }
    }
    
    void AnimateCube()
    {
        // Flotación
        floatTimer += Time.deltaTime * floatSpeed;
        Vector3 newPosition = startPosition + Vector3.up * Mathf.Sin(floatTimer) * floatAmplitude;
        transform.position = newPosition;
        
        // Rotación
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        transform.Rotate(Vector3.right, rotationSpeed * 0.5f * Time.deltaTime);
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Verificar si es la mano del jugador
        if (other.CompareTag("Player") || other.CompareTag("Hand"))
        {
            CollectCube();
        }
    }
    
    public void CollectCube()
    {
        if (isCollected) return;
        
        isCollected = true;
        
        // Añadir puntos al GameManager
        if (gameManager)
        {
            gameManager.AddScore((int)cubeType);
        }
        
        // Efectos visuales
        PlayCollectEffect();
        
        // Efectos de sonido
        PlayCollectSound();
        
        // Ocultar cubo
        if (meshRenderer) meshRenderer.enabled = false;
        
        // Destruir después de efectos
        Destroy(gameObject, 1f);
        
        Debug.Log($"Collected {cubeType} cube! Points: {(int)cubeType}");
    }
    
    void PlayCollectEffect()
    {
        if (collectEffect)
        {
            collectEffect.Play();
            
            // Cambiar color del efecto según tipo de cubo
            var main = collectEffect.main;
            switch (cubeType)
            {
                case CubeType.Green:
                    main.startColor = Color.green;
                    break;
                case CubeType.Blue:
                    main.startColor = Color.blue;
                    break;
                case CubeType.Red:
                    main.startColor = Color.red;
                    break;
                case CubeType.Gold:
                    main.startColor = Color.yellow;
                    break;
            }
        }
    }
    
    void PlayCollectSound()
    {
        if (collectSound && collectSounds != null && collectSounds.Length > 0)
        {
            int soundIndex = Random.Range(0, collectSounds.Length);
            
            // Pitch diferente según rareza
            float pitch = 1f;
            switch (cubeType)
            {
                case CubeType.Green: pitch = 1f; break;
                case CubeType.Blue: pitch = 1.1f; break;
                case CubeType.Red: pitch = 1.2f; break;
                case CubeType.Gold: pitch = 1.5f; break;
            }
            
            collectSound.pitch = pitch;
            collectSound.clip = collectSounds[soundIndex];
            collectSound.Play();
        }
    }
    
    // Método público para recolección manual
    public void ForceCollect()
    {
        CollectCube();
    }
}