# Stroke.Tip

为 [Stroke](https://github.com/poerin/Stroke) 鼠标手势引擎提供屏幕提示文字的插件。它在主屏幕底部显示提示文字。

## 目录

- [功能特性](#功能特性)
- [运行要求](#运行要求)
- [使用方法](#使用方法)
    - [手势脚本](#手势脚本)
- [许可证](#许可证)

## 功能特性

- 文字提示：在主屏幕底部显示提示文字。
- 自定义：通过参数实现对提示文字颜色/大小/显示时长的自定义。


## 运行要求

- Stroke 引擎。
- .NET Framework 4.8 运行时。

## 使用方法

### 手势脚本

插件仅提供一个公开方法 `ShowTipText(string text, Color color, float fontSize = 26f, int durationMs = 500)`，用于显示提示文字。

以下为示例代码。

```csharp
         // 显示白色提示文字，大小为26, 持续 1 秒
        Tip.ShowTipText("保存成功", Color.White, 26f, 1000);
        
        // 显示绿色提示，使用默认大小和时长
        Tip.ShowTipText("手势识别中...", Color.Lime);
```

## 许可证

基于 MIT 许可证开源。