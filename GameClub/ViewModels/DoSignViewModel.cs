using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GameClub.Models;

namespace GameClub.ViewModels
{
    public class DoSignViewModel
    {
        public IEnumerable<SignList> signLists { get; set; }
        public List<SignInfo> signInfos { get; set; }
        public List<MemberGroup> MemberGroups { get; set; }
    }
}