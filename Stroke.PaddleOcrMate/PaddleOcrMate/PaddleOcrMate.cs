using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;


namespace Stroke
{
    public static class PaddleOcrMate
    {
        public static string ApiUrl;
        public static string Token;

        private static readonly string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        private static ClipboardListener clipboardListener;
        private static bool isOcrPending;

        static PaddleOcrMate()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            ApiUrl = "";
            Token = "";
            clipboardListener = new ClipboardListener();
            isOcrPending = false;

            clipboardListener.Do += async delegate
            {
                if (!isOcrPending)
                    return;

                isOcrPending = false;
                clipboardListener.Listening = false;

                string recognizedText = await ProcessClipboardForOcr();
                if (!string.IsNullOrEmpty(recognizedText))
                {
                    DisplayText(recognizedText);
                }
            };
        }

        public static void BeginClipboardOcr()
        {
            isOcrPending = true;
            clipboardListener.Listening = true;
        }

        private static async Task<string> ProcessClipboardForOcr()
        {
            byte[] imageBytes = null;

            if (Clipboard.ContainsImage())
            {
                using (Image clipboardImage = Clipboard.GetImage())
                {
                    if (clipboardImage != null)
                    {
                        using (MemoryStream imageStream = new MemoryStream())
                        {
                            clipboardImage.Save(imageStream, System.Drawing.Imaging.ImageFormat.Png);
                            imageBytes = imageStream.ToArray();
                        }
                    }
                }
            }
            else if (Clipboard.ContainsFileDropList())
            {
                var droppedFiles = Clipboard.GetFileDropList();
                if (droppedFiles.Count > 0)
                {
                    string filePath = droppedFiles[0].Trim('"');
                    string extension = Path.GetExtension(filePath)?.ToLower();
                    string[] supportedExtensions = { ".png", ".jpg", ".jpeg", ".bmp", ".tiff", ".tif", ".gif", ".webp" };
                    if (supportedExtensions.Contains(extension))
                    {
                        try
                        {
                            imageBytes = System.IO.File.ReadAllBytes(filePath);
                        }
                        catch
                        { }
                    }
                }
            }

            if (imageBytes == null)
            {
                return string.Empty;
            }

            return await CallPaddleOcrApi(imageBytes);
        }

        private static async Task<string> CallPaddleOcrApi(byte[] imageBytes)
        {
            if (string.IsNullOrEmpty(ApiUrl) || string.IsNullOrEmpty(Token))
                return string.Empty;

            try
            {
                string base64Image = Convert.ToBase64String(imageBytes);

                var requestPayload = new
                {
                    file = base64Image,
                    fileType = 1,
                    useDocOrientationClassify = false,
                    useDocUnwarping = false,
                    useChartRecognition = false,
                };

                string jsonPayload = new JavaScriptSerializer().Serialize(requestPayload);

                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", "token " + Token);
                    var httpContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await httpClient.PostAsync(ApiUrl, httpContent);
                    if (!response.IsSuccessStatusCode)
                        return string.Empty;

                    string responseBody = await response.Content.ReadAsStringAsync();
                    return ExtractMarkdownFromResponse(responseBody);
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        private static string ExtractMarkdownFromResponse(string jsonResponse)
        {
            try
            {
                var serializer = new JavaScriptSerializer();
                var resultObject = serializer.Deserialize<dynamic>(jsonResponse);
                var layoutResults = resultObject["result"]["layoutParsingResults"];
                if (layoutResults != null && layoutResults.Length > 0)
                {
                    return layoutResults[0]["markdown"]["text"];
                }
            }
            catch
            { }
            return string.Empty;
        }

        public static void DisplayText(string text)
        {
            Form displayForm = new Form();
            displayForm.Text = "Stroke.PaddleOcrMate";
            displayForm.FormBorderStyle = FormBorderStyle.Sizable;
            displayForm.StartPosition = FormStartPosition.Manual;
            displayForm.ShowIcon = false;
            displayForm.ShowInTaskbar = true;
            displayForm.ControlBox = true;
            displayForm.MinimizeBox = true;
            displayForm.MaximizeBox = true;
            displayForm.TopMost = true;
            displayForm.MinimumSize = new Size(300, 360);

            TextBox resultTextBox = new TextBox();
            resultTextBox.Multiline = true;
            resultTextBox.ScrollBars = ScrollBars.Vertical;
            resultTextBox.Font = new Font("微软雅黑", 12f);
            resultTextBox.Text = text.Replace("\n", "\r\n");
            resultTextBox.Dock = DockStyle.Fill;
            resultTextBox.ReadOnly = false;
            resultTextBox.WordWrap = true;

            displayForm.Controls.Add(resultTextBox);

            int defaultWidth = 600;
            int maxHeight = (int)(Screen.PrimaryScreen.WorkingArea.Height * 0.7);
            Size estimatedSize = MeasureTextSize(text, resultTextBox.Font, defaultWidth);
            displayForm.Width = defaultWidth;
            displayForm.Height = Math.Min(Math.Max(estimatedSize.Height, 360), maxHeight);

            Point mousePosition = Cursor.Position;
            displayForm.Location = new Point(
                Math.Max(0, mousePosition.X - displayForm.Width / 2),
                Math.Max(0, mousePosition.Y - displayForm.Height / 2)
            );

            displayForm.Show();
        }

        private static Size MeasureTextSize(string text, Font font, int width)
        {
            string[] lines = text.Split('\n');
            int lineCount = lines.Length;

            using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
            {
                int maxLineWidth = 0;
                foreach (string line in lines)
                {
                    string cleanLine = line.TrimEnd('\r');
                    SizeF lineSize = graphics.MeasureString(cleanLine, font);
                    if (lineSize.Width > maxLineWidth)
                        maxLineWidth = (int)Math.Ceiling(lineSize.Width);
                }
                int lineHeight = TextRenderer.MeasureText("A", font).Height;
                int totalHeight = lineHeight * lineCount + 20;
                return new Size(maxLineWidth + 20, totalHeight);
            }
        }
    }
}