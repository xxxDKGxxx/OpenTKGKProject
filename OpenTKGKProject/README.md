# OpenTKGKProject

A professional 3D graphics engine and simulation developed for the Computer Graphics (GK) course at the Faculty of Mathematics and Information Science (MiNI), Warsaw University of Technology (WUT).

This project implements a modern rendering pipeline using OpenGL, focusing on deferred shading, advanced lighting models, and interactive scene management.

## Features

### Rendering Pipeline
- **Deferred Shading**: Implementation of a G-Buffer storing albedo, normals, and depth, allowing for efficient rendering of scenes with high light counts.
- **Shadow Mapping**: Support for real-time shadows using layered shadow maps for directional and spotlights.
- **Anti-Aliasing**: Multisample Anti-Aliasing (MSAA) for smoother geometry edges.
- **Fog System**: Distance-based linear fog for enhanced atmospheric depth.

### Lighting System
- **Dynamic Light Sources**:
    - **Directional Light**: Simulating sunlight with a Day/Night cycle.
    - **Point Lights**: Omnidirectional lights with configurable attenuation.
    - **Spotlights**: Focused beams with adjustable cutoff and outer cutoff angles.
- **Attachment System**: Ability to attach light sources to moving objects (e.g., car headlights and taillights).
- **Light Debugging**: Visual representation of light positions and directions in the scene.

### Camera & Control
- **Multiple Camera Modes**:
    - **Editor/FlyBy**: Free-roaming cameras for scene exploration.
    - **Orbital**: Rotates around a fixed point of interest.
    - **Follow/LookAt Object**: Specialized modes for tracking moving entities (e.g., the car).
- **Projection Modes**: Toggle between Perspective and Orthographic projections.

### Asset Management
- **Model Loading**: Integration with Assimp for importing complex 3D models (FBX, OBJ, etc.) with textures and normals.
- **Procedural Geometry**: Custom implementations for spheres, cubes, and terrain/ground.

### Interactive Interface
- **ImGui Integration**: A comprehensive real-time UI for:
    - Switching between G-Buffer debug views (Color, Normals, Depth).
    - Controlling light parameters (angles, colors, intensity).
    - Toggling time of day and debugging tools.
    - Monitoring performance via an FPS counter.

## Tech Stack
- **Language**: C# (.NET 8.0)
- **Graphics API**: OpenGL 4.6 (via OpenTK)
- **UI Library**: ImGui.NET
- **Asset Loading**: AssimpNet, StbImageSharp
- **Math Library**: OpenTK.Mathematics

## Controls
- **W/A/S/D**: Move camera (in FlyBy/Editor modes).
- **Mouse Movement**: Rotate camera.
- **Q/E**: Vertical movement (in FlyBy/Editor modes).
- **Esc**: Exit application.
- **UI Interaction**: Use the mouse to interact with the ImGui panels for various scene settings.

## Getting Started

### Prerequisites
- .NET 8.0 SDK
- A graphics card supporting OpenGL 4.6

### Installation
1. Clone the repository.
2. Ensure submodules or local project references (like `ObjectOrientedOpenGL`) are correctly linked.
3. Build the project:
   ```bash
   dotnet build
   ```
4. Run the application:
   ```bash
   dotnet run --project OpenTKGKProject
   ```

## Project Structure
- `Resources/Shaders`: GLSL shader programs for geometry, lighting, and shadow passes.
- `Resources/Models`: 3D model data and loaders.
- `Resources/Lights`: Implementations of various light types.
- `Program.cs`: Main application entry point and game loop orchestration.

## Acknowledgments
Special thanks to @tomasz-herman for letting me use his Object Oriented OpenGL for this project.
