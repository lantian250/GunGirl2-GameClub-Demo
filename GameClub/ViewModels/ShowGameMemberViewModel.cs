using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GameClub.Models;

namespace GameClub.ViewModels
{
    public class ShowGameMemberViewModel
    {
        public GameMember GameMember { get; set; }
        public IEnumerable<SignInfo> SignInfos { get; set; }
        public List<SignList> SignLists{ get; set; }
        public IEnumerable<Contribution> Contributions { get; set; }
        public IEnumerable<MemberGroup> MemberGroups { get; set; }
    }
}