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

        public static void ShowTipText(string text, int duration = 500, Color? color = null, Color? backColor = null)
        {
            Color resolvedColor = color ?? Color.Black;
            Font defaultFont = new Font("微软雅黑", 26f, FontStyle.Bold);
            ShowTipText(text, duration, resolvedColor, backColor, defaultFont);
        }

        public static void ShowTipText(string text, int duration, Color color, Color? backColor, Font font)
        {
            if (font == null)
                font = new Font("微软雅黑", 26f, FontStyle.Bold);

            if (Application.MessageLoop)
            {
                ShowTipCore(text, duration, color, backColor, font);
            }
            else
            {
                SynchronizationContext syncContext = SynchronizationContext.Current;
                if (syncContext == null)
                {
                    Thread thread = new Thread(() =>
                    {
                        Application.Run(new TipForm(text, duration, color, backColor, font));
                    });
                    thread.SetApartmentState(ApartmentState.STA);
                    thread.Start();
                }
                else
                {
                    syncContext.Post(_ => ShowTipCore(text, duration, color, backColor, font), null);
                }
            }
        }

        public static void ShowTipText(string text, int duration, Color color, Color? backColor,
            string fontFamily, float fontSize = 26f, FontStyle fontStyle = FontStyle.Bold)
        {
            Font font = new Font(fontFamily, fontSize, fontStyle);
            ShowTipText(text, duration, color, backColor, font);
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
            Screen currentScreen = Screen.FromPoint(Cursor.Position);
            Rectangle workingArea = currentScreen.WorkingArea;
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

        private static void ShowTipCore(string text, int duration, Color color, Color? backColor, Font font)
        {
            TipForm form = new TipForm(text, duration, color, backColor, font);
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
        private Color? _backColor;

        public TipForm(string text, int duration, Color textColor, Color? backColor, Font font)
        {
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            TopMost = true;
            StartPosition = FormStartPosition.Manual;
            AllowTransparency = true;

            if (backColor == Color.Transparent)
            {
                BackColor = Color.Fuchsia;
                TransparencyKey = Color.Fuchsia;
            }
            else
            {
                BackColor = Color.Black;
                TransparencyKey = Color.Black;
            }
            Opacity = 1.0;

            if (textColor == Color.Transparent)
            {
                ForeColor = BackColor;
            }
            else
            {
                ForeColor = textColor;
                if (ForeColor == BackColor)
                {
                    ForeColor = Color.FromArgb(ForeColor.A, ForeColor.R, ForeColor.G, ForeColor.B ^ 1);
                }
            }

            Text = text;
            _tipFont = font;
            Font = _tipFont;
            _backColor = backColor;

            Size textSize = MeasureTextAccurate(text, _tipFont);
            Padding = new Padding(16, 8, 16, 8);
            Size = new Size(
                textSize.Width + Padding.Horizontal,
                textSize.Height + Padding.Vertical);

            _closeTimer = new System.Windows.Forms.Timer { Interval = duration };
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
            rect.Inflate(4, 4);


            Color drawBackColor;
            if (_backColor == null)
            {
                int average = (ForeColor.R + ForeColor.G + ForeColor.B) / 3;
                drawBackColor = average < 128 ? Color.FromArgb(248, 248, 248) : Color.FromArgb(8, 8, 8);
            }
            else if (_backColor == Color.Transparent)
            {
                drawBackColor = Color.Transparent;
            }
            else
            {
                drawBackColor = _backColor.Value;
            }

            if (drawBackColor != Color.Transparent)
            {
                using (SolidBrush backBrush = new SolidBrush(drawBackColor))
                {
                    graphics.FillRectangle(backBrush, rect);
                }
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