# StrokePlugins

**Plugin collection for [Stroke](https://github.com/poerin/Stroke) mouse gesture engine.**

This repository hosts a set of plugins that extend Stroke’s capabilities. Each plugin is distributed as a self-contained .NET class library (DLL) and is designed to be dropped into Stroke’s working directory for instant gesture-driven access.

![License](https://img.shields.io/badge/license-MIT-blue.svg)
![Platform](https://img.shields.io/badge/platform-Windows-lightgrey)

[查看中文版文档](README_CN.md)

## Table of Contents

- [Available Plugins](#available-plugins)
- [Installation & Usage](#installation--usage)
- [Developing Your Own Plugin](#developing-your-own-plugin)
- [Contributing](#contributing)
- [License](#license)

## Available Plugins

| Plugin | Description |
|---|---|
| [Stroke.TrayIcon](Stroke.TrayIcon) | System tray icon with pause / resume and exit controls. |
| [Stroke.TranslateMate](Stroke.TranslateMate) | Clipboard-triggered Chinese-English translation via Baidu Translation API, with Youdao Dictionary pronunciation and vocabulary notebook. |
| [Stroke.PaddleOcrMate](Stroke.PaddleOcrMate) | Optical character recognition (OCR) via PaddleOCR, driven by clipboard images or file paths. |
| [Stroke.Tip](Stroke.Tip) | Renders customizable tip text at the bottom of the screen, supporting font color, size, and display duration. |
| [Stroke.MinToTray](Stroke.MinToTray) | Minimizes the active window to the system tray and displays a corresponding notification area icon. |

Contributions for new plugins are welcome.

## Installation & Usage

1. Download the latest release of the desired plugin from this repository, or compile the source code yourself.
2. Place the resulting `.dll` file (e.g., `Stroke.TranslateMate.dll`) into the same folder as `Stroke.exe`.
3. Restart Stroke.
4. Configure a gesture in the Stroke configurator to call the plugin’s public API.

Each plugin exposes its functionality through a static class in the `Stroke` namespace. You call its methods directly in your Stroke action scripts. Refer to the individual plugin READMEs for API credentials and example scripts.

## Developing Your Own Plugin

**The recommended approach is to use the included scaffolding tool to generate the project structure.**

1. Run `StrokePluginScaffold.exe` located in the `StrokePluginScaffold` directory, follow the prompts, and a solution with the correct namespaces and references will be generated automatically.
2. Open the `.sln` file inside the newly created `Stroke.<PluginName>` folder and start coding.

**Alternatively, create the project manually:**

1. Create a new Class Library project targeting **.NET Framework 4.8**.
2. Use the `Stroke` namespace for your public static class.
3. Implement your logic and expose `static` methods that Stroke scripts can call.
4. Name your DLL using the convention `Stroke.<PluginName>.dll`.

If your plugin needs to reference types from the Stroke engine, add the Stroke project as a reference. The recommended way is to include Stroke source code as a project reference in your solution, ensuring your plugin builds against the latest Stroke source (the scaffolding tool can optionally create this reference for you).

## Contributing

New plugins, improvements, and bug fixes are welcome. Please open an issue or submit a pull request.

**Pull request guidelines:**
- Squash all commits into one before merging. The final commit message should follow the format: `YYYY-MM-DD-X` (today’s date + sequence number of the day, e.g., `2026-04-30-1`). This keeps the history clean and easy to navigate.

**Code style:**
- Prioritize self-explanatory code through clear, descriptive variable names and logic. Avoid unnecessary comments — let the code speak for itself.
- To help unify identifier naming and text content, you can use the following prompt with an AI assistant:

```
# Code Identifier and Text Content Optimization

Please help me optimize the code with the following requirements:

## Optimization Scope

- Code identifiers: includes all identifiers such as variable names, method names, class names, function names, etc.
- Text content: covers all user-facing text in the code, including exception messages, log messages, print outputs, and comments.

## Naming Conventions

- Use complete words to express meaning; avoid obscure or custom abbreviations.
- Only widely accepted abbreviations are allowed (e.g., `id`, `url`, `max`, `min`, etc.).
- Ensure names are concise and accurately reflect their data, functionality, or logical meaning.
- Follow the naming conventions of the corresponding programming language.

## Modification Restrictions

- Only modify names and text content; do not change code structure, logic, or functionality.
- Do not add comments unless necessary. Remove existing comments as much as possible, allowing the code to be self-explanatory through clear naming.

## Output Format

- Present the optimized code within a standard code block.
- When mixing Chinese and English, ensure there is a standard space between Chinese and English words.
- Use full-width forms for Chinese punctuation and half-width forms for English punctuation.
```

## License

This repository is licensed under the MIT License. See [LICENSE](LICENSE) for details.