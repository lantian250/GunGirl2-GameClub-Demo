using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GameClub.Models;
using Webdiyer.WebControls.Mvc;

namespace GameClub.ViewModels
{
    public class InformMessageViewModel
    {
        public PagedList<InformMessage> InformMessages { get; set; }
        public int PageIndex { get; set; }
        public string Keyword { get; set; }
        public bool IsDesc { get; set; }
    }
}