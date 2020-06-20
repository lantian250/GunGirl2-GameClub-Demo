using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GameClub.Models;
using GameClub.Abstract;

namespace GameClub.Concrete
{
    public class EFQuestionary : IQuestionary
    {
        GameClubEntities gameClubEntities = new GameClubEntities();
        EFUserRecord EFUserRecord = new EFUserRecord();
        public IEnumerable<Questionary> Questionaries => gameClubEntities.Questionary;

        public IEnumerable<Question> Questions => gameClubEntities.Question;

        public IEnumerable<Select> Selects => gameClubEntities.Select;

        public IEnumerable<FillOut> FillOuts => gameClubEntities.FillOut;

        public bool AddFillOut(List<FillOut> fillOuts)
        {
            if (fillOuts.Count > 0)
            {
                foreach (var item in fillOuts)
                {
                    FillOut fillOut = new FillOut
                    {
                        GameID = item.GameID,
                        QuestionaryID = item.QuestionaryID,
                        QuestionID = item.QuestionID,
                        SelectID = item.SelectID,
                        Value = item.Value
                    };
                    gameClubEntities.FillOut.Add(fillOut);
                }
                gameClubEntities.SaveChanges();
                return true;
            }
            return false;
        }

        public bool AddQuestion(Question question)
        {
            if (question == null)
            {
                return false;
            }
            Question questionResult = gameClubEntities.Question.Where(q => q.QuestionID == question.QuestionID).FirstOrDefault();
            if (questionResult == null)
            {
                gameClubEntities.Question.Add(question);
                gameClubEntities.SaveChanges();
                return true;
            }
            return false;
        }

        public bool AddQuestionary(Questionary questionary)
        {
            if (questionary == null)
            {
                return false;
            }
            Questionary questionaryResult = gameClubEntities.Questionary.Where(q => q.QuestionaryID == questionary.QuestionaryID).FirstOrDefault();
            if (questionaryResult == null)
            {
                questionary.CreateTime = DateTime.Now;
                gameClubEntities.Questionary.Add(questionary);
                gameClubEntities.SaveChanges();
                EFUserRecord.AddUserOperateRecord("添加了ID为"+ questionary.QuestionaryID +"的问卷调查");
                return true;
            }
            return false;
        }

        public bool AddSelect(Select select)
        {
            if (select == null)
            {
                return false;
            }
            Select selectResult = gameClubEntities.Select.Where(s => s.SelectID == select.SelectID).FirstOrDefault();
            if (selectResult == null)
            {
                gameClubEntities.Select.Add(select);
                gameClubEntities.SaveChanges();
                return true;
            }
            return false;
        }

        public bool DelFillOut(string questionaryID, int GameID)
        {
            if (!string.IsNullOrEmpty(questionaryID) && GameID > 0)
            {
                foreach (var item in gameClubEntities.FillOut.Where(f=>f.QuestionaryID==questionaryID&&f.GameID==GameID))
                {
                    gameClubEntities.FillOut.Remove(item);
                }
                gameClubEntities.SaveChanges();
                return true;
            }
            return false;
        }

        public bool DelQuestion(string questionID)
        {
            if (string.IsNullOrEmpty(questionID))
            {
                return false;
            }
            Question question = gameClubEntities.Question.Where(q => q.QuestionID == questionID).FirstOrDefault();
            if (question != null)
            {
                foreach (var item in gameClubEntities.Select.Where(s => s.QuestionID == question.QuestionID))
                {
                    gameClubEntities.Select.Remove(item);
                }
                foreach (var item in gameClubEntities.FillOut.Where(f => f.QuestionaryID == question.QuestionID))
                {
                    gameClubEntities.FillOut.Remove(item);
                }
                gameClubEntities.Question.Remove(question);
                gameClubEntities.SaveChanges();
                return true;
            }
            return false;
        }

        public bool DelQuestionary(string questionaryID)
        {
            if (!string.IsNullOrEmpty(questionaryID))
            {
                Questionary questionaryResult = gameClubEntities.Questionary.Where(q => q.QuestionaryID == questionaryID).FirstOrDefault();
                if (questionaryResult != null)
                {
                    foreach (var item in gameClubEntities.Question.Where(q=>q.QuestionaryID==questionaryResult.QuestionaryID))
                    {
                        foreach (var items in gameClubEntities.Select.Where(s=>s.QuestionID==item.QuestionID))
                        {
                            gameClubEntities.Select.Remove(items);
                        }
                        gameClubEntities.Question.Remove(item);
                    }
                    foreach (var item in gameClubEntities.FillOut.Where(f => f.QuestionaryID == questionaryResult.QuestionaryID))
                    {
                        gameClubEntities.FillOut.Remove(item);
                    }
                    gameClubEntities.Questionary.Remove(questionaryResult);
                    gameClubEntities.SaveChanges();
                    EFUserRecord.AddUserOperateRecord("删除了ID为" + questionaryResult.QuestionaryID + "的问卷调查");
                    return true;
                }
            }
            return false;
        }

        public bool DelSelect(string selectID)
        {
            if (string.IsNullOrEmpty(selectID))
            {
                return false;
            }
            Select select = gameClubEntities.Select.Where(s => s.SelectID == selectID).FirstOrDefault();
            if (select != null)
            {
                foreach (var item in gameClubEntities.FillOut.Where(f => f.SelectID == select.SelectID))
                {
                    gameClubEntities.FillOut.Remove(item);
                }
                gameClubEntities.Select.Remove(select);
                gameClubEntities.SaveChanges();
                return true;
            }
            return false;
        }

        public Question Question(string questionId)
        {
            return gameClubEntities.Question.Where(q => q.QuestionID == questionId).FirstOrDefault();
        }

        public Questionary Questionary(string questionaryID)
        {
            return gameClubEntities.Questionary.Where(q => q.QuestionaryID== questionaryID).FirstOrDefault();
        }

        public Select Select(string selectID)
        {
            return gameClubEntities.Select.Where(q => q.SelectID== selectID).FirstOrDefault();
        }

        public bool UpdateQuestion(Question question)
        {
            if (question == null)
            {
                return false;
            }
            Question questionResult = gameClubEntities.Question.Where(q => q.QuestionID == question.QuestionID).FirstOrDefault();
            if (questionResult != null)
            {
                questionResult.QuestionContext = questionResult.QuestionContext;
                questionResult.Type = question.Type;
                gameClubEntities.SaveChanges();
                return true;
            }
            return false;
        }

        public bool UpdateQuestionary(Questionary questionary)
        {
            if (questionary == null)
            {
                return false;
            }
            Questionary questionaryResult = gameClubEntities.Questionary.Where(q => q.QuestionaryID == questionary.QuestionaryID).FirstOrDefault();
            if (questionaryResult != null)
            {
                questionaryResult.Title = questionary.Title;
                questionaryResult.Context = questionary.Context;
                questionaryResult.StartTime = questionary.StartTime;
                questionaryResult.EndTime = questionary.EndTime;
                gameClubEntities.SaveChanges();
            }
            return false;
        }

        public bool UpdateSelect(Select select)
        {
            if (select == null)
            {
                return false;
            }
            Select selectResult = gameClubEntities.Select.Where(s => s.SelectID == select.SelectID).FirstOrDefault();
            if (selectResult != null)
            {
                selectResult.Value = select.Value;
                gameClubEntities.SaveChanges();
            }
            return false;
        }
    }
}