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
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using HK_Product.Helper;

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
            var claim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            string userid = claim != null ? claim.Value : null;

            userid = "U0001";

            var chat = _ctx.Chats.Where(c => c.UserId == userid).OrderByDescending(c => c.ChatTime).FirstOrDefault();
            var qa = _ctx.QAHistory.Where(q => q.ChatId == chat.ChatId).OrderByDescending(q => q.QahistoryId).FirstOrDefault();
            var user = _ctx.Users.Include(u => u.Applications).FirstOrDefault(u => u.UserId == userid);

            var model = new QagptViewModel
            {
                Chat = chat,
                QA = qa,
                User = user
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Qagpt(string text)
        {
            var ch = await ChCeng.Change(text);

            //OpenpiViewModel openapi = new OpenpiViewModel() 
            //{
            //    Appid = ,
            //    Temperature = "1",
            //    Chatid = ,
            //    Q = text,
            //    Dateid =  

            //};
            
            var sim = await Openapi.OpenApi(ch);

            return Json(sim);

        }

        
        [HttpPost]
        public async Task<IActionResult> AddChat(string name)
        {
            string userid = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).ToString();
            if (userid != null)
            {
                //抓到聊天室model最後一個id
                Chat chat1 = await _ctx.Chats.OrderByDescending(c => c.ChatId).FirstOrDefaultAsync();
                string oldchatid = chat1?.ChatId;

                string newChatId = oldchatid.Substring(0, 1) + (int.Parse(oldchatid.Substring(1)) + 1).ToString("D4");
                //新增聊天室窗
                Chat chat = new Chat()
                {
                    ChatId = newChatId,
                    ChatTime = DateTime.Now,
                    ChatName = "NewChat",
                    UserId = userid,
                };
                _ctx.Chats.Add(chat);
                await _ctx.SaveChangesAsync();

                return Json(chat);
            }
            return Json(new {success = false});

        }


        
    }
}
