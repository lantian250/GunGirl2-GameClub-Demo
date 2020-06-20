using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GameClub.Models;
using Webdiyer.WebControls.Mvc;

namespace GameClub.ViewModels
{
    public class ContributionListViewModel
    {
        public IEnumerable<ContributionList> ContributionLists { get; set; }
        public ContributionList ContributionList { get; set; }
        public PagedList<ContributionList> ContributionListsPageLists { get; set; }
        public IEnumerable<SignList> SignLists{ get; set; }
        public IEnumerable<MemberGroupList> MemberGroupLists { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string Keyword { get; set; }
        public string SortBy { get; set; }
        public string CurrentSort { get; set; }
        public bool IsOrderBy { get; set; }
    }
}