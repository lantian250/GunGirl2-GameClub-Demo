using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GameClub.Models;
using GameClub.Abstract;
namespace GameClub.Concrete
{
    public class EFFeedBack : IFeedBack
    {
        GameClubEntities gameClubEntities = new GameClubEntities();
        EFUserRecord EFUserRecord = new EFUserRecord();

        public IEnumerable<Feedback> Feedbacks => gameClubEntities.Feedback;

        public Feedback Feedback(int feedbackID)
        {
            return gameClubEntities.Feedback.Where(f => f.FeedBackID == feedbackID).FirstOrDefault();
        }

        public bool AddFeedback(Feedback feedback)
        {
            if (feedback == null)
            {
                return false;
            }
            else
            {
                feedback.CreateTime = DateTime.Now;
                gameClubEntities.Feedback.Add(feedback);
                gameClubEntities.SaveChanges();
                EFUserRecord.AddUserOperateRecord("添加了标题为"+feedback.Title+"的留言反馈");
                return true;
            }
        }

        public bool DelFeedback(int feedbackID)
        {
            if (feedbackID > 0)
            {
                Feedback feedback = gameClubEntities.Feedback.Where(f => f.FeedBackID == feedbackID).FirstOrDefault();
                if (feedback != null)
                {
                    gameClubEntities.Feedback.Remove(feedback);
                    gameClubEntities.SaveChanges();
                    EFUserRecord.AddUserOperateRecord("删除了标题为" + feedback.Title + "的留言反馈");
                    return true;
                }
            }
            return false;
        }

        public bool UpdateFeedback(Feedback feedback)
        {
            if (feedback != null)
            {
                Feedback feedbackResult = gameClubEntities.Feedback.Where(f => f.FeedBackID == feedback.FeedBackID).FirstOrDefault();
                if (feedbackResult != null)
                {
                    feedbackResult.Title = feedback.Title;
                    feedbackResult.Context = feedback.Context;
                    feedbackResult.Reply = feedback.Reply;
                    gameClubEntities.SaveChanges();
                    EFUserRecord.AddUserOperateRecord("更新了标题为" + feedback.Title + "的留言反馈");
                    return true;
                }
            }
            return false;
        }
    }
}