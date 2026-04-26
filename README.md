# Vulume

Minimalist volume HUD for Windows. Optimized for high-contrast environments.

[![License: MIT](https://img.shields.io/badge/License-MIT-00FF41.svg?style=flat-square)](https://opensource.org/licenses/MIT)
[![Release](https://img.shields.io/github/v/release/KillerKannibal/Vulume?style=flat-square&color=00FF41)](https://github.com/KillerKannibal/Vulume/releases)


![Vulume HUD Preview] (media\Screenshot.png)
---

### Core
* **Engine:** .NET 8.0 (Self-Contained)
* **API:** Win32 (User32/Shell32)
* **Audio:** AudioSwitcher.AudioApi
* **Styling:** WPF / XAML

### Functionality
* **Dynamic Offsets:** Real-time vertical positioning via settings.
* **RGB Integration:** System-wide hex/RGB color matching via Extended WPF Toolkit.
* **OSD Override:** Optional integration with HideVolumeOSD to suppress native Windows elements.
* **Portability:** Zero-dependency execution (Standalone).

### Installation

1. Download `Vulume_Setup.exe` from [Releases](https://github.com/KillerKannibal/Vulume/releases).
2. Execute installer (Admin required for startup tasks).
3. Access configuration via System Tray context menu.

### Build from Source

```bash
# Requirements: .NET 8.0 SDK
git clone [https://github.com/KillerKannibal/Vulume.git](https://github.com/KillerKannibal/Vulume.git)
cd Vulume
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
