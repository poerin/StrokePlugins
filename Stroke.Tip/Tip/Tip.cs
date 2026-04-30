using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Threading;
using System.Windows.Forms;

namespace Stroke
{
    public static class Tip
    {
        private static readonly List<TipForm> _activeTips = new List<TipForm>();
        private static readonly object _lock = new object();

        public static void ShowTipText(string text, Color color, float fontSize = 26f, int durationMs = 500)
        {
            if (Application.MessageLoop)
            {
                ShowTip(text, color, fontSize, durationMs);
            }
            else
            {
                SynchronizationContext syncContext = SynchronizationContext.Current;
                if (syncContext == null)
                {
                    Thread thread = new Thread(() =>
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

        internal static void RegisterTip(TipForm form)
        {
            lock (_lock)
            {
                _activeTips.Add(form);
            }
            RepositionTips();
        }

        internal static void UnregisterTip(TipForm form)
        {
            lock (_lock)
            {
                _activeTips.Remove(form);
            }
            RepositionTips();
        }

        private static void RepositionTips()
        {
            List<TipForm> tips;
            lock (_lock)
            {
                tips = new List<TipForm>(_activeTips);
            }
            Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;
            int baseY = workingArea.Bottom - 100;
            int spacing = 4;
            int currentY = baseY;
            for (int i = tips.Count - 1; i >= 0; i--)
            {
                TipForm tipForm = tips[i];
                currentY -= tipForm.Height + spacing;
                tipForm.Location = new Point(
                    (workingArea.Width - tipForm.Width) / 2,
                    currentY);
            }
        }

        private static void ShowTip(string text, Color color, float fontSize, int durationMs)
        {
            TipForm form = new TipForm(text, color, fontSize, durationMs);
            RegisterTip(form);
            form.Show();
            form.Activate();
            form.BringToFront();
        }
    }

    internal class TipForm : Form
    {
        private System.Windows.Forms.Timer _closeTimer;
        private Font _tipFont;

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
            _tipFont = new Font("微软雅黑", fontSize, FontStyle.Bold);
            Font = _tipFont;
            ForeColor = textColor;

            Size textSize = MeasureTextAccurate(text, _tipFont);
            Padding = new Padding(16, 8, 16, 8);
            Size = new Size(
                textSize.Width + Padding.Horizontal,
                textSize.Height + Padding.Vertical);

            _closeTimer = new System.Windows.Forms.Timer { Interval = durationMs };
            _closeTimer.Tick += (s, e) => { _closeTimer.Stop(); Close(); };
            _closeTimer.Start();

            FormClosed += (s, e) =>
            {
                _closeTimer.Dispose();
                _tipFont.Dispose();
                Tip.UnregisterTip(this);
            };
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics graphics = e.Graphics;
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Rectangle rect = new Rectangle(
                Padding.Left,
                Padding.Top,
                ClientSize.Width - Padding.Horizontal,
                ClientSize.Height - Padding.Vertical);

            int average = (ForeColor.R + ForeColor.G + ForeColor.B) / 3;
            Color backColor = average < 128 ? Color.FromArgb(248, 248, 248) : Color.FromArgb(8, 8, 8);

            using (SolidBrush backBrush = new SolidBrush(backColor))
            {
                graphics.FillRectangle(backBrush, rect);
            }

            TextRenderer.DrawText(graphics, Text, _tipFont, rect, ForeColor,
                TextFormatFlags.HorizontalCenter |
                TextFormatFlags.VerticalCenter |
                TextFormatFlags.SingleLine |
                TextFormatFlags.NoPadding);
        }

        private static Size MeasureTextAccurate(string text, Font font)
        {
            return TextRenderer.MeasureText(text, font, Size.Empty,
                TextFormatFlags.SingleLine | TextFormatFlags.NoPadding);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams createParams = base.CreateParams;
                createParams.ExStyle |= 0x20;
                return createParams;
            }
        }
    }
}