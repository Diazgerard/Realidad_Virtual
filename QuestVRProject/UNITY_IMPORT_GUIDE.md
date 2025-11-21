# Cómo Importar el Juego "VR Cubes Collector" a Unity

## 📋 Pasos para Configurar el Proyecto en Unity

### 1. **Crear el Proyecto Unity**
1. Abre **Unity Hub**
2. Clic en **"New Project"**
3. Selecciona **"3D (Core)"** template
4. Nombre: **"VRCubesCollector"**
5. Ubicación: Donde prefieras
6. Version: **Unity 2022.3 LTS** o superior
7. Clic **"Create Project"**

### 2. **Configurar Platform Settings**
```
File → Build Settings
Platform: Android
Click "Switch Platform"
```

### 3. **Instalar Paquetes VR Necesarios**

#### XR Plugin Management:
```
Window → Package Manager
- Unity Registry
- Search: "XR Plugin Management"
- Install
```

#### XR Interaction Toolkit:
```
Package Manager
- Search: "XR Interaction Toolkit" 
- Install
- Import samples si aparece
```

#### TextMeshPro:
```
Package Manager  
- Search: "TextMeshPro"
- Install (si no está ya)
```

### 4. **Configurar XR Settings**
```
Edit → Project Settings → XR Plug-in Management
✅ Enable "Oculus" (o "Meta XR")
✅ Enable "Initialize XR on Startup"
```

### 5. **Configurar Android Settings**
```
Edit → Project Settings → Player → Android Settings

Configuration:
- Scribing Backend: IL2CPP
- Target Architectures: ✅ ARM64  
- Target API Level: 32 (Android 12)
- Minimum API Level: 23 (Android 6)

Identification:
- Company Name: Tu nombre
- Product Name: VR Cubes Collector
- Package Name: com.tunombre.vrcubescollector
```

## 📁 **IMPORTAR LOS SCRIPTS**

### Paso 1: Copiar Scripts
1. En Unity, ve a la carpeta **Assets**
2. Crea carpeta **"Scripts"** si no existe
3. **Copia todos los archivos .cs** de tu proyecto VS Code:
   - `GameManager.cs`
   - `CollectableCube.cs`  
   - `CubeSpawner.cs`
   - `VRController.cs`
   - `VRUIManager.cs`
   - `HandTracking.cs`
   - `TeleportSystem.cs`

### Paso 2: Verificar Scripts
- En Unity, todos los scripts deben aparecer en **Assets/Scripts**
- Si hay errores rojos, está bien por ahora (faltan componentes)

## 🎮 **CREAR LA ESCENA DEL JUEGO**

### 1. **Configurar Escena Base**
```
File → New Scene → Basic (Built-in)
File → Save Scene As → "GameScene"
```

### 2. **Eliminar Default Camera**
- Selecciona **Main Camera** en Hierarchy
- Presiona **Delete**

### 3. **Agregar XR Origin**
```
Hierarchy → Right click → XR → XR Origin (Action-based)
```

### 4. **Crear GameObjects Principales**

#### A) GameManager:
```
Hierarchy → Right click → Create Empty
Rename: "GameManager"
Add Component → Game Manager (script)
```

#### B) CubeSpawner:
```
Hierarchy → Right click → Create Empty  
Rename: "CubeSpawner"
Add Component → Cube Spawner (script)
```

#### C) VRController:
```
Select XR Origin
Add Component → VR Controller (script)
```

### 5. **Crear UI Canvas**
```
Hierarchy → Right click → UI → Canvas
Rename: "GameUI"
Canvas → Render Mode: World Space
Add Component → VR UI Manager (script)
```

### 6. **Crear Elementos UI**
En el Canvas "GameUI":
```
Right click GameUI → UI → Text - TextMeshPro
Rename: "ScoreText"
Text: "Puntuación: 0"

Right click GameUI → UI → Text - TextMeshPro  
Rename: "TimerText"
Text: "Tiempo: 60s"

Right click GameUI → UI → Text - TextMeshPro
Rename: "GameOverText" 
Text: "Game Over"

Right click GameUI → UI → Button - TextMeshPro
Rename: "RestartButton"
Text: "Reiniciar"
```

### 7. **Crear Cubo Prefab**
```
Hierarchy → Right click → 3D Object → Cube
Rename: "CollectableCube"
Add Component → Collectable Cube (script)
Add Component → Box Collider
✅ Is Trigger = true

Drag "CollectableCube" to Assets folder (crear prefab)
Delete from Hierarchy
```

### 8. **Crear Materiales para Cubos**
```
Assets → Right click → Create → Material
Names: "GreenCube", "BlueCube", "RedCube", "GoldCube"
Set colors: Verde, Azul, Rojo, Amarillo
```

## ⚙️ **CONFIGURAR COMPONENTES**

### 1. **GameManager Configuration:**
- Drag ScoreText → Score Text field
- Drag TimerText → Timer Text field  
- Drag GameOverText → Game Over Text field
- Drag RestartButton → Restart Button field
- Drag CubeSpawner → Cube Spawner field

### 2. **CubeSpawner Configuration:**
- Drag CollectableCube prefab → Cube Prefab field
- Set Spawn Interval: 2
- Set Max Cubes On Screen: 8

### 3. **CollectableCube Prefab:**
- Open prefab
- Drag materials array → Cube Materials
- Configure as needed

### 4. **VRUIManager Configuration:**
- Drag GameUI Canvas → Game UI field
- Drag all UI elements to respective fields

## 🎯 **CREAR ENTORNO DE JUEGO**

### 1. **Suelo/Plataforma:**
```
Hierarchy → 3D Object → Plane
Scale: (5, 1, 5)
Position: (0, -1, 0)
Name: "Floor"
```

### 2. **Iluminación:**
```
Hierarchy → Light → Directional Light
Position: (0, 10, 0)
Rotation: (50, 30, 0)
```

### 3. **Skybox (Opcional):**
```
Window → Rendering → Lighting
Environment → Skybox Material: Default
```

## 📱 **BUILD Y DEPLOY**

### 1. **Preparar Build:**
```
File → Build Settings
Add Open Scenes (GameScene)
Platform: Android ✅
```

### 2. **Conectar Quest:**
- Habilita **Developer Mode** en Quest
- Conecta USB-C
- Autoriza USB debugging

### 3. **Build and Run:**
```
Build Settings → Build and Run
Selecciona ubicación para APK
¡Espera la magia! 🎉
```

## 🐛 **Solución de Problemas Comunes**

### Scripts con errores:
- Verificar que todos los using statements estén
- Instalar TextMeshPro si da error TMP

### XR no funciona:
- Verificar Oculus XR Plugin habilitado
- Restart Unity después de cambios XR

### Build falla:
- Verificar Android SDK configurado
- Limpiar build cache

## 🎊 **¡FELICIDADES!**
¡Ya tienes tu juego VR funcionando! 

**Próximas mejoras que puedes hacer:**
- Añadir más tipos de cubos
- Power-ups especiales  
- Múltiples niveles
- Efectos de partículas
- Sonidos más elaborados
- Leaderboards

¿Necesitas ayuda con algún paso específico?