using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace GameClub.ViewModels
{
    public class ChangePasswordViewModel
    {
        public int UserID { get; set; }
        [Required(ErrorMessage = "旧密码不能为空")]
        public string Password { get; set; }
        [Required(ErrorMessage = "新密码不能为空")]
        public string ChangePassword { get; set; }
        [Compare("ChangePassword", ErrorMessage = "新密码不一致")]
        public string ChangePasswordTwo { get; set; }
    }
}