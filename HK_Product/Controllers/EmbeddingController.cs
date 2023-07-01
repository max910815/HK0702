using HK_Product.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System;
using HK_Product.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace HK_Product.Controllers
{
    public class EmbeddingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        private readonly HKContext _ctx;

        public EmbeddingController(HKContext ctx)
        {
            _ctx = ctx;
        }

        public class MyJsonObject
        {
            [JsonPropertyName("content")]
            public string Content { get; set; }

            [JsonPropertyName("embedding")]
            public List<float> Embedding { get; set; }
        }



        public async Task<IActionResult> readfile()
        {
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "files");

            string[] jsonFiles = Directory.GetFiles(folderPath, "*.json");


            foreach (string filePath in jsonFiles)
            {
                string id;
                var userWithMaxId = await _ctx.Embedding.OrderByDescending(u => u.EmbeddingId).FirstOrDefaultAsync();
                if (userWithMaxId != null && userWithMaxId.EmbeddingId != null)
                {
                    // 從 UserID 中提取出數字部分，並轉換成整數
                    int maxId = int.Parse(userWithMaxId.EmbeddingId.Substring(1));

                    // 新的 UserID 是最大 UserID 加一
                    int newId = maxId + 1;

                    // 將新的 UserID 轉換回字符串形式，並確保它始終有四位數字
                    string newUserId = "E" + newId.ToString().PadLeft(4, '0');
                    id = newUserId;

                }
                else
                {
                    // 如果沒有任何 User，則新的 UserID 可以是 "E0001"
                    string newUserId = "E0001";
                    id = newUserId;
                }


                string jsonContent = System.IO.File.ReadAllText(filePath);


                MyJsonObject obj = JsonConvert.DeserializeObject<MyJsonObject>(jsonContent);

                // 現在你可以使用 obj.Context 和 obj.Embedding 存取內容了
                Console.WriteLine(obj.Content);
                Console.WriteLine(obj.Embedding);


                string context = obj.Content;
                string embedding = System.Text.Json.JsonSerializer.Serialize(obj.Embedding);


                Embedding embs = new Embedding()
                {
                    EmbeddingId = id,
                    EmbeddingQuestion = "NULL",
                    EmbeddingAnswer = "NULL",
                    Qa = context,
                    EmbeddingVectors = embedding,
                    AifileId = "A1000"
                };

                _ctx.Add(embs);
                await _ctx.SaveChangesAsync();
            }


            return View();
        }
    }
}
