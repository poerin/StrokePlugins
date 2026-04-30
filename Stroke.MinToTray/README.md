 ## 支持将多个窗口分别最小化到系统托盘，每个图标可独立恢复

 `MinToTray.MinimizeToTray(System.IntPtr,System.String,System.Drawing.Icon)`
    
            最小化指定窗口到系统托盘。
            <param name="hWnd">目标窗口句柄，默认使用当前活动窗口</param>
            <param name="tipText">托盘图标提示文本（可选）</param>
            <param name="customIcon">自定义图标（可选）</param>
示例:
    `MinToTray.MinimizeToTray();`
