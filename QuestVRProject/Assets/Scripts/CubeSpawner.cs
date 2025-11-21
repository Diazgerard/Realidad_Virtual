using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CubeSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject cubePrefab;
    public Transform[] spawnPoints;
    public float spawnInterval = 2f;
    public float spawnIntervalVariation = 0.5f;
    public int maxCubesOnScreen = 10;
    
    [Header("Spawn Area")]
    public Vector3 spawnAreaSize = new Vector3(10f, 5f, 10f);
    public bool useSpawnPoints = true;
    public bool visualizeSpawnArea = true;
    
    [Header("Dynamic Difficulty")]
    public bool increaseDifficulty = true;
    public float difficultyIncreaseRate = 0.9f; // Multiplicador cada 30 segundos
    public float minSpawnInterval = 0.5f;
    
    // Private variables
    private List<GameObject> activeCubes = new List<GameObject>();
    private bool isSpawning = false;
    private Coroutine spawnCoroutine;
    private float currentSpawnInterval;
    private GameManager gameManager;
    
    void Start()
    {
        Initialize();
    }
    
    void Initialize()
    {
        currentSpawnInterval = spawnInterval;
        gameManager = FindObjectOfType<GameManager>();
        
        // Si no hay spawn points definidos, crear algunos automáticamente
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            CreateAutoSpawnPoints();
        }
        
        // Limpiar lista de cubos
        activeCubes.Clear();
    }
    
    void CreateAutoSpawnPoints()
    {
        // Crear 8 puntos de spawn alrededor del área
        List<Transform> autoSpawnPoints = new List<Transform>();
        
        for (int i = 0; i < 8; i++)
        {
            GameObject spawnPoint = new GameObject($"AutoSpawnPoint_{i}");
            spawnPoint.transform.parent = transform;
            
            // Distribuir en círculo
            float angle = i * (360f / 8f) * Mathf.Deg2Rad;
            float radius = spawnAreaSize.x * 0.4f;
            
            Vector3 position = new Vector3(
                Mathf.Cos(angle) * radius,
                Random.Range(-spawnAreaSize.y * 0.5f, spawnAreaSize.y * 0.5f),
                Mathf.Sin(angle) * radius
            );
            
            spawnPoint.transform.localPosition = position;
            autoSpawnPoints.Add(spawnPoint.transform);
        }
        
        spawnPoints = autoSpawnPoints.ToArray();
    }
    
    public void StartSpawning()
    {
        if (isSpawning) return;
        
        isSpawning = true;
        spawnCoroutine = StartCoroutine(SpawnCoroutine());
        
        if (increaseDifficulty)
        {
            StartCoroutine(DifficultyIncreaseCoroutine());
        }
        
        Debug.Log("Cube spawning started!");
    }
    
    public void StopSpawning()
    {
        if (!isSpawning) return;
        
        isSpawning = false;
        
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
        
        // Opcional: destruir todos los cubos activos
        DestroyAllActiveCubes();
        
        Debug.Log("Cube spawning stopped!");
    }
    
    IEnumerator SpawnCoroutine()
    {
        while (isSpawning)
        {
            // Limpiar lista de cubos destruidos
            CleanupActiveCubes();
            
            // Solo spawnar si no hemos alcanzado el límite
            if (activeCubes.Count < maxCubesOnScreen)
            {
                SpawnCube();
            }
            
            // Esperar intervalo con variación
            float waitTime = currentSpawnInterval + Random.Range(-spawnIntervalVariation, spawnIntervalVariation);
            waitTime = Mathf.Max(waitTime, 0.1f); // Mínimo 0.1 segundos
            
            yield return new WaitForSeconds(waitTime);
        }
    }
    
    IEnumerator DifficultyIncreaseCoroutine()
    {
        while (isSpawning)
        {
            yield return new WaitForSeconds(30f); // Cada 30 segundos
            
            if (currentSpawnInterval > minSpawnInterval)
            {
                currentSpawnInterval *= difficultyIncreaseRate;
                currentSpawnInterval = Mathf.Max(currentSpawnInterval, minSpawnInterval);
                
                Debug.Log($"Difficulty increased! New spawn interval: {currentSpawnInterval:F2}s");
            }
        }
    }
    
    void SpawnCube()
    {
        if (cubePrefab == null)
        {
            Debug.LogWarning("Cube prefab is not assigned!");
            return;
        }
        
        Vector3 spawnPosition = GetRandomSpawnPosition();
        
        // Instantiate cube
        GameObject newCube = Instantiate(cubePrefab, spawnPosition, GetRandomRotation());
        
        // Configurar el cubo
        CollectableCube cubeScript = newCube.GetComponent<CollectableCube>();
        if (cubeScript == null)
        {
            cubeScript = newCube.AddComponent<CollectableCube>();
        }
        
        // Añadir a la lista de cubos activos
        activeCubes.Add(newCube);
        
        Debug.Log($"Spawned cube at {spawnPosition}");
    }
    
    Vector3 GetRandomSpawnPosition()
    {
        if (useSpawnPoints && spawnPoints != null && spawnPoints.Length > 0)
        {
            // Usar puntos de spawn definidos
            Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            
            // Añadir pequeña variación
            Vector3 variation = new Vector3(
                Random.Range(-0.5f, 0.5f),
                Random.Range(-0.5f, 0.5f),
                Random.Range(-0.5f, 0.5f)
            );
            
            return randomSpawnPoint.position + variation;
        }
        else
        {
            // Usar área de spawn aleatoria
            Vector3 randomPosition = new Vector3(
                Random.Range(-spawnAreaSize.x * 0.5f, spawnAreaSize.x * 0.5f),
                Random.Range(-spawnAreaSize.y * 0.5f, spawnAreaSize.y * 0.5f),
                Random.Range(-spawnAreaSize.z * 0.5f, spawnAreaSize.z * 0.5f)
            );
            
            return transform.position + randomPosition;
        }
    }
    
    Quaternion GetRandomRotation()
    {
        return Quaternion.Euler(
            Random.Range(0f, 360f),
            Random.Range(0f, 360f),
            Random.Range(0f, 360f)
        );
    }
    
    void CleanupActiveCubes()
    {
        activeCubes.RemoveAll(cube => cube == null);
    }
    
    void DestroyAllActiveCubes()
    {
        foreach (GameObject cube in activeCubes)
        {
            if (cube != null)
            {
                Destroy(cube);
            }
        }
        activeCubes.Clear();
    }
    
    void OnDrawGizmosSelected()
    {
        if (!visualizeSpawnArea) return;
        
        // Dibujar área de spawn
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, spawnAreaSize);
        
        // Dibujar puntos de spawn
        if (spawnPoints != null)
        {
            Gizmos.color = Color.red;
            foreach (Transform spawnPoint in spawnPoints)
            {
                if (spawnPoint != null)
                {
                    Gizmos.DrawWireSphere(spawnPoint.position, 0.3f);
                }
            }
        }
    }
    
    // Métodos públicos para control externo
    public void SetSpawnInterval(float newInterval)
    {
        currentSpawnInterval = Mathf.Max(newInterval, 0.1f);
    }
    
    public void SetMaxCubes(int maxCubes)
    {
        maxCubesOnScreen = Mathf.Max(maxCubes, 1);
    }
    
    public int GetActiveCubeCount()
    {
        CleanupActiveCubes();
        return activeCubes.Count;
    }
}