using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using GameClub.Models;
using Webdiyer.WebControls.Mvc;

namespace GameClub.ViewModels
{
    public class AllUserInfoViewModel
    {
        GameClubEntities gameClubEntities = new GameClubEntities();
        public IEnumerable<UserInfo> UserInfos { get; set; }
        public PagedList<UserInfo> UserInfosPageList{ get; set; }
        public List<Authority> Authorities { get; set; }
        
        public List<GameMember> GameMembers { get; set; }
        public string Authority(int authority)
        {
            return gameClubEntities.Authority.Where(a => a.Number == authority).Select(a => a.AuthorityString).FirstOrDefault().ToString();
        }
        public int PageID { get; set; }
        public string returnUrl { get; set; }
        public string keyword { get; set; }
        public int PageSize { get; set; }
        public string SortBy { get; set; }
        public string CurrentSort { get; set; }
    }
}