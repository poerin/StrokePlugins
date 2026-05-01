# Stroke.Tip

一款为 [Stroke](https://github.com/poerin/Stroke) 鼠标手势引擎设计的屏幕提示插件。提示文字会显示在鼠标所在屏幕的底部居中位置。

## 目录

- [功能特性](#功能特性)
- [运行要求](#运行要求)
- [使用方法](#使用方法)
    - [手势脚本](#手势脚本)
- [许可证](#许可证)

## 功能特性

- **多屏感知定位**：提示将跟随鼠标光标所在的屏幕，在多显示器环境下天然适用。
- **外观全面可定制**：
    - 文字颜色（默认为黑色）。
    - 背景颜色（省略时根据文字亮度自动适配高对比度背景；可指定具体纯色或 `Color.Transparent` 实现无背景）。
    - 字体名称、字号与样式（默认为微软雅黑 26 磅粗体）。
    - 显示时长，以毫秒为单位（默认 500 毫秒）。
- **镂空文字支持**：将文字颜色设为 `Color.Transparent` 并搭配非透明背景，即可呈现文字区域透出桌面背景的镂空效果。
- **多重重载方法**：支持直接传入 `Font` 对象或分别指定字体参数，使用灵活。

## 运行要求

- Stroke 引擎。
- .NET Framework 4.8 运行时。

## 使用方法

### 手势脚本

插件提供静态方法 `Tip.ShowTipText`，共有三种重载形式：

```csharp
public static void ShowTipText(string text, int duration = 500, Color? color = null, Color? backColor = null);
public static void ShowTipText(string text, int duration, Color color, Color? backColor, Font font);
public static void ShowTipText(string text, int duration, Color color, Color? backColor, string fontFamily, float fontSize = 26f, FontStyle fontStyle = FontStyle.Bold);
```

```csharp
// 使用全部默认参数。
Tip.ShowTipText("文本样例：全部默认");

// 自定义持续时长与文字颜色。
Tip.ShowTipText("文本样例：白色文字，2 秒", 2000, Color.White);

// 指定纯色背景。
Tip.ShowTipText("文本样例：白色文字，深蓝背景", 1500, Color.White, Color.DarkBlue);

// 完全透明背景，仅显示文字。
Tip.ShowTipText("文本样例：透明背景，绿色文字", 1000, Color.Lime, Color.Transparent);

// 镂空文字效果：绿色背景，文字透明透出桌面。
Tip.ShowTipText("文本样例：镂空效果，绿色背景", 2000, Color.Transparent, Color.Lime);

// 通过 Font 对象指定字体。
Tip.ShowTipText("文本样例：楷体常规字体", 800, Color.Black, null, new Font("楷体", 20f, FontStyle.Regular));

// 分别指定字体名称、大小与样式。
Tip.ShowTipText("文本样例：Courier New 粗体", 1200, Color.Orange, null, "Courier New", 18f, FontStyle.Bold);

// 同时自定义颜色、背景与字体。
Tip.ShowTipText("文本样例：青色文字、深灰色背景、Consolas 22 磅斜体", 2000, Color.Cyan, Color.FromArgb(40, 40, 40), "Consolas", 22f, FontStyle.Italic);
```

## 许可证

基于 MIT 许可证开源。