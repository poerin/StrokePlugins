# Stroke.Tip

An on‑screen prompt plugin for the [Stroke](https://github.com/poerin/Stroke) mouse gesture engine. It renders tip text at the bottom of the primary display.

## Table of Contents

- [Features](#features)
- [Requirements](#requirements)
- [Usage](#usage)
    - [Gesture Scripts](#gesture-scripts)
- [License](#license)

## Features

- **Text prompt**: Renders tip text at the bottom of the primary display.
- **Customizable**: Supports custom text color, font size, and display duration through method parameters.

## Requirements

- Stroke engine.
- .NET Framework 4.8 runtime.

## Usage

### Gesture Scripts

The plugin exposes a single public method, `ShowTipText`, which accepts the tip text, a color, an optional font size, and an optional display duration in milliseconds.

```csharp
// Display white tip text at size 26 for one second.
Tip.ShowTipText("Save successful", Color.White, 26f, 1000);

// Display green tip text with default size and duration.
Tip.ShowTipText("Recognising gesture...", Color.Lime);
```

## License

Distributed under the MIT License.