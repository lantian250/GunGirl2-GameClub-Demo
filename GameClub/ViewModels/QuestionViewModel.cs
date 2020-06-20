using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GameClub.Models;
using Webdiyer.WebControls.Mvc;

namespace GameClub.ViewModels
{
    public class QuestionViewModel
    {
        public string QuestionaryID { get; set; }
        public IEnumerable<Question> Questions { get; set; }
        public IEnumerable<Select> Selects { get; set; }
        public IEnumerable<FillOut> FillOuts { get; set; }
        public Questionary Questionary { get; set; }
        public Question Question { get; set; }
        public Select Select { get; set; }
        public FillOut FillOut { get; set; }

    }
}