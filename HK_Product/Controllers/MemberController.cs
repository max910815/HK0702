using HK_Product.Data;
using HK_Product.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.FileIO;
using Microsoft.Exchange.WebServices.Data;

namespace HK_Product.Controllers
{
    public class MemberController : Controller
    {

        private readonly ILogger<MemberController> _logger;
        private readonly HKContext _ctx;

        public MemberController(HKContext ctx, ILogger<MemberController> logger)
        {
            _ctx = ctx;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Member()
        {
            // 獲取當前已驗證使用者的名稱
            string userName = User.Identity.Name;

            // 根據使用者名稱從資料庫中查詢相關的使用者資訊
            User user = _ctx.Users.FirstOrDefault(u => u.UserName == userName);

            if (user == null)
            {
                // 處理未找到使用者的情況
                // 例如返回一個錯誤視圖或重定向到其他頁面
                return View("NotFound");
            }

            return View(user);
        }


        [HttpPost]

        public async Task<IActionResult> Member(List<IFormFile> files)
        {
            string path = "Upload";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            try
            {
                var appWithMaxId = await _ctx.Applications.OrderByDescending(u => u.ApplicationId).FirstOrDefaultAsync();
                // 獲取當前已驗證使用者的名稱
                string userName = User.Identity.Name;
                // 根據使用者名稱從資料庫中查詢相關的使用者資訊
                User Userid = _ctx.Users.FirstOrDefault(u => u.UserName == userName);

                string newAppId;

                if (appWithMaxId.UserId != null)
                {
                    // 從 UserID 中提取出數字部分，並轉換成整數
                    int maxappId = int.Parse(appWithMaxId.UserId.Substring(1));

                    // 新的 UserID 是最大 UserID 加一
                    int newappId = maxappId + 1;

                    // 將新的 UserID 轉換回字符串形式，並確保它始終有四位數字
                    newAppId = "A" + newappId.ToString().PadLeft(4, '0');

                }
                else
                {
                    // 如果沒有任何 User，則新的 UserID 可以是 "U0001"
                    newAppId = "U0001";
                }

                // Create new Application data
                HK_Product.Models.Application app = new HK_Product.Models.Application
                {
                    ApplicationId = newAppId,
                    // Set Model, Parameter, and UserId as needed
                    Model = "gpt-35-turbo",
                    Parameter = null,
                    UserId = Userid.UserId
                };

                _ctx.Applications.Add(app);
                await _ctx.SaveChangesAsync();

                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        string fileName = Path.GetFileName(file.FileName);
                        string filePath = Path.Combine(path, fileName);
                        string fileType = file.ContentType;

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }

                        // 取得完整檔案路徑
                        string fullPath = Path.GetFullPath(filePath);
                        _logger.LogInformation("Upload file success, path: {FilePath}", fullPath);

                        var fileWithMaxId = await _ctx.AIFiles.OrderByDescending(u => u.AifileId).FirstOrDefaultAsync();
                        string id;

                        if (fileWithMaxId.AifileId != null)
                        {
                            // 從 UserID 中提取出數字部分，並轉換成整數
                            int maxId = int.Parse(fileWithMaxId.AifileId.Substring(1));
                            // 新的 UserID 是最大 UserID 加一
                            int newId = maxId + 1;
                            // 將新的 UserID 轉換回字符串形式，並確保它始終有四位數字
                            string newUserId = "D" + newId.ToString().PadLeft(4, '0');
                            id = newUserId;
                        }
                        else
                        {
                            // 如果沒有任何 User，則新的 UserID 可以是 "U0001"
                            string newUserId = "D0001";
                            id = newUserId;
                        }

                        Aifile embs = new Aifile()
                        {
                            AifileId = id,
                            AifileType = fileType,
                            AifilePath = filePath,
                            ApplicationId = newAppId
                        };

                        _ctx.Add(embs);
                        await _ctx.SaveChangesAsync();
                    }
                }

                TempData["UploadSuccess"] = true;
                User user = _ctx.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
                ViewBag.User = user;
                return RedirectToAction("success","Member");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Upload Error");
                return BadRequest();
            }
        }

        public IActionResult success()
        {
            string userName = User.Identity.Name;

            // 根據使用者名稱從資料庫中查詢相關的使用者資訊
            User user = _ctx.Users.FirstOrDefault(u => u.UserName == userName);

            if (user == null)
            {
                // 如果找不到使用者，返回一個錯誤
                return NotFound("User not found");
            }

            Application app = _ctx.Applications.FirstOrDefault(u => u.UserId == user.UserId);

            if (app == null)
            {
                // 如果找不到相關應用，返回一個錯誤
                return NotFound("Application not found for the user");
            }

            List<Aifile> userAifiles = _ctx.AIFiles.Where(a => a.ApplicationId == app.ApplicationId).ToList();

            if (userAifiles.Count == 0)
            {
                // 如果使用者沒有任何相關的AI文件，返回一個錯誤或提示
                return NotFound("No AI files found for the application");
            }

            return View(userAifiles);
        }



        [HttpGet]
        public IActionResult Revise()
        {
            // 獲取當前已驗證使用者的名稱
            string userName = User.Identity.Name;
            // 根據使用者名稱從資料庫中查詢相關的使用者資訊
            User user = _ctx.Users.FirstOrDefault(u => u.UserName == userName);

            if (user == null)
            {
                return View("NotFound");
            }

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Revise([Bind("UserId,UserName,UserEmail,UserPassword")] User user)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    _ctx.Update(user);
                    await _ctx.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Member", "Member");
            }
            return View(user);

        }

        private bool UserExists(string userId)
        {
            return _ctx.Users.Any(e => e.UserId == userId);

        }
    }
}

