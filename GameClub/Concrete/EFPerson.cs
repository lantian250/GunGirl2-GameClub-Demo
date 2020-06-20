using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GameClub.Abstract;
using GameClub.Models;

namespace GameClub.Concrete
{
    public class EFPerson : IPerson
    {
        GameClubEntities GameClubEntities = new GameClubEntities();
        public IEnumerable<Questionary> Questionaries => GameClubEntities.Questionary;

        public IEnumerable<Article> Articles => GameClubEntities.Article;

        public bool AddFeedback(Feedback feedback)
        {
            if (feedback == null)
            {
                return false;
            }
            GameClubEntities.Feedback.Add(feedback);
            GameClubEntities.SaveChanges();
            return true;
        }

        public bool AddFillOut()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Contribution> Contributions(int GameID)
        {
            return GameClubEntities.Contribution.Where(c => c.GameID == GameID).OrderByDescending(c => c.ContributionID);
        }

        public IEnumerable<Feedback> Feedbacks(int UserID)
        {
            return GameClubEntities.Feedback.Where(f => f.UserID == UserID).OrderByDescending(f => f.FeedBackID);
        }

        public IEnumerable<FillOut> FillOuts(int GameID)
        {
            return GameClubEntities.FillOut.Where(f => f.GameID == GameID).OrderByDescending(f => f.QuestionaryID);
        }

        public GameMember GameMember(int GameID)
        {
            return GameClubEntities.GameMember.Where(g => g.GameID == GameID).FirstOrDefault();
        }

        public IEnumerable<MemberGroup> MemberGroups(int GameID)
        {
            return GameClubEntities.MemberGroup.Where(m => m.GameID == GameID);
        }

        public IEnumerable<SignInfo> SignInfos(int GameID)
        {
            return GameClubEntities.SignInfo.Where(s => s.GameID == GameID);
        }

        
    }
}