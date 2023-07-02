using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using System;

namespace HK_Product.Helper
{
    public static class Openapi
    {
        public static async Task<string> OpenApi(string str)
        {
            var httpClient = new HttpClient();
            var url = "https://localhost:7168/api/Similar";

            var data = new
            {
                ApplicationId = "1",
                temperature = "0.95",
                ChatId = "1",
                Question = str,
                DataId = "1"
            };

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };

            var json = JsonSerializer.Serialize(data, options);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await httpClient.PostAsync(url, content);
                var resultJson = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<dynamic>(resultJson, options);
                return result.ans;
            }
            catch (Exception error)
            {
                Console.WriteLine("Error: " + error.Message);
                return null;
            }
        }
    }
}
