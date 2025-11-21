# VR Cubes Collector - Lista de Assets Necesarios

## 📁 Estructura de Carpetas en Unity

```
Assets/
├── Scripts/
│   ├── GameManager.cs ✅
│   ├── CollectableCube.cs ✅  
│   ├── CubeSpawner.cs ✅
│   ├── VRController.cs ✅
│   ├── VRUIManager.cs ✅
│   ├── HandTracking.cs ✅
│   └── TeleportSystem.cs ✅
├── Materials/
│   ├── GreenCube.mat 🔄
│   ├── BlueCube.mat 🔄
│   ├── RedCube.mat 🔄
│   ├── GoldCube.mat 🔄
│   └── RayMaterial.mat 🔄
├── Prefabs/
│   ├── CollectableCube.prefab 🔄
│   ├── ScorePopup.prefab 🔄
│   └── ParticleEffect.prefab 🔄
├── Scenes/
│   └── GameScene.unity 🔄
├── Audio/ (Opcional)
│   ├── CollectSound.wav 🔄
│   ├── BackgroundMusic.ogg 🔄
│   └── GameOverSound.wav 🔄
└── UI/
    └── Fonts/ (Si necesitas)
```

## 🎨 Assets que Necesitas Crear en Unity

### 1. **Materiales (Assets/Materials/)**

#### GreenCube.mat:
- Albedo: Verde (#00FF00)
- Metallic: 0.2
- Smoothness: 0.8

#### BlueCube.mat:
- Albedo: Azul (#0080FF)  
- Metallic: 0.2
- Smoothness: 0.8

#### RedCube.mat:
- Albedo: Rojo (#FF0000)
- Metallic: 0.3
- Smoothness: 0.9

#### GoldCube.mat:
- Albedo: Dorado (#FFD700)
- Metallic: 0.8
- Smoothness: 0.9
- Emission: Dorado tenue

#### RayMaterial.mat:
- Shader: Sprites/Default
- Color: Blanco con alpha 0.5

### 2. **Prefabs que Crear:**

#### CollectableCube.prefab:
```
GameObject: Cube (primitivo)
Components:
- MeshRenderer + MeshFilter
- BoxCollider (IsTrigger = true)
- Rigidbody (UseGravity = false, IsKinematic = true)
- CollectableCube.cs script
- ParticleSystem (hijo)
```

#### ScorePopup.prefab:
```
GameObject: Canvas
- Canvas: World Space
- Child: Text (TextMeshPro)
  - Text: "+10"
  - Font Size: 2
  - Color: Blanco
```

### 3. **Configuraciones de Escena:**

#### Hierarchy Setup:
```
GameScene
├── XR Origin (Action-based)
│   ├── Camera Offset
│   │   ├── Main Camera
│   │   ├── LeftHand Controller  
│   │   └── RightHand Controller
│   └── VRController.cs
├── GameManager (Empty GameObject)
│   └── GameManager.cs
├── CubeSpawner (Empty GameObject)  
│   └── CubeSpawner.cs
├── GameUI (Canvas - World Space)
│   ├── ScoreText (TextMeshPro)
│   ├── TimerText (TextMeshPro)  
│   ├── GameOverText (TextMeshPro)
│   ├── RestartButton (Button)
│   ├── ProgressBar (Slider)
│   └── VRUIManager.cs
├── Floor (Plane)
├── Lighting
│   └── Directional Light
└── Environment (Empty - para organizar)
```

### 4. **Layers Recomendados:**
```
Layer 8: Floor
Layer 9: Interactable  
Layer 10: UI
Layer 11: Cubes
```

### 5. **Tags Necesarios:**
```
Player (para XR Origin)
Hand (para controladores)
Cube (para cubos coleccionables)
Floor (para suelo)
```

## 🔧 Configuraciones de Proyecto

### Physics Settings:
```
Edit → Project Settings → Physics
Gravity: (0, -9.81, 0)
Layer Collision Matrix:
- Cubes interact with Player/Hand
- Floor interacts with everything
```

### XR Settings:
```
Edit → Project Settings → XR Plug-in Management
✅ Oculus/Meta XR Provider
✅ Initialize XR on Startup

XR Interaction Toolkit:
✅ Use XR Device Simulator in Editor
```

### Quality Settings:
```
Edit → Project Settings → Quality  
Default Level: Medium-High
VSync Count: Don't Sync (VR handles this)
```

### Player Settings Android:
```
Configuration:
- Scripting Backend: IL2CPP
- Api Compatibility Level: .NET Standard 2.1
- Target Architectures: ARM64 ✅
- Internet Access: Not Required
- Install Location: Auto

Identification:
- Company Name: [Tu Nombre]
- Product Name: VR Cubes Collector  
- Package Name: com.tunombre.vrcubescollector
- Version: 1.0
- Bundle Version Code: 1

Resolution:
- Default Orientation: Landscape Left
- UI Orientation: Auto Rotation
```

## 📦 Packages Necesarios

### Instalar via Package Manager:
```
XR Plugin Management (2.4.x)
XR Interaction Toolkit (2.5.x)
XR Legacy Input Helpers (2.1.x)
TextMeshPro (3.0.x)
Universal RP (14.x) - Opcional para mejor performance
```

### Meta XR SDK (Opcional pero recomendado):
- Descargar desde Meta Developer Portal
- Importar como Custom Package
- Seguir Meta XR Setup Wizard

## 🎯 Orden de Creación Recomendado

1. **Crear materiales** primero
2. **Crear prefab CollectableCube** con materiales
3. **Configurar escena básica** (XR Origin, Floor, Lights)  
4. **Crear GameObjects** y asignar scripts
5. **Configurar UI Canvas** y elementos
6. **Conectar referencias** entre scripts
7. **Configurar spawn points** en CubeSpawner
8. **Test en Editor** con XR Device Simulator
9. **Build to Quest** para testing real

## ⚠️ Notas Importantes

- **Todos los scripts ya están listos** - solo copiarlos
- **Performance**: Mantén max 8-10 cubos simultáneos
- **UI Distance**: Canvas a 2m del jugador para comodidad
- **Hand Tracking**: Funciona con grab range de 0.5m
- **Haptics**: Configurado para Quest Pro controllers

¿Necesitas ayuda creando algún asset específico?