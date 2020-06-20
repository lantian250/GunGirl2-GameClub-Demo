using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GameClub.Models;
using System.ComponentModel.DataAnnotations;

namespace GameClub.ViewModels
{
    public class MyUserInfoViewModel
    {
        public int UserID { get; set; }
        public UserInfo UserInfo { get; set; }
        [Required(ErrorMessage ="用户昵称不能为空！")]
        public string UserName { get; set; }

        public InformMessage InformMessage { get; set; }
    }
    
}