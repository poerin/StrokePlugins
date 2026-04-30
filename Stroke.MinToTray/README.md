# Stroke.MinToTray

A plugin for the [Stroke](https://github.com/poerin/Stroke) mouse gesture engine that provides the ability to minimise a window to the system tray. It displays the software icon and window text in the notification area; clicking the icon restores the window.

## Table of Contents

- [Features](#features)
- [Requirements](#requirements)
- [Usage](#usage)
    - [Gesture Scripts](#gesture-scripts)
- [License](#license)

## Features

- **Minimise to tray**: Minimises the current window to the system tray and shows its icon.
- **Restore**: Left‑click the icon to restore the window.

## Requirements

- Stroke engine.
- .NET Framework 4.8 runtime.

## Usage

### Gesture Scripts

The plugin exposes a method `MinimizeToTray(IntPtr hWnd = default, string tipText = null, Icon customIcon = null)` to minimise a window to a tray icon.

Example code:

```csharp
MinToTray.MinimizeToTray();
```

## License

Distributed under the MIT License.