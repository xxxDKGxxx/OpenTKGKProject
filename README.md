# OpenTKGKProject

A 3D graphics engine and real-time interactive simulation developed for the Computer Graphics (GK) course at the Faculty of Mathematics and Information Science (MiNI), Warsaw University of Technology (WUT).

![License](https://img.shields.io/badge/license-MIT-blue.svg)
![Platform](https://img.shields.io/badge/platform-Windows%20%7C%20Linux%20%7C%20macOS-0078d7.svg)
![Framework](https://img.shields.io/badge/.NET-8.0-512bd4.svg)
![Graphics API](https://img.shields.io/badge/OpenGL-4.6-5586a4.svg)
![Library](https://img.shields.io/badge/OpenTK-4.8.2-green.svg)
![UI](https://img.shields.io/badge/ImGui.NET-1.90.0-brightgreen.svg)

---

## 🚀 Overview

**OpenTKGKProject** implements a modern, deferred rendering pipeline in C# using **OpenTK** (OpenGL bindings) and **ImGui.NET**. The application features advanced lighting models, G-Buffer debug views, dynamic day/night cycles, shadow mapping, custom camera systems, and interactive scene controls.

---

## ✨ Features

### 🎨 Rendering Pipeline
- **Deferred Shading**: Implementation of a G-Buffer storing albedo, normals, and depth, allowing efficient rendering of scenes with high light counts.
- **Shadow Mapping**: Support for real-time shadows using layered shadow maps for directional and spotlights.
- **Multisample Anti-Aliasing (MSAA)**: Smooth geometry edges across deferred passes.
- **Atmospheric Fog**: Distance-based linear fog system for enhanced environmental depth.

### 💡 Lighting System
- **Dynamic Light Sources**:
  - **Directional Light**: Simulating sunlight with a real-time Day/Night cycle.
  - **Point Lights**: Omnidirectional lights with configurable attenuation.
  - **Spotlights**: Focused beams with adjustable inner/outer cutoff angles.
- **Object Light Attachments**: Dynamic attachment of light sources to moving entities (e.g., car headlights and taillights).
- **Visual Light Debugging**: Toggleable visual representation of light positions, bounds, and frustums.

### 🎥 Camera & Controls
- **Multiple Camera Modes**:
  - **Editor / FlyBy**: Free-roaming camera for effortless scene exploration.
  - **Orbital**: Rotates around a fixed target point.
  - **Follow / LookAt Object**: Specialized tracking modes for moving entities.
- **Projection Modes**: Real-time switching between Perspective and Orthographic projections.

### 📦 Asset Management & Procedural Geometry
- **Model Loading**: Integrated with **AssimpNet** and **StbImageSharp** for importing 3D models (FBX, OBJ) with textures, normals, and materials.
- **Procedural Geometry**: Custom C# generators for procedural spheres, cubes, planes, and terrain meshes.

### 🛠️ Real-Time ImGui Control Panel
- **G-Buffer Visualizer**: Debug single-frame G-Buffer attachments (Albedo, Normals, Depth).
- **Lighting Controls**: Adjust light color, intensity, attenuation, and spotlight angles live.
- **Time of Day Slider**: Interactive control over solar position and atmospheric lighting.
- **Performance Profiling**: Built-in FPS counter and frame time diagnostics.

---

## 🛠️ Tech Stack

- **Language**: C# (.NET 8.0)
- **Graphics API**: OpenGL 4.6 (via [OpenTK](https://github.com/opentk/opentk))
- **UI Library**: [ImGui.NET](https://github.com/ImGuiNET/ImGui.NET)
- **Asset Processing**: AssimpNet, StbImageSharp
- **Mathematics**: `OpenTK.Mathematics`

---

## 🎮 Controls

| Key / Input | Action |
| :--- | :--- |
| **W / A / S / D** | Move camera forward/left/backward/right (FlyBy/Editor modes) |
| **Q / E** | Move camera vertically down / up |
| **Mouse Drag** | Rotate camera view angle |
| **Esc** | Exit application |
| **Mouse Click** | Interact with ImGui control panels & UI sliders |

---

## 💻 Getting Started

### Prerequisites

- **.NET 8.0 SDK** or newer
- Graphics card with **OpenGL 4.6** support

### Running the Application

1. **Clone the repository**:
   ```bash
   git clone https://github.com/your-username/OpenTKGKProject.git
   cd OpenTKGKProject
   ```

2. **Build the solution**:
   ```bash
   dotnet build OpenTKGKProject.sln
   ```

3. **Run the main project**:
   ```bash
   dotnet run --project OpenTKGKProject/OpenTKGKProject.csproj
   ```

---

## 📂 Project Structure

```
OpenTKGKProject/
├── OpenTKGKProject.sln            # Main Visual Studio / Rider solution
├── ObjectOrientedOpenGL/          # Core OOP abstraction layer for OpenGL objects
│   ├── Core/                      # Buffers, Shaders, Textures, VAO abstractions
│   └── Extra/                     # ImGui OpenGL backend integration
└── OpenTKGKProject/               # Main application project
    ├── Program.cs                 # Engine entry point, rendering loop & scene setup
    └── Resources/                 # Game assets
        ├── Shaders/               # GLSL shaders (Deferred G-Buffer, Lighting pass, Shadows)
        ├── Models/                # 3D Mesh assets & model loaders
        └── Lights/                # Light source structures & properties
```

---

## 🙏 Acknowledgments

Special thanks to [Tomasz Herman](https://github.com/tomasz-herman) for providing the Object-Oriented OpenGL template.

---

## 📜 License

This project is licensed under the [MIT License](LICENSE).
