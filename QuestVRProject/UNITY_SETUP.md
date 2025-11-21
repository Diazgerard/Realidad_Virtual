# Guía de Configuración Unity para Meta Quest Pro

## 1. Configuración Inicial del Proyecto Unity

### Crear Nuevo Proyecto
1. Abrir Unity Hub
2. Crear nuevo proyecto 3D
3. Nombrar: "QuestVRProject"
4. Seleccionar Unity 2022.3 LTS o superior

### Importar Archivos del Proyecto
1. Copiar la carpeta `Assets/Scripts` a tu proyecto Unity
2. Importar los scripts creados en VS Code

## 2. Configuración de Build Settings

### Platform Settings
```
File → Build Settings
- Platform: Android
- Switch Platform
```

### Player Settings
```
Edit → Project Settings → Player
```

#### XR Settings
- **XR Plug-in Management**
  - Install XR Plugin Management
  - Enable "Oculus" provider
  - Enable "Initialize XR on Startup"

#### Android Settings
- **Configuration**
  - Scripting Backend: IL2CPP
  - Target Architectures: ARM64
  - Target API Level: API Level 32 (Android 12.0)
  - Minimum API Level: API Level 23 (Android 6.0)

- **Identification**
  - Package Name: com.yourname.questvr
  - Version: 0.1
  - Bundle Version Code: 1

#### Quality Settings
```
Edit → Project Settings → Quality
- Set Default Quality Level to: Medium or High
- Disable shadows for better performance if needed
```

## 3. Instalación del Meta XR SDK

### Método 1: Package Manager
```
Window → Package Manager
- Unity Registry
- Search: "XR Interaction Toolkit"
- Install latest version
```

### Método 2: Meta XR All-in-One SDK
1. Descargar desde Meta Developer Portal
2. Import Custom Package en Unity
3. Seguir el setup wizard

## 4. Configuración de Escena

### Setup Básico de VR
1. Crear escena nueva: `Scenes/MainScene`
2. Eliminar Main Camera
3. Agregar XR Origin (Action-based) desde XR Interaction Toolkit

### Componentes Requeridos
- **XR Origin (Action-based)**
  - Camera Offset
  - Main Camera
  - LeftHand Controller
  - RightHand Controller

### Configurar Controladores
1. Seleccionar LeftHand Controller
   - Agregar XR Ray Interactor
   - Configurar Line Visual

2. Seleccionar RightHand Controller
   - Agregar XR Ray Interactor
   - Configurar Line Visual

## 5. Agregar Scripts del Proyecto

### VRController
1. Crear GameObject vacío: "VRManager"
2. Agregar script `VRController.cs`
3. Configurar parámetros en el inspector

### HandTracking
1. Agregar script `HandTracking.cs` al VRManager
2. Asignar modelos de manos si los tienes

### TeleportSystem
1. Crear GameObject: "TeleportSystem"
2. Agregar script `TeleportSystem.cs`
3. Configurar capa de suelo (Floor layer)

## 6. Configuración de Input Actions

### Crear Input Action Asset
```
Assets → Create → Input Actions
Nombre: "XR Input Actions"
```

### Configurar Acciones
- **Left Controller**
  - Position: [XR HMD] leftHandPosition
  - Rotation: [XR HMD] leftHandRotation
  - Primary2DAxis: [XR Controller] Primary2DAxis

- **Right Controller**
  - Position: [XR HMD] rightHandPosition  
  - Rotation: [XR HMD] rightHandRotation
  - Trigger: [XR Controller] Trigger
  - PrimaryButton: [XR Controller] Primary Button

## 7. Configuración de Layers y Physics

### Layers
```
Edit → Project Settings → Tags and Layers
```
Crear layers:
- Floor (Layer 8)
- Interactable (Layer 9)
- UI (Layer 10)

### Physics
```
Edit → Project Settings → Physics
```
- Configurar interacciones entre layers
- Gravity: (0, -9.81, 0)

## 8. Build y Deploy

### Preparar Build
1. `File → Build Settings`
2. Add Open Scenes
3. Player Settings configurado correctamente

### Conectar Quest Pro
1. Habilitar Developer Mode en Quest
2. Conectar via USB-C
3. Autorizar debugging USB

### Build y Run
1. `Build and Run` desde Unity
2. Seleccionar ubicación para APK
3. Wait para la instalación automática

## 9. Testing y Debug

### En Unity Editor
- Play Mode para testing básico
- VR Preview si tienes Link/Airlink configurado

### En Device
- Usar `adb logcat` para logs
- Unity Remote para debugging en tiempo real

### Comandos ADB Útiles
```powershell
# Ver devices conectados
adb devices

# Ver logs de Unity
adb logcat -s Unity

# Instalar APK manualmente
adb install -r path/to/your.apk

# Lanzar aplicación
adb shell am start -n com.yourname.questvr/com.unity3d.player.UnityPlayerActivity
```

## 10. Optimización para Quest Pro

### Performance Settings
- Target Frame Rate: 72Hz o 90Hz
- Fixed Foveated Rendering: Enabled
- Render Scale: 1.0-1.2

### Graphics Settings
- Use URP (Universal Render Pipeline) para mejor performance
- Lightmap baking para iluminación estática
- LOD Groups para modelos complejos

### Memory Management
- Texture Compression: ASTC
- Audio Compression: Vorbis
- Mesh Compression: High

## 11. Troubleshooting Común

### "No compatible devices found"
- Verificar USB debugging habilitado
- Reinstalar ADB drivers
- Usar cable USB oficial

### "Build failed"
- Verificar Android SDK path
- Limpiar Build cache
- Verificar Target API levels

### "XR not working"
- Verificar Oculus XR Plugin enabled
- Restart Unity después de cambios XR
- Verificar Initialize XR on Startup

## Siguientes Pasos

Una vez configurado, podrás:
1. Desarrollar scripts en VS Code
2. Sincronizar con Unity Editor
3. Test inmediato en Quest Pro
4. Usar hand tracking avanzado
5. Implementar passthrough AR

¿Necesitas ayuda con algún paso específico?