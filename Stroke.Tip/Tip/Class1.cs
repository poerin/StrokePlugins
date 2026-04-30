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
        /// <summary>
        /// 在屏幕底部中央显示提示文字。
        /// <para>背景完全透明，仅显示文字，无窗体边框和任务栏图标。</para>
        /// </summary>
        /// <param name="text">要显示的文字内容。支持中文，建议使用简短提示。</param>
        /// <param name="color">文字颜色。例如 <see cref="Color.White"/>、<see cref="Color.Lime"/>。</param>
        /// <param name="fontSize">
        /// 字体大小，单位为像素。默认值为 <c>26f</c>。
        /// <para>建议范围：16 ~ 48。过小不易阅读，过大可能超出屏幕。</para>
        /// </param>
        /// <param name="durationMs">
        /// 显示时长，单位为毫秒。默认值为 <c>500</c>。
        /// <para>建议范围：300 ~ 3000。过短可能看不清，过长影响操作流畅度。</para>
        /// </param>
        /// <remarks>
        /// <list type="bullet">
        ///   <item>若当前线程有消息循环，直接在原线程显示。</item>
        ///   <item>若无消息循环，自动创建 STA 线程显示。</item>
        ///   <item>窗体定位在屏幕底部中央，距离底边 100 像素。</item>
        /// </list>
        /// </remarks>
        /// <example>
        /// <code>
        /// // 显示白色提示，持续 1 秒
        /// Tip.ShowTipText("保存成功", Color.White, 26f, 1000);
        /// 
        /// // 显示绿色提示，使用默认大小和时长
        /// Tip.ShowTipText("手势识别中...", Color.Lime);
        /// </code>
        /// </example>
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
        }
    }

    internal class TipForm : Form
    {
        private System.Windows.Forms.Timer _closeTimer;

        public TipForm(string text, Color textColor, float fontSize, int durationMs)
        {
            // ========== 透明背景关键设置 ==========
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            TopMost = true;
            StartPosition = FormStartPosition.Manual;

            // 允许窗体透明（必须）
            AllowTransparency = true;
            // 背景色设为透明（配合 AllowTransparency）
            BackColor = Color.Black;
            TransparencyKey = Color.Black;
            // 不设置 Opacity，让 TransparencyKey 完全透明
            Opacity = 1.0;

            // 使用自定义绘制，避免 Label 的透明问题
            Text = text;
            Font = new Font("微软雅黑", fontSize, FontStyle.Bold);
            ForeColor = textColor;

            // 精确测量文字尺寸
            Size textSize = MeasureTextAccurate(text, Font);
            Padding = new Padding(16, 8, 16, 8);
            Size = new Size(
                textSize.Width + Padding.Horizontal,
                textSize.Height + Padding.Vertical);

            // 定位到屏幕底部中央
            var screen = Screen.PrimaryScreen.WorkingArea;
            Location = new Point(
                (screen.Width - Width) / 2,
                screen.Bottom - Height - 100);

            // 定时关闭
            _closeTimer = new System.Windows.Forms.Timer { Interval = durationMs };
            _closeTimer.Tick += (s, e) => { _closeTimer.Stop(); Close(); };
            _closeTimer.Start();

            FormClosed += (s, e) => _closeTimer.Dispose();
        }

        /// <summary>
        /// 自定义绘制文字，确保透明背景 + 高质量抗锯齿
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // 在 Padding 区域内绘制文字
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

        /// <summary>
        /// 精确测量文本尺寸
        /// </summary>
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

        // 防止背景闪烁
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