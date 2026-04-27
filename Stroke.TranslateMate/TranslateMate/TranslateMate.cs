using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;


namespace Stroke
{
    public static class TranslateMate
    {
        public static string AppId;
        public static string SecretKey;
        public static string From;
        public static string To;
        public static bool UseAudio;
        public static bool UseGlossary;

        private static readonly string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        private static bool isGlossaryChanged;
        private static Dictionary<string, int> glossary;
        private static System.Threading.Timer glossarySaveTimer;
        private static ClipboardListener clipboardListener;
        private static bool isTranslating;

        [DllImport("winmm.dll")]
        public static extern uint mciSendString(string command, string returnString, uint returnLength, uint callback);

        public static void PlayAudio(string word)
        {
            string safeWord = string.Join("_", word.Split(Path.GetInvalidFileNameChars()));
            string audioDirectory = Path.Combine(BaseDirectory, "Audio");
            string filePath = Path.Combine(audioDirectory, safeWord + ".mp3");

            if (!File.Exists(filePath))
            {
                try
                {
                    using (WebClient client = new WebClient())
                    {
                        client.DownloadFile("https://dict.youdao.com/dictvoice?audio=" + word, filePath);
                    }
                }
                catch (WebException)
                {
                    if (File.Exists(filePath))
                    {
                        try { File.Delete(filePath); } catch { }
                    }
                }
                catch
                { }
            }

            if (File.Exists(filePath))
            {
                string audioAlias = "word_audio_" + word.GetHashCode();
                mciSendString("close " + audioAlias, null, 0u, 0u);
                mciSendString("open \"" + filePath + "\" alias " + audioAlias, null, 0u, 0u);
                mciSendString("play " + audioAlias, null, 0u, 0u);
                Task.Delay(3000).ContinueWith(_ => mciSendString("close " + audioAlias, null, 0u, 0u));
            }
        }

        public static void WordToGlossary(string word)
        {
            if (glossary.ContainsKey(word))
            {
                glossary[word]++;
            }
            else
            {
                glossary.Add(word, 1);
            }
            isGlossaryChanged = true;
        }

        public static void SaveGlossary(object state)
        {
            try
            {
                if (!isGlossaryChanged)
                    return;

                string glossaryPath = Path.Combine(BaseDirectory, "Glossary.csv");
                using (StreamWriter writer = new StreamWriter(glossaryPath))
                {
                    foreach (KeyValuePair<string, int> entry in glossary.OrderBy(x => x.Key))
                    {
                        writer.WriteLine($"{entry.Key},{entry.Value}");
                    }
                }
                isGlossaryChanged = false;
            }
            catch
            {
            }
        }

        static TranslateMate()
        {
            AppId = "";
            SecretKey = "";
            From = "en";
            To = "zh";
            UseAudio = true;
            UseGlossary = true;
            isGlossaryChanged = false;
            glossary = new Dictionary<string, int>();
            glossarySaveTimer = new System.Threading.Timer(SaveGlossary);
            clipboardListener = new ClipboardListener();
            isTranslating = false;

            if (UseGlossary)
            {
                string glossaryPath = Path.Combine(BaseDirectory, "Glossary.csv");
                try
                {
                    using (FileStream fileStream = new FileStream(glossaryPath, FileMode.OpenOrCreate))
                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            string[] parts = line.Split(',');
                            glossary.Add(parts[0], int.Parse(parts[1]));
                        }
                    }
                }
                catch { }
                glossarySaveTimer.Change(60000, 60000);
            }

            if (UseAudio && !Directory.Exists(Path.Combine(BaseDirectory, "Audio")))
            {
                Directory.CreateDirectory(Path.Combine(BaseDirectory, "Audio"));
            }

            clipboardListener.Do += async delegate
            {
                string text = Clipboard.GetText();
                if (text.Length != 0)
                {
                    bool isEnglish = true;
                    Random random = new Random(default(Guid).GetHashCode());

                    for (int index = 0, nonAsciiCount = 0; index < 16; index++)
                    {
                        if (text[random.Next(0, text.Length)] > '\u0080')
                        {
                            nonAsciiCount++;
                        }
                        if (nonAsciiCount > 8)
                        {
                            isEnglish = false;
                            break;
                        }
                    }

                    text = ((!isEnglish) ? text.Replace("\r\n", "") : text.Replace("-\r\n", "").Replace("\r\n", " "));

                    if (isTranslating)
                    {
                        if (isEnglish)
                        {
                            From = "en";
                            To = "zh";
                        }
                        else
                        {
                            From = "zh";
                            To = "en";
                        }

                        string translation = await Query(text);
                        if (translation != "")
                        {
                            DisplayText(translation);
                        }

                        if (isEnglish && Regex.IsMatch(text, "^ ?[0-9a-zA-Z]+ ?$"))
                        {
                            text = text.Trim(' ');
                            if (Regex.IsMatch(text, "^[A-Z][a-z]+$"))
                            {
                                text = text.ToLower();
                            }
                            if (UseAudio)
                            {
                                PlayAudio(text);
                            }
                            if (UseGlossary)
                            {
                                WordToGlossary(text);
                            }
                        }
                    }
                    else
                    {
                        DisplayText(text);
                    }
                    clipboardListener.Listening = false;
                }
            };
        }

        public static void DisplayText(string text)
        {
            Form form = new Form();
            form.FormBorderStyle = FormBorderStyle.None;
            form.StartPosition = FormStartPosition.Manual;
            form.ControlBox = false;
            form.ShowIcon = false;
            form.ShowInTaskbar = false;
            form.BackColor = Color.FromArgb(242, 236, 220);
            form.Opacity = 0.96;
            Label label = new Label();
            label.Text = text;
            label.Font = new Font("微软雅黑", 12f);
            label.MaximumSize = new Size(640, 1080);
            label.AutoSize = true;
            label.Location = new Point(4, 4);
            label.TextAlign = ContentAlignment.MiddleLeft;
            form.Controls.Add(label);
            form.MinimumSize = new Size(label.Width + 8, label.Height + 8);
            form.MaximumSize = form.MinimumSize;
            form.Size = form.MinimumSize;
            form.Location = new Point(Cursor.Position.X - form.Size.Width / 2, Cursor.Position.Y - form.Size.Height / 2);
            bool isDragging = false;
            int dragOffsetX = Control.MousePosition.X;
            int dragOffsetY = Control.MousePosition.Y;
            label.MouseDown += ((object sender, MouseEventArgs e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    dragOffsetX = e.X;
                    dragOffsetY = e.Y;
                    isDragging = true;
                }
            });
            label.MouseMove += ((object sender, MouseEventArgs e) =>
            {
                if (isDragging)
                {
                    form.Location = new Point(Control.MousePosition.X - dragOffsetX, Control.MousePosition.Y - dragOffsetY);
                }
            });
            label.MouseUp += ((object sender, MouseEventArgs e) =>
            {
                if (e.Button == MouseButtons.Left && isDragging)
                {
                    isDragging = false;
                }
            });
            label.MouseClick += ((object sender, MouseEventArgs e) =>
            {
                if (e.Button == MouseButtons.Right)
                {
                    form.Close();
                }
            });
            label.MouseDoubleClick += ((object sender, MouseEventArgs e) =>
            {
                Clipboard.SetText(label.Text);
            });
            form.TopMost = true;
            form.Show();
        }

        private static async Task<Stream> GetWebStream(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            request.Timeout = 6000;
            try
            {
                WebResponse response = await request.GetResponseAsync();
                return response.GetResponseStream();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<string> Query(string queryText)
        {
            string salt = (new Random()).Next(100000).ToString();
            string sign = string.Join("", Array.ConvertAll(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(AppId + queryText + salt + SecretKey)), (o) => o.ToString("x2")));
            string url = $"http://api.fanyi.baidu.com/api/trans/vip/translate?q={HttpUtility.UrlEncode(queryText)}&from={From}&to={To}&appid={AppId}&salt={salt}&sign={sign}";
            Stream responseStream = await GetWebStream(url);
            if (responseStream == null)
                return string.Empty;
            StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
            string result = reader.ReadToEnd();
            reader.Close();
            responseStream.Close();
            StringBuilder translationBuilder = new StringBuilder();
            foreach (Match match in Regex.Matches(Regex.Match(result, "trans_result\":\\[.*\\]").Value, "{\"src\":\".*?\",\"dst\":\".*?\"}"))
            {
                translationBuilder.AppendLine(Regex.Unescape(Regex.Match(match.Value, "\"dst\":\".*\"").Value.Replace("\"dst\":", "")).Trim('\"'));
            }
            return translationBuilder.ToString();
        }

        public static void BeginClipboardDisplay()
        {
            isTranslating = false;
            clipboardListener.Listening = true;
        }

        public static void BeginClipboardTranslation()
        {
            isTranslating = true;
            clipboardListener.Listening = true;
        }
    }
}