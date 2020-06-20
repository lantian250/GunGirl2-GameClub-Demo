using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GameClub.Models;
using Webdiyer.WebControls.Mvc;

namespace GameClub.ViewModels
{
    public class QuestionaryViewModel
    {
        public PagedList<Questionary> Questionaries { get; set; }
        public IEnumerable<Question> Questions { get; set; }
        public IEnumerable<Select> Selects { get; set; }
        public IEnumerable<FillOut> FillOuts { get; set; }
        public IEnumerable<GameMember> GameMembers { get; set; }
        public string GetGameName(int GameID)
        {
            return GameMembers.Where(g => g.GameID == GameID).Select(g=>g.GameName).FirstOrDefault().ToString();
        }
        public Questionary Questionary { get; set; }
        public Question Question { get; set; }
        public Select Select { get; set; }
        public FillOut FillOut { get; set; }
        public int PageIndex { get; set;}

    }
}