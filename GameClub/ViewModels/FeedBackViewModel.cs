using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GameClub.Models;
using Webdiyer.WebControls.Mvc;

namespace GameClub.ViewModels
{
    public class FeedBackViewModel
    {
        public PagedList<Feedback> Feedbacks { get; set; }
        public Feedback Feedback { get; set; }
        public int PageIndex { get; set; }
    }
}