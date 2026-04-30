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

        private const int SW_HIDE = 0;
        private const int SW_RESTORE = 9;
        private const uint WM_GETICON = 0x007F;
        private const int ICON_BIG = 1;
        private const int ICON_SMALL = 0;
        private const int GCL_HICON = -14;
        private const int GCL_HICONSM = -34;

        private static readonly List<TrayInfo> _trayItems = new List<TrayInfo>();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWindow(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetClassName(IntPtr hWnd, System.Text.StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll", EntryPoint = "GetClassLong", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern uint GetClassLong32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetClassLongPtr", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetClassLongPtr64(IntPtr hWnd, int nIndex);

        private static IntPtr GetClassLongPtr(IntPtr hWnd, int nIndex)
        {
            if (IntPtr.Size == 8)
                return GetClassLongPtr64(hWnd, nIndex);
            else
                return new IntPtr(GetClassLong32(hWnd, nIndex));
        }

        public static void MinimizeToTray(IntPtr hWnd = default, string tipText = null, Icon customIcon = null)
        {
            if (hWnd == default)
                hWnd = GetCurrentWindow();

            if (!IsWindow(hWnd))
                return;

            if (IsShellWindow(hWnd))
                return;

            if (_trayItems.Exists(t => t.WindowHandle == hWnd))
                return;

            Icon icon = customIcon ?? GetWindowIcon(hWnd) ?? GetDefaultIcon();
            string title = tipText ?? GetWindowTitle(hWnd) ?? "Minimized Window";

            NotifyIcon notifyIcon = new NotifyIcon
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
            TrayInfo last = _trayItems[_trayItems.Count - 1];
            RestoreWindow(last.WindowHandle);
        }

        public static void RestoreWindow(IntPtr hWnd)
        {
            TrayInfo item = _trayItems.Find(t => t.WindowHandle == hWnd);
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
            foreach (TrayInfo item in _trayItems.ToArray())
            {
                item.Icon.Visible = false;
                item.Icon.Dispose();
            }
            _trayItems.Clear();
        }

        public static bool IsMinimized
        {
            get { return _trayItems.Count > 0; }
        }

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

        private static bool IsShellWindow(IntPtr hWnd)
        {
            System.Text.StringBuilder className = new System.Text.StringBuilder(256);
            GetClassName(hWnd, className, className.Capacity + 1);
            string name = className.ToString();
            return name == "Progman" || name == "WorkerW" || name == "Shell_TrayWnd";
        }

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
                catch { }
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
            const int nChars = 63;
            System.Text.StringBuilder buffer = new System.Text.StringBuilder(nChars);
            if (GetWindowText(hWnd, buffer, nChars) > 0)
                return buffer.ToString();
            return null;
        }
    }
}