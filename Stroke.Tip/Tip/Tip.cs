using System;
using System.Drawing;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Stroke
{
    public static class Tip
    {
		public static void ShowTipText(string text, Color color, float fontSize = 26f, int durationMs = 500)
        {
            if (Application.MessageLoop)
            {
                ShowTip(text, color, fontSize, durationMs);
            }
            else
            {
                var syncContext = SynchronizationContext.Current;
                if (syncContext == null)
                {
                    var thread = new Thread(() =>
                    {
                        Application.Run(new TipForm(text, color, fontSize, durationMs));
                    });
                    thread.SetApartmentState(ApartmentState.STA);
                    thread.Start();
                }
                else
                {
                    syncContext.Post(_ => ShowTip(text, color, fontSize, durationMs), null);
                }
            }
        }

        private static void ShowTip(string text, Color color, float fontSize, int durationMs)
        {
            var form = new TipForm(text, color, fontSize, durationMs);
            form.Show();
            form.Activate();
            form.BringToFront();
        }
    }

    internal class TipForm : Form
    {
        private System.Windows.Forms.Timer _closeTimer;

        public TipForm(string text, Color textColor, float fontSize, int durationMs)
        {
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            TopMost = true;
            StartPosition = FormStartPosition.Manual;
            AllowTransparency = true;
            BackColor = Color.Black;
            TransparencyKey = Color.Black;
            Opacity = 1.0;

            Text = text;
            Font = new Font("微软雅黑", fontSize, FontStyle.Bold);
            ForeColor = textColor;

            Size textSize = MeasureTextAccurate(text, Font);
            Padding = new Padding(16, 8, 16, 8);
            Size = new Size(
                textSize.Width + Padding.Horizontal,
                textSize.Height + Padding.Vertical);

            var screen = Screen.PrimaryScreen.WorkingArea;
            Location = new Point(
                (screen.Width - Width) / 2,
                screen.Bottom - Height - 100);

            _closeTimer = new System.Windows.Forms.Timer { Interval = durationMs };
            _closeTimer.Tick += (s, e) => { _closeTimer.Stop(); Close(); };
            _closeTimer.Start();

            FormClosed += (s, e) => _closeTimer.Dispose();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            var rect = new Rectangle(
                Padding.Left,
                Padding.Top,
                ClientSize.Width - Padding.Horizontal,
                ClientSize.Height - Padding.Vertical);

            TextRenderer.DrawText(g, Text, Font, rect, ForeColor,
                TextFormatFlags.HorizontalCenter |
                TextFormatFlags.VerticalCenter |
                TextFormatFlags.SingleLine |
                TextFormatFlags.NoPadding);
        }

        private static Size MeasureTextAccurate(string text, Font font)
        {
            using (var bitmap = new Bitmap(1, 1))
            using (var g = Graphics.FromImage(bitmap))
            {
                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                var format = StringFormat.GenericTypographic;
                format.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;

                SizeF sizeF = g.MeasureString(text, font, PointF.Empty, format);

                int width = (int)Math.Ceiling(sizeF.Width * 1.05);
                int height = (int)Math.Ceiling(sizeF.Height * 1.05);

                return new Size(width, height);
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x20; // WS_EX_TRANSPARENT
                return cp;
            }
        }
    }
}