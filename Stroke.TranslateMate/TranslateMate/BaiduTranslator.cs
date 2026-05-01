using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Stroke
{
    public class BaiduTranslator : ITranslator
    {
        private readonly string appId;
        private readonly string secretKey;

        public BaiduTranslator(string appId, string secretKey)
        {
            this.appId = appId;
            this.secretKey = secretKey;
        }

        public Task<string> TranslateAsync(string text, string sourceLanguage, string targetLanguage)
        {
            if (string.IsNullOrEmpty(appId) || string.IsNullOrEmpty(secretKey))
                return Task.FromResult(string.Empty);

            string salt = new Random().Next(100000).ToString();
            string sign = GenerateSign(appId, text, salt, secretKey);
            string url = $"http://api.fanyi.baidu.com/api/trans/vip/translate?q={HttpUtility.UrlEncode(text)}&from={sourceLanguage}&to={targetLanguage}&appid={appId}&salt={salt}&sign={sign}";

            return RequestTranslationAsync(url);
        }

        private async Task<string> RequestTranslationAsync(string url)
        {
            Stream responseStream = await GetResponseStreamAsync(url);
            if (responseStream == null)
                return string.Empty;

            using (StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8")))
            {
                string response = reader.ReadToEnd();
                return ExtractTranslation(response);
            }
        }

        private async Task<Stream> GetResponseStreamAsync(string url)
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
            catch
            {
                return null;
            }
        }

        private string GenerateSign(string appId, string query, string salt, string secretKey)
        {
            return string.Join("", Array.ConvertAll(
                MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(appId + query + salt + secretKey)),
                value => value.ToString("x2")));
        }

        private string ExtractTranslation(string jsonResponse)
        {
            StringBuilder translationBuilder = new StringBuilder();
            foreach (Match match in Regex.Matches(
                Regex.Match(jsonResponse, "trans_result\":\\[.*\\]").Value,
                "{\"src\":\".*?\",\"dst\":\".*?\"}"))
            {
                translationBuilder.AppendLine(
                    Regex.Unescape(
                        Regex.Match(match.Value, "\"dst\":\".*\"").Value
                             .Replace("\"dst\":", ""))
                         .Trim('\"'));
            }
            return translationBuilder.ToString();
        }
    }
}