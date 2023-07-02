using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Text.Json;

namespace HK_Product.Helper
{
    public static class ChCeng
    {


        public static async Task<string> Change(string ask)
        {
            

            try
            {
                var httpClient = new HttpClient();
                var endpoint = "https://api.cognitive.microsofttranslator.com";
                var key = "c463e5c24e8449079b99d0a617472744";
                var resourceLocation = "eastasia";
                var url = $"{endpoint}/translate?api-version=3.0&from=zh-Hant&to=en";

                var requestContent = new
                {
                    text = ask
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

                string sim = responseData[0].translations[0].text;

                return sim; // 返回翻譯結果
            }
            catch (Exception error)
            {
                Console.WriteLine("Error: " + error.Message);
                return null;
            }
        }
    }
}
