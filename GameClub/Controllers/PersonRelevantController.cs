using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GameClub.Filters;
using GameClub.Models;
using Webdiyer.WebControls.Mvc;
using GameClub.Abstract;
using GameClub.ViewModels;

namespace GameClub.Controllers
{
    [User(null)]
    public class PersonRelevantController : Controller
    {
        static int UserID;
        static int GameID;
        static string UserName;
        QuestionaryViewModel QuestionaryViewModel;
        QuestionViewModel QuestionViewModel;
        IPerson EFPerson;
        IFeedBack EFFeedBack;
        IQuestionary EFQuestionary;
        public PersonRelevantController(HttpContext httpContext, IPerson person, IFeedBack feedBack, IQuestionary questionary)
        {
            if (httpContext.Session["UserID"] != null)
            {
                UserID = Convert.ToInt32(httpContext.Session["UserID"].ToString());
                GameID = Convert.ToInt32(httpContext.Session["UserID"].ToString());
                UserName = httpContext.Session["UserName"].ToString();
            }
            EFPerson = person;
            EFFeedBack = feedBack;
            EFQuestionary = questionary;
        }

        public ActionResult FeedBack(int PageIndex = 1, string keyword = null)
        {
            DealViewBag("FeedBack");
            FeedBackViewModel feedBackViewModel = new FeedBackViewModel
            {
                PageIndex = PageIndex,
            };
            if (!string.IsNullOrEmpty(keyword))
            {
                feedBackViewModel.Feedbacks = EFPerson.Feedbacks(UserID).Where(f => f.Title.ToString().Contains(keyword) || f.Context.ToString().Contains(keyword)).ToPagedList(PageIndex, 10);
            }
            else
            {
                feedBackViewModel.Feedbacks = EFPerson.Feedbacks(UserID).ToPagedList(PageIndex, 10);
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView("_FeedBack", feedBackViewModel);
            }
            return View(feedBackViewModel);
        }

        public ActionResult AddFeedBack(string Title, string Context)
        {
            if (!string.IsNullOrEmpty(Title) && !string.IsNullOrEmpty(Context))
            {
                Feedback feedback = new Feedback
                {
                    UserID = UserID,
                    UserName = UserName,
                    Title = Title,
                    Context = Context,
                };
                if (EFFeedBack.AddFeedback(feedback))
                {
                    FeedBackViewModel feedBackViewModel = new FeedBackViewModel
                    {
                        PageIndex = 1,
                        Feedbacks = EFPerson.Feedbacks(UserID).ToPagedList(1, 10)
                    };
                    return PartialView("_FeedBack", feedBackViewModel);
                }
            }
            return View(false);
        }

        public ActionResult DelFeedBack(int FeedBackID)
        {
            if (FeedBackID > 0)
            {
                if (EFFeedBack.DelFeedback(FeedBackID))
                {
                    FeedBackViewModel feedBackViewModel = new FeedBackViewModel
                    {
                        PageIndex = 1,
                        Feedbacks = EFPerson.Feedbacks(UserID).ToPagedList(1, 10)
                    };
                    return PartialView("_FeedBack", feedBackViewModel);
                }
            }
            return View(false);
        }

        public ActionResult Questionary(int PageIndex = 1)
        {
            DealViewBag("Questionary");
            QuestionaryViewModel questionaryViewModel = GetQuestionaryViewModel(PageIndex);
            if (Request.IsAjaxRequest())
            {
                return PartialView("_Questionary", QuestionaryViewModel);
            }
            return View(QuestionaryViewModel);
        }

        public ActionResult Question(string QuestionaryID)
        {
            QuestionViewModel = GetQuestionViewModel(QuestionaryID);
            if (Request.IsAjaxRequest())
            {
                return PartialView("_Questions", QuestionViewModel);
            }
            return View(QuestionViewModel);
        }

        public ActionResult AddFillOut(string QuestionaryID, FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(QuestionaryID))
            {
                List<FillOut> fillOuts = new List<FillOut>();
                EFQuestionary.DelFillOut(QuestionaryID, UserID);
                IEnumerable<Question> questions = EFQuestionary.Questions.Where(q => q.QuestionaryID == QuestionaryID);
                foreach (var item in questions)
                {
                    foreach (var items in EFQuestionary.Selects.Where(q => q.QuestionID == item.QuestionID))
                    {
                        if (item.Type)
                        {
                            if (formCollection[items.QuestionID + items.SelectID] != null)
                            {
                                FillOut fillOut = new FillOut
                                {
                                    GameID = UserID,
                                    QuestionaryID = QuestionaryID,
                                    QuestionID = items.QuestionID,
                                    SelectID = items.SelectID,
                                };
                                if (items.Type)
                                {
                                    if (!string.IsNullOrEmpty(formCollection[items.QuestionID + items.SelectID + "input"].ToString()))
                                    {
                                        fillOut.Value = formCollection[items.QuestionID + items.SelectID + "input"].ToString();
                                    }
                                    else
                                    {
                                        fillOut.Value = "未填写";
                                    }
                                }
                                else
                                {
                                    fillOut.Value = items.Value;
                                }
                                fillOuts.Add(fillOut);
                            }
                        }
                        else
                        {
                            string SelectID;
                            if (formCollection[items.QuestionID] != null)
                            {
                                 SelectID= formCollection[items.QuestionID].ToString();
                            }
                            else
                            {
                                SelectID = null;
                            }
                            if (SelectID != null&&SelectID.Equals(items.SelectID))
                            {
                                FillOut fillOut = new FillOut
                                {
                                    GameID = UserID,
                                    QuestionaryID = QuestionaryID,
                                    QuestionID = items.QuestionID,
                                    SelectID = formCollection[items.QuestionID].ToString(),
                                };
                                if (items.Type)
                                {
                                    if (!string.IsNullOrEmpty(formCollection[items.QuestionID + items.SelectID + "input"].ToString()))
                                    {
                                        fillOut.Value = formCollection[items.QuestionID + items.SelectID + "input"].ToString();
                                    }
                                    else
                                    {
                                        fillOut.Value = "未填写";
                                    }
                                }
                                else
                                {
                                    fillOut.Value = items.Value;
                                }
                                fillOuts.Add(fillOut);
                            }
                        }
                    }
                }
                EFQuestionary.AddFillOut(fillOuts);
                QuestionViewModel = GetQuestionViewModel(QuestionaryID);
                if (Request.IsAjaxRequest())
                {
                    return PartialView("_Questions", QuestionViewModel);
                }
                return View(QuestionViewModel);
            }
            return View(false);
        }



        public ActionResult Article(int PageIndex = 1, string keyword = null)
        {
            DealViewBag("Article");
            var articles = EFPerson.Articles.OrderByDescending(a => a.ArticleID).ToPagedList(PageIndex, 10);
            if (!string.IsNullOrEmpty(keyword))
            {
                articles = EFPerson.Articles.Where(a => a.Title.ToString().Contains(keyword)).OrderByDescending(a => a.ArticleID).ToPagedList(PageIndex, 20);
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView("_Article", articles);
            }
            return View(articles);
        }
        public ActionResult ShowArticle(int ArticleID)
        {
            if (ArticleID > 0)
            {
                Article article = EFPerson.Articles.Where(a => a.ArticleID == ArticleID).FirstOrDefault();
                article.Context = Server.HtmlDecode(article.Context);
                return View(article);
            }
            return View();
        }







        public QuestionaryViewModel GetQuestionaryViewModel(int PageIndex)
        {
            QuestionaryViewModel = new QuestionaryViewModel
            {
                Questionaries = EFQuestionary.Questionaries.OrderByDescending(q => q.QuestionaryID).ToPagedList(PageIndex, 10),
                Questions = EFQuestionary.Questions,
                Selects = EFQuestionary.Selects,
                FillOuts = EFQuestionary.FillOuts.Where(f=>f.GameID==GameID),
                PageIndex = PageIndex,
            };
            return QuestionaryViewModel;
        }

        public QuestionViewModel GetQuestionViewModel(string QuestionaryID)
        {
            QuestionViewModel = new QuestionViewModel
            {
                Questionary = EFQuestionary.Questionaries.Where(q => q.QuestionaryID == QuestionaryID).FirstOrDefault(),
                Questions = EFQuestionary.Questions.Where(q => q.QuestionaryID == QuestionaryID),
                Selects = EFQuestionary.Selects,
                FillOuts = EFQuestionary.FillOuts.Where(f => f.GameID == GameID),
                QuestionaryID = QuestionaryID,
            };
            return QuestionViewModel;
        }
        public QuestionViewModel UpdateQuestionViewModel(QuestionViewModel questionViewModel)
        {
            questionViewModel.Questionary = EFQuestionary.Questionaries.Where(q => q.QuestionaryID == questionViewModel.QuestionaryID).FirstOrDefault();
            questionViewModel.Questions = EFQuestionary.Questions.Where(q => q.QuestionaryID == questionViewModel.QuestionaryID);
            questionViewModel.Selects = EFQuestionary.Selects;
            return questionViewModel;
        }

        public void DealViewBag(string setMenu)
        {
            ViewBag.PersonRelevant = "active open";
            if (setMenu.Equals("FeedBack"))
            {
                ViewBag.FeedBack = "active";
            }
            else if (setMenu.Equals("Questionary"))
            {
                ViewBag.Questionary = "active";
            }
            else if (setMenu.Equals("Article"))
            {
                ViewBag.Article = "active";
            }
        }
    }
}