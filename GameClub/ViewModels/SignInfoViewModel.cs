using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GameClub.Models;
using Webdiyer.WebControls.Mvc;


namespace GameClub.ViewModels
{
    public class SignInfoViewModel
    {
        public IEnumerable<SignInfo> SignInfos { get; set; }
        public SignInfo SignInfo { get; set; }
        public PagedList<SignInfo> SignInfoPageTable { get; set; }
        public IEnumerable<SignList> SignLists { get; set; }
        public GameMember GameMember { get; set; }
        public IEnumerable<GameMember> GameMembers { get; set; }
        public string GetGameName(int GameID)
        {
            return GameMembers.Where(g => g.GameID == GameID).Select(g => g.GameName).FirstOrDefault().ToString();
        }
        public string GetIsLeaveString(bool IsLeave)
        {
            return IsLeave ? "请假" : "未请假";
        }
        public int PageID { get; set; }
        public int PageSize { get; set; }
        public string Keyword { get; set; }
        public string SortBy { get; set; }
        public string CurrentSort { get; set; }
        public bool IsOrderBy { get; set; }
    }
}