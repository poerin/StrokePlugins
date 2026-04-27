using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;


namespace Stroke
{
    class ClipboardListener : Form
    {
        private const int WM_CLIPBOARDUPDATE = 0x031D;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool AddClipboardFormatListener(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool RemoveClipboardFormatListener(IntPtr hWnd);

        internal bool Listening = false;
        internal event Action Do;
        internal DateTime previousTime = DateTime.MinValue;

        internal ClipboardListener()
        {
            AddClipboardFormatListener(this.Handle);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            RemoveClipboardFormatListener(this.Handle);
            base.OnClosing(e);
        }

        protected override void WndProc(ref Message m)
        {
            DateTime currentTime = DateTime.Now;
            if ((currentTime - previousTime).TotalMilliseconds > 200 && m.Msg == WM_CLIPBOARDUPDATE && Listening)
            {
                previousTime = currentTime;
                Do();
            }
            base.WndProc(ref m);
        }

    }
}
