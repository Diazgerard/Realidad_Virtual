using UnityEngine;
using UnityEngine.XR;

/// <summary>
/// Sistema de seguimiento de manos para Meta Quest Pro
/// Gestiona la detección y seguimiento de las manos del usuario
/// </summary>
public class HandTracking : MonoBehaviour
{
    [Header("Hand Tracking Settings")]
    public bool enableHandTracking = true;
    public float handConfidenceThreshold = 0.5f;
    
    [Header("Hand Models")]
    public GameObject leftHandModel;
    public GameObject rightHandModel;
    
    [Header("Debug")]
    public bool showHandDebug = true;
    
    // Estados de las manos
    private bool leftHandTracked = false;
    private bool rightHandTracked = false;
    
    // Poses de las manos
    private Vector3 leftHandPosition;
    private Quaternion leftHandRotation;
    private Vector3 rightHandPosition;
    private Quaternion rightHandRotation;
    
    void Start()
    {
        InitializeHandTracking();
    }
    
    void Update()
    {
        if (enableHandTracking)
        {
            UpdateHandTracking();
            UpdateHandGestures();
        }
    }
    
    /// <summary>
    /// Inicializa el sistema de seguimiento de manos
    /// </summary>
    private void InitializeHandTracking()
    {
        if (enableHandTracking)
        {
            Debug.Log("Initializing hand tracking for Meta Quest Pro");
            
            // Configurar modelos de manos si están asignados
            if (leftHandModel != null)
            {
                leftHandModel.SetActive(false);
            }
            
            if (rightHandModel != null)
            {
                rightHandModel.SetActive(false);
            }
        }
    }
    
    /// <summary>
    /// Actualiza el seguimiento de manos
    /// </summary>
    private void UpdateHandTracking()
    {
        // Seguimiento de la mano izquierda
        UpdateLeftHand();
        
        // Seguimiento de la mano derecha
        UpdateRightHand();
    }
    
    /// <summary>
    /// Actualiza la mano izquierda
    /// </summary>
    private void UpdateLeftHand()
    {
        var leftHandDevices = new System.Collections.Generic.List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, leftHandDevices);
        
        if (leftHandDevices.Count > 0)
        {
            var leftHandDevice = leftHandDevices[0];
            
            // Obtener posición y rotación
            if (leftHandDevice.TryGetFeatureValue(CommonUsages.devicePosition, out leftHandPosition) &&
                leftHandDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out leftHandRotation))
            {
                leftHandTracked = true;
                
                // Activar y posicionar el modelo de la mano
                if (leftHandModel != null)
                {
                    leftHandModel.SetActive(true);
                    leftHandModel.transform.position = leftHandPosition;
                    leftHandModel.transform.rotation = leftHandRotation;
                }
                
                if (showHandDebug)
                {
                    Debug.DrawRay(leftHandPosition, leftHandRotation * Vector3.forward, Color.red, 0.1f);
                }
            }
        }
        else
        {
            leftHandTracked = false;
            if (leftHandModel != null)
            {
                leftHandModel.SetActive(false);
            }
        }
    }
    
    /// <summary>
    /// Actualiza la mano derecha
    /// </summary>
    private void UpdateRightHand()
    {
        var rightHandDevices = new System.Collections.Generic.List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(XRNode.RightHand, rightHandDevices);
        
        if (rightHandDevices.Count > 0)
        {
            var rightHandDevice = rightHandDevices[0];
            
            // Obtener posición y rotación
            if (rightHandDevice.TryGetFeatureValue(CommonUsages.devicePosition, out rightHandPosition) &&
                rightHandDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out rightHandRotation))
            {
                rightHandTracked = true;
                
                // Activar y posicionar el modelo de la mano
                if (rightHandModel != null)
                {
                    rightHandModel.SetActive(true);
                    rightHandModel.transform.position = rightHandPosition;
                    rightHandModel.transform.rotation = rightHandRotation;
                }
                
                if (showHandDebug)
                {
                    Debug.DrawRay(rightHandPosition, rightHandRotation * Vector3.forward, Color.blue, 0.1f);
                }
            }
        }
        else
        {
            rightHandTracked = false;
            if (rightHandModel != null)
            {
                rightHandModel.SetActive(false);
            }
        }
    }
    
    /// <summary>
    /// Detecta gestos básicos de las manos
    /// </summary>
    private void UpdateHandGestures()
    {
        // Detectar gesto de "puño cerrado" (pinch)
        DetectPinchGesture();
        
        // Detectar gesto de "apuntar"
        DetectPointingGesture();
    }
    
    /// <summary>
    /// Detecta el gesto de pellizco (pinch)
    /// </summary>
    private void DetectPinchGesture()
    {
        // Esta funcionalidad requiere el Meta XR SDK completo
        // Por ahora implementamos una detección básica usando triggers
        
        var leftHandDevices = new System.Collections.Generic.List<InputDevice>();
        var rightHandDevices = new System.Collections.Generic.List<InputDevice>();
        
        InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, leftHandDevices);
        InputDevices.GetDevicesAtXRNode(XRNode.RightHand, rightHandDevices);
        
        // Detectar pinch en mano izquierda
        if (leftHandDevices.Count > 0)
        {
            if (leftHandDevices[0].TryGetFeatureValue(CommonUsages.trigger, out float leftTrigger))
            {
                if (leftTrigger > 0.8f)
                {
                    OnLeftHandPinch();
                }
            }
        }
        
        // Detectar pinch en mano derecha
        if (rightHandDevices.Count > 0)
        {
            if (rightHandDevices[0].TryGetFeatureValue(CommonUsages.trigger, out float rightTrigger))
            {
                if (rightTrigger > 0.8f)
                {
                    OnRightHandPinch();
                }
            }
        }
    }
    
    /// <summary>
    /// Detecta el gesto de apuntar
    /// </summary>
    private void DetectPointingGesture()
    {
        // Implementación básica del gesto de apuntar
        if (rightHandTracked)
        {
            // Crear un ray desde la mano derecha hacia adelante
            Ray pointingRay = new Ray(rightHandPosition, rightHandRotation * Vector3.forward);
            
            if (Physics.Raycast(pointingRay, out RaycastHit hit, 10f))
            {
                OnPointingAtObject(hit.collider.gameObject);
                
                if (showHandDebug)
                {
                    Debug.DrawRay(rightHandPosition, rightHandRotation * Vector3.forward * 10f, Color.yellow);
                }
            }
        }
    }
    
    /// <summary>
    /// Evento cuando la mano izquierda hace pinch
    /// </summary>
    private void OnLeftHandPinch()
    {
        Debug.Log("Left hand pinch detected");
        // Implementar acción específica para pinch izquierdo
    }
    
    /// <summary>
    /// Evento cuando la mano derecha hace pinch
    /// </summary>
    private void OnRightHandPinch()
    {
        Debug.Log("Right hand pinch detected");
        // Implementar acción específica para pinch derecho
    }
    
    /// <summary>
    /// Evento cuando se apunta a un objeto
    /// </summary>
    private void OnPointingAtObject(GameObject target)
    {
        Debug.Log($"Pointing at: {target.name}");
        // Implementar lógica de selección o highlight del objeto
    }
    
    /// <summary>
    /// Obtiene si la mano izquierda está siendo seguida
    /// </summary>
    public bool IsLeftHandTracked()
    {
        return leftHandTracked;
    }
    
    /// <summary>
    /// Obtiene si la mano derecha está siendo seguida
    /// </summary>
    public bool IsRightHandTracked()
    {
        return rightHandTracked;
    }
    
    /// <summary>
    /// Obtiene la posición de la mano izquierda
    /// </summary>
    public Vector3 GetLeftHandPosition()
    {
        return leftHandPosition;
    }
    
    /// <summary>
    /// Obtiene la posición de la mano derecha
    /// </summary>
    public Vector3 GetRightHandPosition()
    {
        return rightHandPosition;
    }
}