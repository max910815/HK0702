using HK_Product.Controllers;
using HK_Product.Data;
using HK_Product.Interface;
using HK_Product.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HK_Product.Services
{
    public class AccountServices
    {

        private readonly HKContext _ctx;
        private readonly IHashService _hashService;

        public AccountServices(HKContext context, IHashService hashService)
        {
            _ctx = context;
            _hashService = hashService;
        }

        public async Task<ApplicationUser> AuthenticateUser(LoginViewModel loginVM)
        {
            //find user
            // _hashService.MD5Hash(loginVM.Password)
            var user = await _ctx.Users
                .FirstOrDefaultAsync(u => u.UserEmail.ToUpper() == loginVM.Email && u.UserPassword == _hashService.MD5Hash( loginVM.Password));

            if (user != null)
            {
                var userInfo = new ApplicationUser
                {
                    Name = user.UserName,
                    Email = user.UserEmail,
                    
                };

                return userInfo;
            }
            else
            {
                return null;
            }
        }
    }
}
