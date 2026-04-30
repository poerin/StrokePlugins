# StrokePlugins

**[Stroke](https://github.com/poerin/Stroke) 鼠标手势引擎的插件集合。**

本仓库包含一组扩展 Stroke 功能的插件。每个插件均以独立的 .NET 类库（DLL）形式提供，可直接放入 Stroke 工作目录，通过手势调用。

![License](https://img.shields.io/badge/license-MIT-blue.svg)
![Platform](https://img.shields.io/badge/platform-Windows-lightgrey)

[View English Documentation](README.md)

## 目录

- [可用插件](#可用插件)
- [安装与使用](#安装与使用)
- [开发自己的插件](#开发自己的插件)
- [参与贡献](#参与贡献)
- [许可证](#许可证)

## 可用插件

| 插件 | 描述 |
|---|---|
| [Stroke.TrayIcon](Stroke.TrayIcon) | 提供系统托盘图标，支持暂停 / 恢复手势与退出程序控制。 |
| [Stroke.TranslateMate](Stroke.TranslateMate) | 基于剪贴板的中英文互译插件，使用百度翻译 API，并集成有道词典发音与生词本。 |
| [Stroke.PaddleOcrMate](Stroke.PaddleOcrMate) | 基于 PaddleOCR 的图片文字识别，通过剪贴板图像或文件路径触发。 |
| [Stroke.Tip](Stroke.Tip) | 在屏幕底部渲染可定制的提示文本，支持调整字体颜色、大小及显示时长。 |
| [Stroke.MinToTray](Stroke.MinToTray) | 将活动窗口最小化至系统托盘，并在通知区域显示对应图标。 |

欢迎贡献新的插件。

## 安装与使用

1. 从本仓库下载所需插件的发布版本，或自行编译源代码。
2. 将生成的 `.dll` 文件（如 `Stroke.TranslateMate.dll`）放入 `Stroke.exe` 所在目录。
3. 重新启动 Stroke。
4. 在 Stroke 配置工具中为手势编写脚本，调用插件的公开方法。

每个插件通过 `Stroke` 命名空间下的静态类公开功能，您可以在 Stroke 动作脚本中直接调用其方法。有关 API 凭证及示例脚本，请参阅各插件的 README 文档。

## 开发自己的插件

创建新插件的基本步骤：
1. 新建一个面向 **.NET Framework 4.8** 的类库项目。
2. 使用 `Stroke` 命名空间创建公开的静态类。
3. 实现业务逻辑并暴露 `static` 方法供 Stroke 脚本调用。
4. 生成的 DLL 按照 `Stroke.<PluginName>.dll` 格式命名。

如果插件需要引用 Stroke 引擎中的类型，可将 Stroke 项目添加为引用。推荐在开发环境中引入 Stroke 源代码作为项目引用，以确保插件始终基于最新 Stroke 源码构建。

## 参与贡献

欢迎提供新的插件、改进或修复缺陷。请通过 Issue 或 Pull Request 提交。

## 许可证

本仓库基于 MIT 许可证开源。详情见 [LICENSE](LICENSE) 文件。
