using HK_Product.Data;
using HK_Product.Models;
using HK_Product.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Exchange.WebServices.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HK_Product.Controllers
{
    public class QagptController : Controller
    {

        private readonly HKContext _ctx;

        public QagptController(HKContext ctx)
        {
            _ctx = ctx;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public IActionResult Qagpt()
        {
            var Name = User.Identity.Name;
            User user = _ctx.Users.FirstOrDefault(u => u.UserName == Name);
            List<Chat> chats = _ctx.Chats.Where(u => u.UserId == "U0001").ToList();

            // 在这里，你需要为每个 chat 获取相关的 QAHistory 对象，因此你最终会得到一个 List<List<QAHistory>>
            List<List<Qahistory>> qaHistories = chats.Select(chat => _ctx.QAHistory.Where(qa => qa.ChatId == chat.ChatId).ToList()).ToList();

            List<Application> apps = _ctx.Applications.Where(u => u.UserId == user.UserId).ToList();

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Qagpt([Bind("QahistoryQ")] UserqViewModel uq)
        {
            var Name = User.Identity.Name;
            User user = _ctx.Users.FirstOrDefault(u => u.UserName == Name);
            // 從資料庫中取得所有的聊天訊息並按照時間進行排序
            var messages = _ctx.QAHistory.OrderBy(m => m.QahistoryQ).ToList();

            var ch = await TranslationService.chCeng("你好");

            return View(messages);

        }

        public static class TranslationService
        {
            private static readonly HttpClient client = new HttpClient();
            private static readonly string key = "c463e5c24e8449079b99d0a617472744";
            private static readonly string endpoint = "https://api.cognitive.microsofttranslator.com";
            private static readonly string resourceLocation = "eastasia";

            public static async Task<string> chCeng(string ask)
            {
                string route = "/translate?api-version=3.0&from=zh-Hant&to=en";
                object[] body = new[] { new { Text = ask } };
                var requestBody = JsonConvert.SerializeObject(body);

                using (var request = new HttpRequestMessage())
                {
                    request.Method = HttpMethod.Post;
                    request.RequestUri = new Uri(endpoint + route);
                    request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                    request.Headers.Add("Ocp-Apim-Subscription-Key", key);
                    request.Headers.Add("Ocp-Apim-Subscription-Region", resourceLocation);

                    HttpResponseMessage response = await client.SendAsync(request);
                    string result = await response.Content.ReadAsStringAsync();

                    // Parse the response
                    TranslationResult[] deserializedOutput = JsonConvert.DeserializeObject<TranslationResult[]>(result);
                    string translation = deserializedOutput[0].Translations[0].Text;
                    return translation;
                }
            }
        }
        public class TranslationResult
        {
            public TranslatedText[] Translations { get; set; }
        }

        public class TranslatedText
        {
            public string Text { get; set; }
        }

    }
}
