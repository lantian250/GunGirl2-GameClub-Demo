using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GameClub.Models;
using Webdiyer.WebControls.Mvc;

namespace GameClub.ViewModels
{
    public class ContributionViewModel
    {
        public IEnumerable<Contribution> Contributions { get; set; }
        public Contribution Contribution { get; set; }
        public PagedList<Contribution> ContributionsPageLists { get; set; }
        public IEnumerable<ContributionList> ContributionLists { get; set; }
        public ContributionList ContributionList { get; set; }
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