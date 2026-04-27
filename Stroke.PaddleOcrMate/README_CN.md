# Stroke.PaddleOcrMate

一个剪贴板驱动的 OCR 文字识别插件，适用于 [Stroke](https://github.com/poerin/Stroke) 鼠标手势引擎。它通过 PaddleOCR API 识别剪贴板中的图片或通过文件路径复制的图片文件，并将结果显示在可调节大小的窗口中。

## 目录

- [功能特性](#功能特性)
- [运行要求](#运行要求)
- [API 凭证](#api-凭证)
    - [PaddleOCR API](#paddleocr-api)
- [使用方法](#使用方法)
    - [手势脚本](#手势脚本)
- [模型建议](#模型建议)
- [许可证](#许可证)

## 功能特性

- **剪贴板图片识别**：识别已复制到剪贴板的图片或通过文件路径复制的图片文件中的文字。
- **文本显示**：识别结果以标准窗口呈现，窗口内带有多行文本框，支持滚动、编辑和复制。

## 运行要求

- Stroke 引擎。
- .NET Framework 4.8 运行时。
- 百度 AI Studio 的 PaddleOCR API Token。

## API 凭证

### PaddleOCR API

- **获取 API URL 与 Token**：访问 [https://aistudio.baidu.com/paddleocr](https://aistudio.baidu.com/paddleocr)，在页面中的 **API 调用示例** 部分找到 `API_URL` 与 `TOKEN`。这些凭证与您的百度帐号绑定。
- **免费使用**：该 API 提供每日免费额度，适合个人使用。

## 使用方法

### 手势脚本

设置 `ApiUrl` 和 `Token` 属性后调用 `BeginClipboardOcr()`。脚本执行后，复制一张图片或一个图片文件路径，插件将自动检测剪贴板内容并进行 OCR 识别。

```csharp
PaddleOcrMate.ApiUrl = "https://your-paddleocr-endpoint";
PaddleOcrMate.Token = "your_access_token";
PaddleOcrMate.BeginClipboardOcr();
```

识别结果将显示在鼠标光标附近的窗口中。

## 模型建议

PaddleOCR 提供多种模型。如果某个模型出现不稳定或返回结果不理想，建议在 PaddleOCR 网站切换到其他模型。切换后获取新的 `API_URL` 和 `Token`，并更新到脚本中即可。

## 许可证

基于 MIT 许可证开源。