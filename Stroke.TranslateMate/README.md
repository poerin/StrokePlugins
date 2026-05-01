# Stroke.TranslateMate

A clipboard-driven translation plugin for [Stroke](https://github.com/poerin/Stroke). It automatically detects Chinese or English text, queries the configured translation API, and displays the result in a draggable overlay. The plugin also provides pronunciation via Youdao Dictionary and maintains a vocabulary notebook.

## Table of Contents

- [Features](#features)
- [Requirements](#requirements)
- [API Credentials](#api-credentials)
    - [Baidu Translation API](#baidu-translation-api)
    - [Tencent Cloud Machine Translation API](#tencent-cloud-machine-translation-api)
- [Usage](#usage)
    - [Gesture Scripts](#gesture-scripts)
    - [Pronunciation Cache](#pronunciation-cache)
    - [Vocabulary Notebook](#vocabulary-notebook)
- [License](#license)

## Features

- **Bi‑directional Translation**: Automatically detects the language of the source text and translates between English and Chinese.
- **Multi‑provider Support**: Switch between Baidu Translation and Tencent Cloud Machine Translation by setting the `Provider` property.
- **Youdao Dictionary Pronunciation**: When a single English word is selected, its audio is played from the Youdao Dictionary interface (`dict.youdao.com`). Audio files are cached locally.
- **Vocabulary Notebook**: Each single English word translated is automatically recorded into a `Glossary.csv` file, sorted alphabetically.
- **Draggable Overlay**: The translation appears in a semi‑transparent floating window near the mouse cursor. The window can be dragged, closed with a right‑click, and its content can be double‑clicked to copy to the clipboard.

## Requirements

- Stroke engine.
- .NET Framework 4.8 runtime.
- A valid account for at least one translation provider (see below).

## API Credentials

### Baidu Translation API

- **Obtain credentials**: Register or log in at [https://api.fanyi.baidu.com](https://api.fanyi.baidu.com), then retrieve your **APP ID** and **Secret Key** from the management console. Select the **General Text Translation** service.
- **Pricing**: A free tier is available. For detailed quota and pricing, refer to the [product page](https://api.fanyi.baidu.com/product/112).

### Tencent Cloud Machine Translation API

- **Obtain credentials**: Log in to the [Tencent Cloud Console](https://console.cloud.tencent.com) to create or view your **SecretId** and **SecretKey**. Then enable the [Machine Translation](https://console.cloud.tencent.com/tmt) service.
- **Region**: A region parameter is required (default `ap-guangzhou`). A full list of available regions can be found in the [API documentation](https://cloud.tencent.com/document/api/551/15615).
- **Pricing**: A free tier is available. For detailed quota and pricing, refer to the [billing overview](https://cloud.tencent.com/document/product/551/35017).

## Usage

### Gesture Scripts

The plugin provides two public methods:

- `TranslateMate.BeginClipboardDisplay()` – display clipboard text without translating.
- `TranslateMate.BeginClipboardTranslation()` – translate the clipboard text.

You must set the `Provider` to choose a translation backend, along with the corresponding credentials. **Before executing the gesture, first select the text you want to translate.** The script will simulate `Ctrl+C` to copy the selection to the clipboard; the plugin detects the new clipboard content and performs the translation automatically.

**Example using Baidu:**

```csharp
TranslateMate.Provider = "Baidu";
TranslateMate.SecretId = "your_app_id";
TranslateMate.SecretKey = "your_secret_key";
TranslateMate.BeginClipboardTranslation();
Base.Activate();
Base.PressKeys("(c)");
```

**Example using Tencent Cloud:**

```csharp
TranslateMate.Provider = "Tencent";
TranslateMate.SecretId = "your_secret_id";
TranslateMate.SecretKey = "your_secret_key";
TranslateMate.Region = "ap-guangzhou";   // optional, defaults to ap-guangzhou
TranslateMate.BeginClipboardTranslation();
Base.Activate();
Base.PressKeys("(c)");
```

For backward compatibility, `AppId` can still be used instead of `SecretId`.

The translation result is displayed in a floating window near the mouse cursor. The window can be dragged to any position, closed by a right‑click, and its text copied by double‑clicking the content.

### Pronunciation Cache

Audio files retrieved from Youdao Dictionary are stored in a subfolder named `Audio`, located in the same directory as the plugin DLL. Cached files are reused to avoid repeated downloads.

### Vocabulary Notebook

When an English word is translated, it is automatically added to `Glossary.csv` in the plugin’s directory. The file is ordered alphabetically and updated periodically.

## License

Distributed under the MIT License.