using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Stroke
{
    public static class MinToTray
    {
        private class TrayInfo
        {
            public IntPtr WindowHandle { get; set; }
            public NotifyIcon Icon { get; set; }
        }
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

        public static void MinimizeToTray(IntPtr hWnd = default, string tipText = null, Icon customIcon = null)
        {
            if (hWnd == default)
                hWnd = GetCurrentWindow(); 

            if (!IsWindow(hWnd))
                throw new ArgumentException("无效的窗口句柄", nameof(hWnd));

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

            notifyIcon.Tag = hWnd; 
            notifyIcon.MouseClick += (sender, e) =>
            {
                if (e.Button == MouseButtons.Left && sender is NotifyIcon iconSender)
                {
                    IntPtr targetHwnd = (IntPtr)iconSender.Tag;
                    RestoreWindow(targetHwnd);
                }
            };
            ShowWindow(hWnd, SW_HIDE);
            _trayItems.Add(new TrayInfo { WindowHandle = hWnd, Icon = notifyIcon });
        }

        public static void RestoreFromTray()
        {
            if (_trayItems.Count == 0) return;
            var last = _trayItems[_trayItems.Count - 1];
            RestoreWindow(last.WindowHandle);
        }

        public static void RestoreWindow(IntPtr hWnd)
        {
            var item = _trayItems.Find(t => t.WindowHandle == hWnd);
            if (item == null) return;

            if (IsWindow(hWnd))
            {
                ShowWindow(hWnd, SW_RESTORE);
                SetForegroundWindow(hWnd);
            }
            item.Icon.Visible = false;
            item.Icon.Dispose();
            _trayItems.Remove(item);
        }

        public static void DisposeTray()
        {
            foreach (var item in _trayItems.ToArray())
            {
                item.Icon.Visible = false;
                item.Icon.Dispose();
            }
            _trayItems.Clear();
        }

        public static bool IsMinimized => _trayItems.Count > 0;
        private static IntPtr GetCurrentWindow()
        {
            try
            {
                return Stroke.CurrentWindow;
            }
            catch
            {
                return GetForegroundWindow();
            }
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
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

        private static string GetWindowTitle(IntPtr hWnd)
        {
            const int nChars = 63;  //System.ArgumentOutOfRangeException: 文本长度必须少于 64 个字符。
            var buff = new System.Text.StringBuilder(nChars);
            if (GetWindowText(hWnd, buff, nChars) > 0)
                return buff.ToString();
            return null;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder lpString, int nMaxCount);
    }
}