using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameClub.Models;

namespace GameClub.Abstract
{
    public interface IQuestionary
    {
        Questionary Questionary(string questionaryID);
        Question Question(string questionId);
        Select Select(string selectID);
        IEnumerable<Questionary> Questionaries { get; }
        IEnumerable<Question> Questions { get; }
        IEnumerable<Select> Selects { get; }
        IEnumerable<FillOut> FillOuts { get; }

        bool AddQuestionary(Questionary questionary);
        bool DelQuestionary(string questionaryID);
        bool UpdateQuestionary(Questionary questionary);
        bool AddQuestion(Question question);
        bool DelQuestion(string questionID);
        bool UpdateQuestion(Question question);
        bool AddSelect(Select select);
        bool DelSelect(string selectID);
        bool UpdateSelect(Select select);
        bool AddFillOut(List<FillOut> fillOuts);
        bool DelFillOut(string questionaryID, int GameID);


    }
}
