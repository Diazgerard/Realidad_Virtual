using UnityEngine;
using UnityEngine.XR;

/// <summary>
/// Sistema de teletransporte para Meta Quest Pro
/// Permite al usuario moverse por el entorno virtual de manera cómoda
/// </summary>
public class TeleportSystem : MonoBehaviour
{
    [Header("Teleport Settings")]
    public LayerMask teleportMask = 1; // Capa donde se puede teletransportar
    public float teleportRange = 10f;
    public float arcHeight = 2f;
    public int arcResolution = 30;
    
    [Header("Visual Feedback")]
    public LineRenderer teleportLine;
    public GameObject teleportMarker;
    public Color validTeleportColor = Color.green;
    public Color invalidTeleportColor = Color.red;
    
    [Header("Input")]
    public bool useRightController = true;
    public float joystickThreshold = 0.7f;
    
    // Estado del sistema
    private bool isTeleportActive = false;
    private bool canTeleport = false;
    private Vector3 teleportDestination;
    private InputDevice targetController;
    
    // Componentes
    private Camera playerCamera;
    private CharacterController playerCharacterController;
    
    void Start()
    {
        InitializeTeleportSystem();
    }
    
    void Update()
    {
        UpdateTeleportInput();
        
        if (isTeleportActive)
        {
            UpdateTeleportArc();
        }
    }
    
    /// <summary>
    /// Inicializa el sistema de teletransporte
    /// </summary>
    private void InitializeTeleportSystem()
    {
        // Obtener la cámara del jugador
        playerCamera = Camera.main;
        if (playerCamera == null)
        {
            playerCamera = FindObjectOfType<Camera>();
        }
        
        // Obtener el CharacterController del jugador
        playerCharacterController = GetComponent<CharacterController>();
        
        // Configurar el controlador objetivo
        SetupTargetController();
        
        // Configurar elementos visuales
        SetupVisualElements();
        
        Debug.Log("Teleport system initialized for Meta Quest Pro");
    }
    
    /// <summary>
    /// Configura el controlador objetivo para el teletransporte
    /// </summary>
    private void SetupTargetController()
    {
        var controllerDevices = new System.Collections.Generic.List<InputDevice>();
        
        if (useRightController)
        {
            InputDevices.GetDevicesAtXRNode(XRNode.RightHand, controllerDevices);
        }
        else
        {
            InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, controllerDevices);
        }
        
        if (controllerDevices.Count > 0)
        {
            targetController = controllerDevices[0];
            Debug.Log($"Teleport controller set: {targetController.name}");
        }
    }
    
    /// <summary>
    /// Configura los elementos visuales del teletransporte
    /// </summary>
    private void SetupVisualElements()
    {
        // Crear LineRenderer si no existe
        if (teleportLine == null)
        {
            GameObject lineObj = new GameObject("TeleportLine");
            lineObj.transform.SetParent(transform);
            teleportLine = lineObj.AddComponent<LineRenderer>();
            teleportLine.material = new Material(Shader.Find("Sprites/Default"));
            teleportLine.width = 0.05f;
            teleportLine.positionCount = arcResolution;
            teleportLine.enabled = false;
        }
        
        // Crear marcador de teletransporte si no existe
        if (teleportMarker == null)
        {
            teleportMarker = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            teleportMarker.name = "TeleportMarker";
            teleportMarker.transform.localScale = new Vector3(1f, 0.1f, 1f);
            teleportMarker.SetActive(false);
            
            // Hacer que el marcador no tenga colisión
            Destroy(teleportMarker.GetComponent<Collider>());
        }
    }
    
    /// <summary>
    /// Actualiza la entrada del teletransporte
    /// </summary>
    private void UpdateTeleportInput()
    {
        if (!targetController.isValid) return;
        
        // Detectar entrada del joystick para activar teletransporte
        if (targetController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 joystickValue))
        {
            bool joystickPressed = joystickValue.y > joystickThreshold;
            
            if (joystickPressed && !isTeleportActive)
            {
                StartTeleport();
            }
            else if (!joystickPressed && isTeleportActive)
            {
                EndTeleport();
            }
        }
    }
    
    /// <summary>
    /// Inicia el modo de teletransporte
    /// </summary>
    private void StartTeleport()
    {
        isTeleportActive = true;
        teleportLine.enabled = true;
        teleportMarker.SetActive(true);
        
        Debug.Log("Teleport mode activated");
    }
    
    /// <summary>
    /// Finaliza el modo de teletransporte
    /// </summary>
    private void EndTeleport()
    {
        if (canTeleport)
        {
            PerformTeleport();
        }
        
        isTeleportActive = false;
        canTeleport = false;
        teleportLine.enabled = false;
        teleportMarker.SetActive(false);
        
        Debug.Log("Teleport mode deactivated");
    }
    
    /// <summary>
    /// Actualiza el arco de teletransporte
    /// </summary>
    private void UpdateTeleportArc()
    {
        if (!targetController.isValid) return;
        
        // Obtener posición y rotación del controlador
        if (targetController.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 controllerPosition) &&
            targetController.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion controllerRotation))
        {
            // Calcular el arco parabólico
            Vector3[] arcPoints = CalculateTeleportArc(controllerPosition, controllerRotation);
            
            // Actualizar LineRenderer
            teleportLine.positionCount = arcPoints.Length;
            teleportLine.SetPositions(arcPoints);
            
            // Verificar si el punto de destino es válido
            ValidateTeleportDestination(arcPoints[arcPoints.Length - 1]);
        }
    }
    
    /// <summary>
    /// Calcula los puntos del arco de teletransporte
    /// </summary>
    private Vector3[] CalculateTeleportArc(Vector3 startPosition, Quaternion startRotation)
    {
        Vector3[] points = new Vector3[arcResolution];
        Vector3 forward = startRotation * Vector3.forward;
        
        // Calcular la velocidad inicial del arco
        Vector3 velocity = forward * teleportRange;
        velocity.y = arcHeight;
        
        Vector3 gravity = Physics.gravity;
        float timeStep = 0.1f;
        
        for (int i = 0; i < arcResolution; i++)
        {
            float t = i * timeStep;
            Vector3 point = startPosition + velocity * t + 0.5f * gravity * t * t;
            
            // Verificar colisión con el suelo
            if (Physics.Raycast(point, Vector3.down, out RaycastHit hit, 2f, teleportMask))
            {
                // Ajustar los puntos restantes al punto de colisión
                for (int j = i; j < arcResolution; j++)
                {
                    points[j] = hit.point;
                }
                teleportDestination = hit.point;
                break;
            }
            
            points[i] = point;
            teleportDestination = point;
        }
        
        return points;
    }
    
    /// <summary>
    /// Valida si el destino de teletransporte es válido
    /// </summary>
    private void ValidateTeleportDestination(Vector3 destination)
    {
        // Verificar si hay suelo en el destino
        if (Physics.Raycast(destination + Vector3.up, Vector3.down, out RaycastHit hit, 3f, teleportMask))
        {
            canTeleport = true;
            teleportDestination = hit.point;
            
            // Actualizar visuales para destino válido
            teleportLine.color = validTeleportColor;
            teleportMarker.transform.position = hit.point;
            teleportMarker.GetComponent<Renderer>().material.color = validTeleportColor;
        }
        else
        {
            canTeleport = false;
            
            // Actualizar visuales para destino inválido
            teleportLine.color = invalidTeleportColor;
            teleportMarker.GetComponent<Renderer>().material.color = invalidTeleportColor;
        }
    }
    
    /// <summary>
    /// Ejecuta el teletransporte al destino
    /// </summary>
    private void PerformTeleport()
    {
        if (!canTeleport) return;
        
        Vector3 headOffset = Vector3.zero;
        
        // Calcular el offset de la cámara si existe
        if (playerCamera != null)
        {
            headOffset = playerCamera.transform.localPosition;
            headOffset.y = 0; // Solo mantener el offset horizontal
        }
        
        // Calcular la nueva posición
        Vector3 newPosition = teleportDestination - headOffset;
        
        // Teletransportar usando CharacterController si existe
        if (playerCharacterController != null)
        {
            playerCharacterController.enabled = false;
            transform.position = newPosition;
            playerCharacterController.enabled = true;
        }
        else
        {
            // Teletransporte directo si no hay CharacterController
            transform.position = newPosition;
        }
        
        Debug.Log($"Teleported to: {teleportDestination}");
        
        // Opcional: Efecto visual de teletransporte
        StartCoroutine(TeleportEffect());
    }
    
    /// <summary>
    /// Efecto visual del teletransporte
    /// </summary>
    private System.Collections.IEnumerator TeleportEffect()
    {
        // Aquí se podría agregar un efecto de fade o partículas
        yield return new WaitForSeconds(0.1f);
        
        Debug.Log("Teleport effect completed");
    }
    
    /// <summary>
    /// Configura las capas válidas para teletransporte
    /// </summary>
    public void SetTeleportMask(LayerMask mask)
    {
        teleportMask = mask;
    }
    
    /// <summary>
    /// Habilita o deshabilita el sistema de teletransporte
    /// </summary>
    public void SetTeleportEnabled(bool enabled)
    {
        this.enabled = enabled;
        
        if (!enabled && isTeleportActive)
        {
            EndTeleport();
        }
    }
}