# Stroke.Tip

An on‑screen notification plugin for the [Stroke](https://github.com/poerin/Stroke) mouse gesture engine. It displays temporary text labels at the bottom center of the display where the mouse is active.

## Table of Contents

- [Features](#features)
- [Requirements](#requirements)
- [Usage](#usage)
    - [Gesture Scripts](#gesture-scripts)
- [License](#license)

## Features

- **Display‑aware positioning**: The tip appears on the screen that currently contains the mouse cursor, which is particularly useful in multi‑monitor setups.
- **Fully customizable appearance**:
    - Text color (defaults to black).
    - Background color (automatic high‑contrast background when omitted; specific solid colors or `Color.Transparent` for no background).
    - Font family, size, and style (defaults to Microsoft YaHei, 26 pt, bold).
    - Display duration in milliseconds (defaults to 500 ms).
- **Hollow text support**: Setting the text color to `Color.Transparent` with a non‑transparent background produces a cut‑out text effect that reveals the desktop behind the tip.
- **Multiple overloads** for convenience, supporting both a `Font` object and separate font parameters.

## Requirements

- Stroke engine.
- .NET Framework 4.8 runtime.

## Usage

### Gesture Scripts

The plugin exposes the static method `Tip.ShowTipText` with three overloads:

```csharp
public static void ShowTipText(string text, int duration = 500, Color? color = null, Color? backColor = null);
public static void ShowTipText(string text, int duration, Color color, Color? backColor, Font font);
public static void ShowTipText(string text, int duration, Color color, Color? backColor, string fontFamily, float fontSize = 26f, FontStyle fontStyle = FontStyle.Bold);
```

```csharp
// Uses all default parameters.
Tip.ShowTipText("Example: all defaults");

// Custom duration and text color.
Tip.ShowTipText("Example: white text, 2 seconds", 2000, Color.White);

// Specifies a solid background color.
Tip.ShowTipText("Example: white text, dark blue background", 1500, Color.White, Color.DarkBlue);

// Completely transparent background; only the text is rendered.
Tip.ShowTipText("Example: transparent background, green text", 1000, Color.Lime, Color.Transparent);

// Hollow text effect: green background, text color set to Transparent reveals the desktop.
Tip.ShowTipText("Example: hollow effect, green background", 2000, Color.Transparent, Color.Lime);

// Font specified via a Font object.
Tip.ShowTipText("Example: Times New Roman regular", 800, Color.Black, null, new Font("Times New Roman", 20f, FontStyle.Regular));

// Font family, size, and style specified separately.
Tip.ShowTipText("Example: Courier New bold", 1200, Color.Orange, null, "Courier New", 18f, FontStyle.Bold);

// All optional parameters customized: color, background, and font.
Tip.ShowTipText("Example: cyan text, dark gray background, Consolas 22 pt italic", 2000, Color.Cyan, Color.FromArgb(40, 40, 40), "Consolas", 22f, FontStyle.Italic);
```

## License

Distributed under the MIT License.