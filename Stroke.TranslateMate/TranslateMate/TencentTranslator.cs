using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Stroke
{
    public class TencentTranslator : ITranslator
    {
        private const string Endpoint = "tmt.tencentcloudapi.com";
        private const string Action = "TextTranslate";
        private const string Version = "2018-03-21";
        private const string Service = "tmt";

        private readonly string secretId;
        private readonly string secretKey;
        private readonly string region;

        public TencentTranslator(string secretId, string secretKey, string region)
        {
            this.secretId = secretId;
            this.secretKey = secretKey;
            this.region = region ?? "ap-guangzhou";
        }

        public async Task<string> TranslateAsync(string text, string sourceLanguage, string targetLanguage)
        {
            if (string.IsNullOrEmpty(secretId) || string.IsNullOrEmpty(secretKey))
                return string.Empty;

            string payload = BuildRequestBody(text, sourceLanguage, targetLanguage);
            string timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            string signature = GenerateSignature(payload, timestamp);

            HttpWebRequest request = CreateRequest(signature, timestamp);
            await WriteRequestBodyAsync(request, payload);

            return await ParseResponseAsync(request);
        }

        private string BuildRequestBody(string text, string sourceLanguage, string targetLanguage)
        {
            var body = new Dictionary<string, object>
            {
                { "SourceText", text },
                { "Source", sourceLanguage },
                { "Target", targetLanguage },
                { "ProjectId", 0 }
            };
            return new JavaScriptSerializer().Serialize(body);
        }

        private HttpWebRequest CreateRequest(string authorization, string timestamp)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"https://{Endpoint}");
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";

            request.Headers.Add("Authorization", authorization);
            request.Headers.Add("X-TC-Action", Action);
            request.Headers.Add("X-TC-Version", Version);
            request.Headers.Add("X-TC-Timestamp", timestamp);
            request.Headers.Add("X-TC-Region", region);

            return request;
        }

        private async Task WriteRequestBodyAsync(HttpWebRequest request, string payload)
        {
            using (StreamWriter writer = new StreamWriter(await request.GetRequestStreamAsync()))
            {
                await writer.WriteAsync(payload);
            }
        }

        private async Task<string> ParseResponseAsync(HttpWebRequest request)
        {
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    string responseText = reader.ReadToEnd();
                    return ExtractTargetText(responseText);
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        private string ExtractTargetText(string jsonResponse)
        {
            var result = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(jsonResponse);
            if (result.TryGetValue("Response", out object responseObj) && responseObj is Dictionary<string, object> responseDict)
            {
                if (responseDict.TryGetValue("TargetText", out object targetText))
                    return targetText.ToString();
            }
            return string.Empty;
        }

        private string GenerateSignature(string payload, string timestamp)
        {
            DateTime now = DateTime.UtcNow;
            string date = now.ToString("yyyy-MM-dd");

            string canonicalHeaders = $"content-type:application/json; charset=utf-8\nhost:{Endpoint}\n";
            string signedHeaders = "content-type;host";
            string hashedPayload = Sha256Hex(payload);
            string canonicalRequest = $"POST\n/\n\n{canonicalHeaders}\n{signedHeaders}\n{hashedPayload}";
            string hashedCanonicalRequest = Sha256Hex(canonicalRequest);

            string credentialScope = $"{date}/{Service}/tc3_request";
            string stringToSign = $"TC3-HMAC-SHA256\n{timestamp}\n{credentialScope}\n{hashedCanonicalRequest}";

            byte[] secretDate = HmacSha256(Encoding.UTF8.GetBytes($"TC3{secretKey}"), date);
            byte[] secretService = HmacSha256(secretDate, Service);
            byte[] secretSigning = HmacSha256(secretService, "tc3_request");
            byte[] signatureBytes = HmacSha256(secretSigning, stringToSign);
            string signatureHex = BitConverter.ToString(signatureBytes).Replace("-", "").ToLower();

            return $"TC3-HMAC-SHA256 Credential={secretId}/{credentialScope}, SignedHeaders={signedHeaders}, Signature={signatureHex}";
        }

        private static string Sha256Hex(string data)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }

        private static byte[] HmacSha256(byte[] key, string data)
        {
            using (HMACSHA256 hmac = new HMACSHA256(key))
            {
                return hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            }
        }
    }
}