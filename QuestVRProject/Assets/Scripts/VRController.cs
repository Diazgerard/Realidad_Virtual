using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;

/// <summary>
/// Controlador principal de VR para Meta Quest Pro - VR Cubes Collector Game
/// Gestiona la configuración inicial y sistemas básicos de VR, incluyendo interacción con cubos
/// </summary>
public class VRController : MonoBehaviour
{
    [Header("VR Configuration")]
    public bool enableHandTracking = true;
    public bool enablePassthrough = false;
    public float playAreaBoundary = 2.0f;

    [Header("Game Integration")]
    public GameManager gameManager;
    public float grabRange = 0.5f;
    public LayerMask cubeLayerMask = -1;

    [Header("Visual Feedback")]
    public LineRenderer leftHandRay;
    public LineRenderer rightHandRay;
    public Material rayMaterial;

    [Header("Haptic Feedback")]
    public bool enableHaptics = true;
    public float hapticIntensity = 0.5f;

    [Header("Debug")]
    public bool showDebugInfo = true;

    private InputDevice leftController;
    private InputDevice rightController;
    private bool isVRActive = false;

    void Start()
    {
        InitializeVR();
    }

    void Update()
    {
        if (isVRActive)
        {
            UpdateControllerInputs();
            
            if (showDebugInfo)
            {
                DisplayDebugInfo();
            }
        }
    }

    /// <summary>
    /// Inicializa el sistema de VR
    /// </summary>
    private void InitializeVR()
    {
        // Verificar si XR está disponible
        if (XRSettings.isDeviceActive)
        {
            isVRActive = true;
            Debug.Log("VR System initialized successfully for Meta Quest Pro");
            
            // Configurar controladores
            SetupControllers();
            
            // Configurar seguimiento de manos si está habilitado
            if (enableHandTracking)
            {
                SetupHandTracking();
            }
        }
        else
        {
            Debug.LogWarning("VR Device not found. Running in simulation mode.");
        }
    }

    /// <summary>
    /// Configura los controladores de Quest Pro
    /// </summary>
    private void SetupControllers()
    {
        var leftControllerDevices = new List<InputDevice>();
        var rightControllerDevices = new List<InputDevice>();
        
        InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, leftControllerDevices);
        InputDevices.GetDevicesAtXRNode(XRNode.RightHand, rightControllerDevices);

        if (leftControllerDevices.Count > 0)
        {
            leftController = leftControllerDevices[0];
            Debug.Log($"Left controller connected: {leftController.name}");
        }

        if (rightControllerDevices.Count > 0)
        {
            rightController = rightControllerDevices[0];
            Debug.Log($"Right controller connected: {rightController.name}");
        }
    }

    /// <summary>
    /// Configura el seguimiento de manos
    /// </summary>
    private void SetupHandTracking()
    {
        Debug.Log("Hand tracking enabled for Meta Quest Pro");
        // Aquí se implementaría la lógica específica del hand tracking
        // Requiere el Meta XR SDK
    }

    /// <summary>
    /// Actualiza las entradas de los controladores
    /// </summary>
    private void UpdateControllerInputs()
    {
        // Botón A del controlador derecho
        if (rightController.TryGetFeatureValue(CommonUsages.primaryButton, out bool aButton) && aButton)
        {
            OnAButtonPressed();
        }

        // Trigger derecho
        if (rightController.TryGetFeatureValue(CommonUsages.trigger, out float rightTrigger) && rightTrigger > 0.5f)
        {
            OnRightTriggerPressed(rightTrigger);
        }

        // Joystick izquierdo para movimiento
        if (leftController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 leftJoystick))
        {
            OnLeftJoystickMoved(leftJoystick);
        }
    }

    /// <summary>
    /// Maneja el evento del botón A
    /// </summary>
    private void OnAButtonPressed()
    {
        Debug.Log("A Button pressed on right controller");
        // Implementar acción específica
    }

    /// <summary>
    /// Maneja el evento del trigger derecho - Interacción con cubos
    /// </summary>
    private void OnRightTriggerPressed(float value)
    {
        Debug.Log($"Right trigger pressed: {value}");
        
        // Intentar agarrar cubo cercano con la mano derecha
        if (value > 0.8f) // Trigger completamente presionado
        {
            TryGrabCube(XRNode.RightHand);
            
            // Haptic feedback
            if (enableHaptics)
            {
                SendHapticFeedback(rightController, hapticIntensity, 0.1f);
            }
        }
    }

    /// <summary>
    /// Maneja el movimiento del joystick izquierdo
    /// </summary>
    private void OnLeftJoystickMoved(Vector2 joystickValue)
    {
        if (joystickValue.magnitude > 0.1f)
        {
            // Implementar movimiento del jugador
            Vector3 movement = new Vector3(joystickValue.x, 0, joystickValue.y);
            transform.Translate(movement * Time.deltaTime * 2.0f);
        }
    }

    /// <summary>
    /// Muestra información de debug en pantalla
    /// </summary>
    private void DisplayDebugInfo()
    {
        // Esta función se podría expandir con una UI de debug
        if (Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log($"VR Active: {isVRActive}");
            Debug.Log($"Left Controller Connected: {leftController.isValid}");
            Debug.Log($"Right Controller Connected: {rightController.isValid}");
            Debug.Log($"Hand Tracking: {enableHandTracking}");
        }
    }

    /// <summary>
    /// Intenta agarrar un cubo cercano con la mano especificada
    /// </summary>
    private void TryGrabCube(XRNode handNode)
    {
        // Obtener posición de la mano
        Vector3 handPosition = GetHandPosition(handNode);
        
        // Buscar cubos en rango
        Collider[] nearbyObjects = Physics.OverlapSphere(handPosition, grabRange, cubeLayerMask);
        
        CollectableCube closestCube = null;
        float closestDistance = float.MaxValue;
        
        foreach (Collider col in nearbyObjects)
        {
            CollectableCube cube = col.GetComponent<CollectableCube>();
            if (cube != null)
            {
                float distance = Vector3.Distance(handPosition, col.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestCube = cube;
                }
            }
        }
        
        // Recolectar el cubo más cercano
        if (closestCube != null)
        {
            closestCube.CollectCube();
            Debug.Log($"Cube collected with {handNode} hand!");
        }
    }
    
    /// <summary>
    /// Obtiene la posición de una mano específica
    /// </summary>
    private Vector3 GetHandPosition(XRNode handNode)
    {
        InputDevice device = (handNode == XRNode.LeftHand) ? leftController : rightController;
        
        if (device.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 position))
        {
            return position;
        }
        
        return transform.position; // Fallback
    }
    
    /// <summary>
    /// Envía feedback háptico al controlador
    /// </summary>
    private void SendHapticFeedback(InputDevice device, float amplitude, float duration)
    {
        if (device.isValid)
        {
            device.SendHapticImpulse(0, amplitude, duration);
        }
    }
    
    /// <summary>
    /// Alternar el modo passthrough (ver el mundo real)
    /// </summary>
    public void TogglePassthrough()
    {
        enablePassthrough = !enablePassthrough;
        Debug.Log($"Passthrough mode: {(enablePassthrough ? "Enabled" : "Disabled")}");
        // Implementar lógica específica del passthrough con Meta XR SDK
    }
    
    /// <summary>
    /// Método público para que otros scripts puedan activar haptics
    /// </summary>
    public void TriggerHapticFeedback(bool isLeftHand, float intensity = 0.5f, float duration = 0.1f)
    {
        InputDevice device = isLeftHand ? leftController : rightController;
        SendHapticFeedback(device, intensity, duration);
    }
}