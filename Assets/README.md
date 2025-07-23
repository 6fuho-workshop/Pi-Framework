# Pi Framework (PF)

## Overview

**Pi Framework (PF)** is a modular Unity framework designed to accelerate the development of small games with a focus on good architecture, reusability, and coherence. PF acts as a higher layer on top of Unity, providing essential architecture and tools so you can build games quickly without starting from scratch each time. It leverages Unity’s platform while optimizing for speed and organization at the application level.

---

## Architecture

### 1. Application Layer Core

- **PiMain**: Entry point of the framework.
- **PiCore**: Initialization system for core services.
- **PiGame**: Game lifecycle manager.
- **PiModuleSystem**: Modular system that enables/disables modules as needed.

### 2. Modules (Enable/Disable per Game)

- **PiScene**: Scene management and transitions.
- **PiUI**: Simple UI stack system.
- **PiSave**: Game save system.
- **PiAudio**: Wrapper for AudioManager.
- **PiAnalytics**: Player behavior analytics.
- **PiInput**: Input mapping abstraction.

### 3. Tools / Editor Extensions

- **PF Dashboard**: Editor UI for enabling/disabling modules.
- **PF Game Template Wizard**: Quickly create new game structures (auto scene + folder).
- **PF Asset Tagger**: Organize and tag assets systematically.

---

## Key Features

- **Modular Design**: Add, remove, enable, or disable modules as needed for each game.
- **Rapid Prototyping**: Start new games quickly with built-in templates and tools.
- **Editor Integration**: Manage modules and project structure directly from the Unity Editor.
- **Reusable Components**: Common game systems (UI, audio, save, analytics, etc.) are provided as plug-and-play modules.
- **Console Commands**: Built-in developer console for runtime commands, debugging, and live tweaking.

---

## Getting Started

1. **Clone or import PF into your Unity project.**
2. Use the **PF Dashboard** in the Unity Editor to enable or disable modules as needed.
3. Use the **Game Template Wizard** to scaffold new games or scenes.
4. Reference and use modules (e.g., `PiAudio`, `PiSave`) in your game scripts.

---

## Example Usage
// Example: Toggle audio mute using PiAudio module Pi.audio.ToggleMute();

---

## Requirements

- Unity (recommended version: 2020.3 LTS or newer)
- .NET Framework 4.7.1
- C# 9.0

---

## Contributing

Contributions and feedback are welcome! Please document new modules and tools clearly, and follow the existing code style and modular structure.

---

## License

Specify your license here (e.g., MIT, Apache 2.0, etc.).

---

## Contact

For questions or support, please contact the maintainer or open an issue in your repository.
