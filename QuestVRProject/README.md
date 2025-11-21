# Quest VR Project

Un proyecto de realidad virtual para Meta Quest Pro desarrollado con Unity.

## Configuración del Proyecto

### Requisitos del Sistema
- Unity 2022.3 LTS o superior
- Meta XR SDK
- Android SDK
- Meta Quest Pro

### Configuración de Build
1. Platform: Android
2. Target API Level: API Level 32 (Android 12.0)
3. Minimum API Level: API Level 23 (Android 6.0)
4. XR Plugin Management: Oculus XR Plugin

### Scripts Principales
- `VRController.cs` - Controlador principal de VR
- `HandTracking.cs` - Sistema de seguimiento de manos
- `TeleportSystem.cs` - Sistema de teletransporte
- `InteractionManager.cs` - Gestor de interacciones

## Estructura del Proyecto
```
Assets/
├── Scripts/           # Scripts C#
├── Scenes/           # Escenas de Unity
├── Materials/        # Materiales
├── Prefabs/          # Prefabs
└── XR/              # Configuraciones XR
```

## Desarrollo
Este proyecto está configurado para trabajar con VS Code y Unity Editor.

### Comandos útiles
- F5: Build y deploy a Quest
- Ctrl+Shift+P: Command Palette de Unity en VS Code