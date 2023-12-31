﻿using System.ComponentModel.DataAnnotations;

namespace HK_Product.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "必須輸入")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "必須輸入")]
        [StringLength(20, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
