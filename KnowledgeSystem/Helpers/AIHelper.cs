using DevExpress.DataAccess.DataFederation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeSystem.Helpers
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;

    namespace YourNamespace
    {
        public class AIHelper
        {
            private readonly string _apiKey;
            private readonly string _model;
            private readonly HttpClient _client;

            public AIHelper(string apiKey, string model = "gemini-2.5-flash")
            {
                if (string.IsNullOrWhiteSpace(apiKey))
                    throw new ArgumentException("API Key không được để trống.");

                _apiKey = apiKey;
                _model = model;
                _client = new HttpClient();
            }

            /// <summary>
            /// Gửi prompt tới Gemini và nhận phản hồi text.
            /// </summary>
            public async Task<string> GenerateTextAsync(string prompt)
            {
                if (string.IsNullOrWhiteSpace(prompt))
                    throw new ArgumentException("Prompt không được để trống.");

                string url = string.Format(
                    "https://generativelanguage.googleapis.com/v1beta/models/{0}:generateContent",
                    _model);

                // ✅ Sử dụng header x-goog-api-key (đúng chuẩn Gemini)
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Add("x-goog-api-key", _apiKey);
                _client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                // Body JSON
                string json = "{ \"contents\": [{ \"parts\": [{ \"text\": \"" +
                              EscapeJson(prompt) + "\" }] }] }";

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _client.PostAsync(url, content);

                string jsonResponse = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    throw new Exception("Gemini API Error: " + response.StatusCode + " - " + jsonResponse);

                return ExtractTextFromResponse(jsonResponse);
            }

            /// <summary>
            /// Trích text từ JSON trả về.
            /// (Làm gọn, tránh dependency Newtonsoft.)
            /// </summary>
            private string ExtractTextFromResponse(string json)
            {
                string key = "\"text\":";
                int idx = json.IndexOf(key, StringComparison.OrdinalIgnoreCase);
                if (idx >= 0)
                {
                    int start = json.IndexOf('"', idx + key.Length) + 1;
                    int end = json.IndexOf('"', start);
                    if (start > 0 && end > start)
                    {
                        string text = json.Substring(start, end - start);
                        return text.Replace("\\n", "\n").Replace("\\\"", "\"");
                    }
                }
                return json;
            }

            /// <summary>
            /// Escape chuỗi JSON.
            /// </summary>
            private string EscapeJson(string text)
            {
                if (text == null) return "";
                return text.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n");
            }

            /// <summary>
            /// Trích từ khóa bằng Gemini.
            /// </summary>
            public async Task<string> ExtractKeywordsAsync(string text, int numKeywords)
            {
                string prompt = "Hãy đọc đoạn văn sau và xuất ra " + numKeywords +
                                " từ khóa quan trọng nhất, cách nhau bằng dấu phẩy, bằng tiếng trung phồn thể và tiếng việt. Không giải thích.\n\n" + text;
                return await GenerateTextAsync(prompt);
            }
        }
    }
}
