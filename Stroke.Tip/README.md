# Stroke.Tip

A plugin for the [Stroke](https://github.com/poerin/Stroke) mouse gesture engine that provides on‑screen text prompts. It displays tip text at the bottom of the primary screen.

## Table of Contents

- [Features](#features)
- [Requirements](#requirements)
- [Usage](#usage)
    - [Gesture Script](#gesture-script)
- [License](#license)

## Features

- Text prompt: displays tip text at the bottom of the primary screen.
- Customizable: allows customisation of text colour, size, and display duration via parameters.

## Requirements

- Stroke engine.
- .NET Framework 4.8 runtime.

## Usage

### Gesture Script

The plugin exposes a single public method `ShowTipText(string text, Color color, float fontSize = 26f, int durationMs = 500)` to display tip text.

Example code:

```csharp
// Display white tip text, size 26, for 1 second
Tip.ShowTipText("Save successful", Color.White, 26f, 1000);

// Display green tip text using default size and duration
Tip.ShowTipText("Recognising gesture...", Color.Lime);