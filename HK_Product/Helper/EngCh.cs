using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Text.Json;

namespace HK_Product.Helper
{
    public static class EngCh
    {
        public static async Task<IActionResult> Translate(string sim2)
        {
            try
            {
                var httpClient = new HttpClient();
                var endpoint = "https://api.cognitive.microsofttranslator.com";
                var key = "c463e5c24e8449079b99d0a617472744";
                var resourceLocation = "eastasia";
                var url = $"{endpoint}/translate?api-version=3.0&from=en&to=zh-Hant";

                var requestContent = new
                {
                    text = sim2
                };

                var json = JsonSerializer.Serialize(requestContent);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                content.Headers.Add("Ocp-Apim-Subscription-Key", key);
                content.Headers.Add("Ocp-Apim-Subscription-Region", resourceLocation);
                content.Headers.Add("X-ClientTraceId", Guid.NewGuid().ToString());

                var response = await httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();

                var responseJson = await response.Content.ReadAsStringAsync();
                var responseData = JsonSerializer.Deserialize<dynamic>(responseJson);

                string translatedText = responseData[0].translations[0].text;

                return new ContentResult
                {
                    Content = translatedText,
                    ContentType = "text/plain",
                    StatusCode = 200
                };
            }
            catch (Exception error)
            {
                Console.WriteLine("Error: " + error.Message);
                return new StatusCodeResult(500);
            }
        }
    }
}
