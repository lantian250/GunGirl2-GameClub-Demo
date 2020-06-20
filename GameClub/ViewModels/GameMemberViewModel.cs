using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GameClub.Models;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace GameClub.ViewModels
{
    public class GameMemberViewModel
    {
        public IEnumerable<GameMember> GameMembers { get; set; }
        public List<GameAuthority> GameAuthorities { get; set; }
        public PagedList<GameMember> GameMembersPagedList { get; set; }
        public string getAuthorityString(int number)
        {
            return GameAuthorities.Where(ga => ga.Number == number).Select(ga => ga.AuthorityString).FirstOrDefault().ToString();    
        }

        public int PageID { get; set; }
        public int PageSize { get; set; }
        public string keyword { get; set; }
        public string SortBy { get; set; }
        public string CurrentSort { get; set; }
        public bool IsOrderBy { get; set; }

    }
}