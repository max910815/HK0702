using Microsoft.AspNetCore.Mvc;
using HK_Product.ViewModels;
using HK_Product.Models;
using System;
using HK_Product.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using HK_Product.Interface;
using HK_Product.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;
using Microsoft.Exchange.WebServices.Data;
using static System.Net.Mime.MediaTypeNames;
using System.Text.Json.Serialization;

namespace HK_Product.Controllers
{
    public class RegisterController : Controller
    {

        private readonly HKContext _ctx;
        private readonly IHashService _hashService;
        private readonly AccountServices _accountServices;

        public RegisterController(HKContext ctx, AccountServices accountServices, IHashService hashService)
        {
            _ctx = ctx;
            _accountServices = accountServices;
            _hashService = hashService;
        }


        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("UserName,UserEmail,UserPassword")] User user)
        {
            if (ModelState.IsValid)
            {

                var existingUser = await _ctx.Users.SingleOrDefaultAsync(u => u.UserEmail == user.UserEmail);
                var userWithMaxId = await _ctx.Users.OrderByDescending(u => u.UserId).FirstOrDefaultAsync();
                var appWithMaxId = await _ctx.Applications.OrderByDescending(u => u.ApplicationId).FirstOrDefaultAsync();


                if (existingUser != null)
                {
                    // Email already exists in the database
                    ViewBag.ErrorMessage = "Signin failed: email already exists.";
                    return View(user);
                }
                else
                {
                    if (userWithMaxId.UserId != null)
                    {
                        // 從 UserID 中提取出數字部分，並轉換成整數
                        int maxId = int.Parse(userWithMaxId.UserId.Substring(1));

                        // 新的 UserID 是最大 UserID 加一
                        int newId = maxId + 1;

                        // 將新的 UserID 轉換回字符串形式，並確保它始終有四位數字
                        string newUserId = "U" + newId.ToString().PadLeft(4, '0');
                        user.UserId = newUserId;

                    }
                    else
                    {
                        // 如果沒有任何 User，則新的 UserID 可以是 "U0001"
                        string newUserId = "U0001";
                        user.UserId = newUserId;
                    }

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

                    user.UserPassword = _hashService.MD5Hash(user.UserPassword);
                    // Email does not exist, proceed with creating the new user
                    _ctx.Add(user);
                    await _ctx.SaveChangesAsync();

                    


                    ViewBag.SuccessMessage = "Signin successful!";
                    return RedirectToAction("Login", "Register");
                }
            }
            return RedirectToAction("Index", "Home");

        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginvm)
        {
            if (ModelState.IsValid)
            {

                ApplicationUser user1 = await _accountServices.AuthenticateUser(loginvm);

                if (user1 == null)
                {
                    ModelState.AddModelError(string.Empty, "帳號密碼有錯!!!");
                    return View(loginvm);
                }


                //Success
                //通過以上帳密比對成立後, 以下開始建立授權
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user1.Name),
                    new Claim(ClaimTypes.Role, user1.Role)   
                    //new Claim(ClaimTypes.Role, user.Role) // 如果要有「群組、角色、權限」，可以加入這一段  
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);



                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity)
                    );

                return RedirectToAction("Index", "Home");

            }

            return View();
        }

        //登出
        public async Task<IActionResult> Signout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index","Home");
        }
    }
}
