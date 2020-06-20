using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GameClub.Models;
using Webdiyer.WebControls.Mvc;

namespace GameClub.ViewModels
{
    public class MemberGroupViewModel
    {
        public MemberGroup MemberGroup { get; set; }
        public IEnumerable<MemberGroup> MemberGroups { get; set; }
        public PagedList<MemberGroup> MemberGroupsPageLists { get; set; }
        public IEnumerable<MemberGroupList> MemberGroupLists { get; set; }
        public MemberGroupList MemberGroupList { get; set; }
        public GameMember GameMember { get; set; }
        public IEnumerable<GameMember> GameMembers { get; set; }
        public string GetGameName(int GameID)
        {
            return GameMembers.Where(g => g.GameID == GameID).Select(g => g.GameName).FirstOrDefault().ToString();
        }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string Keyword { get; set; }
        public string SortBy { get; set; }
        public string CurrentSort { get; set; }
        public bool IsOrderBy { get; set; }
    }
}