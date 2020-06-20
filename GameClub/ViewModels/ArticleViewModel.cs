using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GameClub.Models;
using Webdiyer.WebControls.Mvc;

namespace GameClub.ViewModels
{
    public class ArticleViewModel
    {
        public IEnumerable<Article> Articles { get; set; }
        public PagedList<Article> ArticlePagedList { get; set; }
        public Article Article { get; set; }
        public int PageIndex { get; set; }
        public string Keyword { get; set; }
    }
}