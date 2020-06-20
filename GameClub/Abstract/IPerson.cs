using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameClub.Models;

namespace GameClub.Abstract
{
    public interface IPerson
    {
        GameMember GameMember(int GameID);
        IEnumerable<SignInfo> SignInfos(int GameID);
        IEnumerable<Contribution> Contributions(int GameID);
        IEnumerable<MemberGroup> MemberGroups(int GameID);
        IEnumerable<Questionary> Questionaries { get; }
        IEnumerable<FillOut> FillOuts(int GameID);
        IEnumerable<Article> Articles { get; }
        IEnumerable<Feedback> Feedbacks(int UserID);
        bool AddFeedback(Feedback feedback);
        bool AddFillOut();
    }
}
