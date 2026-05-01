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

**推荐使用自带的脚手架工具快速生成项目结构。**

1. 运行 `StrokePluginTools` 目录下的 `StrokePluginScaffold.exe`，根据提示完成配置，将自动生成包含正确命名空间与引用关系的解决方案。
2. 在生成的 `Stroke.<PluginName>` 目录下即可打开 `.sln` 文件开始开发。

此外，你也可以使用批量编译工具 `StrokePluginBatchBuild.exe`（同样位于 `StrokePluginTools` 目录）一次性编译所有插件及主引擎。该工具会自动查找 MSBuild，以 Release 配置构建所有解决方案，并将生成的 `.dll` 和 `.exe` 文件统一收集到仓库根目录的 `Release` 文件夹中。

**或者手动创建项目：**

1. 新建一个面向 **.NET Framework 4.8** 的类库项目。
2. 使用 `Stroke` 命名空间创建公开的静态类。
3. 实现业务逻辑并暴露 `static` 方法供 Stroke 脚本调用。
4. 生成的 DLL 按照 `Stroke.<PluginName>.dll` 格式命名。

如果插件需要引用 Stroke 引擎中的类型，可将 Stroke 项目添加为引用。推荐在开发环境中引入 Stroke 源代码作为项目引用，以确保插件始终基于最新 Stroke 源码构建（脚手架工具在生成时可选择是否包含此引用）。

## 参与贡献

欢迎提供新的插件、改进或修复缺陷。请通过 Issue 或 Pull Request 提交。

**Pull Request 规范：**
- 合并前请将所有 commit 压缩（Squash）为一条。最终 commit message 格式建议为：`YYYY-MM-DD-X`（当天日期 + 当天提交序号，例如 `2026-04-30-1`），以便保持历史记录整洁清晰。

**代码风格：**
- 注重代码的自解释性，倾向于用清晰的变量名和逻辑替代注释。请尽可能删除不必要的说明性注释，保留代码本身的简洁和可读性。
- 你可以借助 AI 助手来统一标识符命名和文本风格，推荐使用以下提示词：

```
# 代码标识符与文本内容优化

请协助我完成代码优化，具体要求如下：

## 优化范围

- 代码标识符：包括变量名、方法名、类名、函数名等所有代码标识符。
- 文本内容：涵盖代码中所有面向用户的文本信息，包括异常提示、日志消息、打印输出及注释说明等。

## 命名规范

- 使用完整单词表达含义，避免使用生僻或自定义缩写。
- 仅允许使用被广泛接受的缩写（如 `id`、`url`、`max`、`min` 等）。
- 确保名称简洁且准确反映其数据、功能或逻辑含义。
- 遵循对应编程语言的命名约定。

## 修改限制

- 仅修改名称和文本内容，不改变代码结构、逻辑或功能。
- 如非必要，不要添加注释。请尽可能删除现有注释，让代码通过清晰的命名自解释。

## 输出格式

- 将优化后的代码置于标准代码块中展示。
- 在中英文混排时，中文与英文单词之间必须确保存在一个标准空格。
- 中文标点符号使用全角形式，英文标点符号使用半角形式。
```

## 许可证

本仓库基于 MIT 许可证开源。详情见 [LICENSE](LICENSE) 文件。