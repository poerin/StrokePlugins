# Stroke.MinToTray

为 [Stroke](https://github.com/poerin/Stroke) 鼠标手势引擎提供缩小窗口到系统托盘功能的插件。它在通知区域显示该软件图标和窗口文字，点击图标可恢复显示。

## 目录

- [功能特性](#功能特性)
- [运行要求](#运行要求)
- [使用方法](#使用方法)
    - [手势脚本](#手势脚本)
- [许可证](#许可证)

## 功能特性

- **缩小到托盘**：将当前窗口缩小到系统托盘，并显示其图标。
- **恢复**：左键单击图标即可恢复显示窗口。

## 运行要求

- Stroke 引擎。
- .NET Framework 4.8 运行时。

## 使用方法

### 手势脚本

插件提供一个公开方法 `MinimizeToTray(IntPtr hWnd = default, string tipText = null, Icon customIcon = null)`，用于缩小窗口到托盘图标。

以下为示例代码。

```csharp
// 缩小当前窗口到托盘。
MinToTray.MinimizeToTray();
```

## 许可证

基于 MIT 许可证开源。