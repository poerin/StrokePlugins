using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Stroke
{
    /// <summary>
    /// 支持将多个窗口分别最小化到系统托盘，每个图标可独立恢复。
    /// </summary>
    public static class MinToTray
    {
        // 存储所有被最小化到托盘的窗口信息
        private class TrayInfo
        {
            public IntPtr WindowHandle { get; set; }
            public NotifyIcon Icon { get; set; }
        }

        // 当前所有托盘子项的列表
        private static readonly List<TrayInfo> _trayItems = new List<TrayInfo>();

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool IsWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr GetClassLongPtr(IntPtr hWnd, int nIndex);

        private const int SW_HIDE = 0;
        private const int SW_RESTORE = 9;
        private const uint WM_GETICON = 0x007F;
        private const int ICON_BIG = 1;
        private const int ICON_SMALL = 0;
        private const int GCL_HICON = -14;
        private const int GCL_HICONSM = -34;

        /// <summary>
        /// 最小化指定窗口到系统托盘。
        /// 如果该窗口已在托盘中，则不做任何操作（可避免重复）。
        /// </summary>
        /// <param name="hWnd">目标窗口句柄，为 default 时使用当前活动窗口</param>
        /// <param name="tipText">托盘图标提示文本（可选）</param>
        /// <param name="customIcon">自定义图标（可选）</param>
        public static void MinimizeToTray(IntPtr hWnd = default, string tipText = null, Icon customIcon = null)
        {
            if (hWnd == default)
                hWnd = GetCurrentWindow(); // 假设 Stroke.CurrentWindow 可用，此处封装一下

            if (!IsWindow(hWnd))
                throw new ArgumentException("无效的窗口句柄", nameof(hWnd));

            // 如果该窗口已经在托盘中，不重复添加
            if (_trayItems.Exists(t => t.WindowHandle == hWnd))
                return;

            Icon icon = customIcon ?? GetWindowIcon(hWnd) ?? GetDefaultIcon();
            string title = tipText ?? GetWindowTitle(hWnd) ?? "Minimized Window";

            var notifyIcon = new NotifyIcon
            {
                Icon = icon,
                Text = title,
                Visible = true
            };

            // 将窗口句柄暂存在 Tag 中，以便在事件回调中识别
            notifyIcon.Tag = hWnd;

            notifyIcon.MouseClick += (sender, e) =>
            {
                if (e.Button == MouseButtons.Left && sender is NotifyIcon iconSender)
                {
                    IntPtr targetHwnd = (IntPtr)iconSender.Tag;
                    RestoreWindow(targetHwnd);
                }
            };

            // 隐藏窗口
            ShowWindow(hWnd, SW_HIDE);

            // 记录到列表
            _trayItems.Add(new TrayInfo { WindowHandle = hWnd, Icon = notifyIcon });
        }

        /// <summary>
        /// 恢复最近一个被最小化到托盘的窗口（后进先出）。
        /// 如果想恢复特定窗口，请使用 RestoreWindow(IntPtr hWnd)。
        /// </summary>
        public static void RestoreFromTray()
        {
            if (_trayItems.Count == 0) return;
            var last = _trayItems[_trayItems.Count - 1];
            RestoreWindow(last.WindowHandle);
        }

        /// <summary>
        /// 恢复指定窗口（通过窗口句柄）。
        /// </summary>
        public static void RestoreWindow(IntPtr hWnd)
        {
            var item = _trayItems.Find(t => t.WindowHandle == hWnd);
            if (item == null) return;

            if (IsWindow(hWnd))
            {
                ShowWindow(hWnd, SW_RESTORE);
                SetForegroundWindow(hWnd);
            }

            // 移除并销毁托盘图标
            item.Icon.Visible = false;
            item.Icon.Dispose();
            _trayItems.Remove(item);
        }

        /// <summary>
        /// 强制移除所有托盘图标（不恢复任何窗口），用于程序退出时清理。
        /// </summary>
        public static void DisposeTray()
        {
            foreach (var item in _trayItems.ToArray())
            {
                item.Icon.Visible = false;
                item.Icon.Dispose();
            }
            _trayItems.Clear();
        }

        /// <summary>
        /// 当前是否有任何窗口被最小化到托盘。
        /// </summary>
        public static bool IsMinimized => _trayItems.Count > 0;

        /// <summary>
        /// 获取当前活动窗口句柄（适配原 Stroke.CurrentWindow）。
        /// </summary>
        private static IntPtr GetCurrentWindow()
        {
            // 假设外部存在 Stroke.CurrentWindow 静态属性
            try
            {
                return Stroke.CurrentWindow;
            }
            catch
            {
                // 若引用缺失，回退为 GetForegroundWindow 系统 API
                return GetForegroundWindow();
            }
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        // 尝试从窗口获取图标
        private static Icon GetWindowIcon(IntPtr hWnd)
        {
            IntPtr hIcon = SendMessage(hWnd, WM_GETICON, new IntPtr(ICON_BIG), IntPtr.Zero);
            if (hIcon == IntPtr.Zero)
                hIcon = SendMessage(hWnd, WM_GETICON, new IntPtr(ICON_SMALL), IntPtr.Zero);
            if (hIcon == IntPtr.Zero)
                hIcon = GetClassLongPtr(hWnd, GCL_HICON);
            if (hIcon == IntPtr.Zero)
                hIcon = GetClassLongPtr(hWnd, GCL_HICONSM);
            if (hIcon != IntPtr.Zero)
            {
                try { return Icon.FromHandle(hIcon); }
                catch { /* 无效句柄忽略 */ }
            }
            return null;
        }

        // 获取当前程序默认图标
        private static Icon GetDefaultIcon()
        {
            try
            {
                return Icon.ExtractAssociatedIcon(Assembly.GetEntryAssembly()?.Location) ?? SystemIcons.Application;
            }
            catch
            {
                return SystemIcons.Application;
            }
        }

        // 获取窗口标题
        private static string GetWindowTitle(IntPtr hWnd)
        {
            const int nChars = 256;
            var buff = new System.Text.StringBuilder(nChars);
            if (GetWindowText(hWnd, buff, nChars) > 0)
                return buff.ToString();
            return null;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder lpString, int nMaxCount);
    }
}