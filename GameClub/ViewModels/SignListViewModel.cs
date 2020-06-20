using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GameClub.Models;
using Webdiyer.WebControls.Mvc;

namespace GameClub.ViewModels
{
    public class SignListViewModel
    {
        public IEnumerable<SignList> signLists { get; set; }
        public IEnumerable<ContributionList> ContributionLists { get; set; }
        public IEnumerable<MemberGroupList> MemberGroupLists{ get; set; }
        public SignList signList { get; set; }
        public PagedList<SignList> signListsPageList { get; set; }
        public int pageID { get; set; }
        public int pageSize{ get; set; }
        public string keyword { get; set; }
        public string SortBy { get; set; }
        public string CurrentSort { get; set; }
        public bool IsOrderBy { get; set; }
    }
}