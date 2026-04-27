# Stroke.PaddleOcrMate

A clipboard-driven OCR plugin for [Stroke](https://github.com/poerin/Stroke) that uses the PaddleOCR API. It extracts text from images in the clipboard or from copied image file paths, and displays the result in a resizable window.

## Table of Contents

- [Features](#features)
- [Requirements](#requirements)
- [API Credentials](#api-credentials)
    - [PaddleOCR API](#paddleocr-api)
- [Usage](#usage)
    - [Gesture Scripts](#gesture-scripts)
- [Model Advice](#model-advice)
- [License](#license)

## Features

- **Clipboard Image Recognition**: Recognizes text from images that have been copied to the clipboard (via any means) or from image files whose paths have been copied.
- **Text Display**: The recognized text appears in a standard, resizable window containing a multiline text box that supports scrolling, editing, and copying.

## Requirements

- Stroke engine.
- .NET Framework 4.8 runtime.
- A PaddleOCR API token from Baidu AI Studio.

## API Credentials

### PaddleOCR API

- **Obtain API URL and Token**: Go to [https://aistudio.baidu.com/paddleocr](https://aistudio.baidu.com/paddleocr) and locate the **API call example** on the page. Copy the `API_URL` and `TOKEN` displayed there. These credentials are bound to your Baidu account.
- **Free Usage**: The API provides daily free quotas suitable for personal use.

## Usage

### Gesture Scripts

Set the `ApiUrl` and `Token` properties, then call `BeginClipboardOcr()`. After the script runs, copy an image or an image file path, and the plugin will detect the new clipboard content and perform OCR automatically.

```csharp
PaddleOcrMate.ApiUrl = "https://your-paddleocr-endpoint";
PaddleOcrMate.Token = "your_access_token";
PaddleOcrMate.BeginClipboardOcr();
```

The recognition result will open in a window near the mouse cursor.

## Model Advice

PaddleOCR offers multiple models. If you encounter instability with one model (for example, excessive HTML placeholders or unsatisfactory output), switch to another model on the PaddleOCR website. After switching, obtain a new `API_URL` and `Token` and update them in your script.

## License

Distributed under the MIT License.