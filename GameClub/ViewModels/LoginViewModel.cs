using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace GameClub.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage ="请输入用户名")]
        public string UserName { get; set; }
        [Required(ErrorMessage ="请输入密码")]
        public string Password { get; set; }
        [Required(ErrorMessage ="请输入验证码")] 
        public string verifyCode { get; set; }
    }
}