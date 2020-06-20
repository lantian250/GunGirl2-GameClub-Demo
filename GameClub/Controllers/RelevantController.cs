using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;
using GameClub.Models;
using GameClub.Abstract;
using GameClub.ViewModels;
using GameClub.Filters;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System.IO;

namespace GameClub.Controllers
{
    [User("RelevantManage")]
    public class RelevantController : Controller
    {
        IInformMessage EFInformMessage;
        IQuestionary EFQuestionary;
        IArticle EFArticle;
        IFeedBack EFFeedBack;
        IGameMember EFGameMember;
        InformMessageViewModel InformMessageViewModel;
        QuestionaryViewModel QuestionaryViewModel;
        QuestionViewModel QuestionViewModel;
        ArticleViewModel ArticleViewModel;
        public RelevantController(IGameMember gameMember, IInformMessage informMessage, IQuestionary questionary, IArticle article, IFeedBack feedBack)
        {
            EFInformMessage = informMessage;
            EFQuestionary = questionary;
            EFArticle = article;
            EFFeedBack = feedBack;
            EFGameMember = gameMember;
        }
        /// <summary>
        /// 社团通告管理
        /// </summary>
        /// <param name="PageIndex"></param>
        /// <param name="Keyword"></param>
        /// <param name="IsDesc"></param>
        /// <returns></returns>
        public ActionResult InformMessage(int PageIndex = 1, string Keyword = null, bool IsDesc = true)
        {
            DealViewBag("InformMessage");
            InformMessageViewModel = GetInformMessageViewModel(PageIndex, Keyword, IsDesc);
            if (Request.IsAjaxRequest())
            {
                return PartialView("_Inform", InformMessageViewModel);
            }
            return View(InformMessageViewModel);
        }

        /// <summary>
        /// 添加社团通告
        /// </summary>
        /// <param name="informMessage"></param>
        /// <param name="informMessageViewModel"></param>
        /// <returns></returns>
        public ActionResult AddInformMessage(InformMessage informMessage, InformMessageViewModel informMessageViewModel)
        {
            if (ModelState.IsValid)
            {
                if ((!string.IsNullOrEmpty(informMessage.Context)) && !(string.IsNullOrEmpty(informMessage.Title)))
                {
                    if (EFInformMessage.AddInformMessage(informMessage))
                    {
                        informMessageViewModel = UpdateInformMessageViewModel(informMessageViewModel);
                        return PartialView("_Inform", informMessageViewModel);
                    }
                }
            }
            return View(false);
        }
        /// <summary>
        /// 删除社团通告
        /// </summary>
        /// <param name="informMessage"></param>
        /// <param name="informMessageViewModel"></param>
        /// <returns></returns>
        public ActionResult DelInformMessage(InformMessage informMessage, InformMessageViewModel informMessageViewModel)
        {
            if (ModelState.IsValid)
            {
                if (informMessage.ID != 0)
                {
                    if (EFInformMessage.DelInformMessage(informMessage))
                    {
                        informMessageViewModel = UpdateInformMessageViewModel(informMessageViewModel);
                        return PartialView("_Inform", informMessageViewModel);
                    }
                }
            }
            return View(false);
        }
        /// <summary>
        /// 更新社团通告
        /// </summary>
        /// <param name="informMessage"></param>
        /// <param name="informMessageViewModel"></param>
        /// <returns></returns>
        public ActionResult UpdateInformMessage(InformMessage informMessage, InformMessageViewModel informMessageViewModel)
        {
            if (ModelState.IsValid)
            {
                if ((!string.IsNullOrEmpty(informMessage.Context)) && !(string.IsNullOrEmpty(informMessage.Title)))
                {
                    if (EFInformMessage.UpdateInformMessage(informMessage))
                    {
                        informMessageViewModel = UpdateInformMessageViewModel(informMessageViewModel);
                        return PartialView("_Inform", informMessageViewModel);
                    }
                }
            }
            return View(false);
        }
        /// <summary>
        /// 获取社团通告视图
        /// </summary>
        /// <param name="PageIndex"></param>
        /// <param name="Keyword"></param>
        /// <param name="IsDesc"></param>
        /// <returns></returns>
        public InformMessageViewModel GetInformMessageViewModel(int PageIndex, string Keyword, bool IsDesc)
        {
            InformMessageViewModel = new InformMessageViewModel
            {
                PageIndex = PageIndex,
                Keyword = Keyword,
                IsDesc = IsDesc,
            };
            if (IsDesc)
            {
                InformMessageViewModel.InformMessages = EFInformMessage.SearchInformMessages(Keyword).OrderByDescending(i => i.CreateTime).ToPagedList(PageIndex, 20);
            }
            else
            {
                InformMessageViewModel.InformMessages = EFInformMessage.SearchInformMessages(Keyword).OrderByDescending(i => i.CreateTime).ToPagedList(PageIndex, 20);
            }
            return InformMessageViewModel;
        }
        /// <summary>
        /// 更新社团通告视图
        /// </summary>
        /// <param name="informMessageViewModel"></param>
        /// <returns></returns>
        public InformMessageViewModel UpdateInformMessageViewModel(InformMessageViewModel informMessageViewModel)
        {
            if (informMessageViewModel.IsDesc)
            {
                informMessageViewModel.InformMessages = EFInformMessage.SearchInformMessages(informMessageViewModel.Keyword).OrderByDescending(i => i.CreateTime).ToPagedList(informMessageViewModel.PageIndex, 20);
            }
            else
            {
                informMessageViewModel.InformMessages = EFInformMessage.SearchInformMessages(informMessageViewModel.Keyword).OrderByDescending(i => i.CreateTime).ToPagedList(informMessageViewModel.PageIndex, 20);
            }
            return informMessageViewModel;
        }

        public ActionResult QuestionaryManage(int PageIndex = 1)
        {
            DealViewBag("QuestionaryManage");
            QuestionaryViewModel = GetQuestionaryViewModel(PageIndex);
            if (Request.IsAjaxRequest())
            {
                return PartialView("_Questionary", QuestionaryViewModel);
            }
            return View(QuestionaryViewModel);
        }
        public ActionResult AddQuestionary(Questionary questionary, QuestionaryViewModel questionaryViewModel)
        {
            if (ModelState.IsValid)
            {
                if (questionary.StartTime > questionary.EndTime)
                {
                    return View(false);
                }
                if (questionary.EndTime < DateTime.Now)
                {
                    return View(false);
                }
                if (EFQuestionary.AddQuestionary(questionary))
                {
                    questionaryViewModel = UpdateQuestionaryViewModel(questionaryViewModel);
                    return PartialView("_Questionary", questionaryViewModel);
                }
            }
            return View(false);
        }
        public ActionResult DelQuestionary(string QuestionaryID)
        {
            if (!string.IsNullOrEmpty(QuestionaryID))
            {
                if (EFQuestionary.DelQuestionary(QuestionaryID))
                {
                    QuestionaryViewModel = GetQuestionaryViewModel(1);
                    return PartialView("_Questionary", QuestionaryViewModel);
                }
            }
            return View(false);
        }
        public ActionResult ShowQuestionaryResult(string QuestionaryID)
        {
            if (!string.IsNullOrEmpty(QuestionaryID))
            {
                Questionary questionary = EFQuestionary.Questionaries.Where(q => q.QuestionaryID == QuestionaryID).FirstOrDefault();
                if (questionary != null)
                {
                    QuestionaryViewModel questionaryViewModel = new QuestionaryViewModel
                    {
                        Questionary = questionary,
                        Questions = EFQuestionary.Questions.Where(q => q.QuestionaryID == questionary.QuestionaryID).OrderBy(q => q.QuestionID),
                        Selects = EFQuestionary.Selects,
                        FillOuts = EFQuestionary.FillOuts.Where(f => f.QuestionaryID == questionary.QuestionaryID).OrderBy(s => s.GameID),
                        GameMembers = EFGameMember.gameMembers,
                    };
                    return View(questionaryViewModel);
                }
            }
            return View(false);
        }

        public FileResult ExportQuestionary(string QuestionaryID)
        {
            CellRangeAddress cellRange;
            //创建Excel文件的对象
            HSSFWorkbook xSSFWorkbook = new HSSFWorkbook();
            //添加一个sheet
            ISheet sheet1 = xSSFWorkbook.CreateSheet("Sheet1");
            //获取list数据
            List<Question> questions = EFQuestionary.Questions.Where(q => q.QuestionaryID == QuestionaryID).OrderBy(q => q.QuestionID).ToList();
            List<Select> selects = EFQuestionary.Selects.ToList();
            List<FillOut> fillOuts = EFQuestionary.FillOuts.Where(f => f.QuestionaryID == QuestionaryID).OrderBy(f => f.GameID).ToList();
            List<GameMember> gameMembers = EFGameMember.gameMembers.ToList();
            //给sheet1添加第一行的头部标题
            IRow row1 = sheet1.CreateRow(0);
            row1.CreateCell(0).SetCellValue("游戏ID");
            row1.CreateCell(1).SetCellValue("游戏昵称");
            int count = 2;
            foreach (var item in questions)
            {
                row1.CreateCell(count).SetCellValue(item.QuestionContext);
                cellRange = new CellRangeAddress(0, 0, count, count + selects.Where(s => s.QuestionID == item.QuestionID).Count() - 1);
                sheet1.AddMergedRegion(cellRange);
                count += selects.Where(s => s.QuestionID == item.QuestionID).Count();
            }
            //将数据逐步写入sheet1各个行
            IRow row2 = sheet1.CreateRow(1);
            row2.CreateCell(0).SetCellValue("选项值");
            cellRange = new CellRangeAddress(1, 1, 0, 1);
            sheet1.AddMergedRegion(cellRange);
            count = 2;
            foreach (var item in questions)
            {
                foreach (var items in selects.Where(s => s.QuestionID == item.QuestionID).OrderBy(s => s.SelectID))
                {
                    if (items.Type)
                    {
                        row2.CreateCell(count++).SetCellValue("其他");
                    }
                    else
                    {
                        row2.CreateCell(count++).SetCellValue(items.Value);
                    }
                }
            }
            count = 2;
            int counts;
            foreach (var GameID in fillOuts.Select(f => f.GameID).Distinct())
            {
                counts = 0;
                IRow row = sheet1.CreateRow(count++);
                row.CreateCell(counts++).SetCellValue(GameID);
                row.CreateCell(counts++).SetCellValue(gameMembers.Where(g => g.GameID == GameID).FirstOrDefault().GameName.ToString());
                foreach (var item in questions)
                {
                    foreach (var items in selects.Where(s => s.QuestionID == item.QuestionID).OrderBy(s => s.SelectID))
                    {
                        FillOut fillOut=fillOuts.Where(f => f.GameID == GameID && f.SelectID == items.SelectID).FirstOrDefault();
                        if (fillOut != null)
                        {
                            if (items.Type)
                            {
                                row.CreateCell(counts++).SetCellValue(fillOut.Value);
                            }
                            else
                            {
                                row.CreateCell(counts++).SetCellValue("√");
                            }
                        }
                        else
                        {
                            row.CreateCell(counts++).SetCellValue("×");
                        }
                    }
                }
            }

            // 写入到客户端 
            MemoryStream ms = new MemoryStream();
            xSSFWorkbook.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/vnd.ms-excel", "编号" + QuestionaryID + "调查问卷结果.xls");
        }

        public ActionResult QuestionManage(string QuestionaryID)
        {
            QuestionViewModel = GetQuestionViewModel(QuestionaryID);
            if (Request.IsAjaxRequest())
            {
                return PartialView("_Questions", QuestionViewModel);
            }
            return View(QuestionViewModel);
        }

        public ActionResult AddQuestion(string QuestionContext, bool Type, string QuestionaryID)
        {
            if (string.IsNullOrEmpty(QuestionContext))
            {
                return View(false);
            }
            Question question = new Question
            {
                QuestionContext = QuestionContext,
                Type = Type,
            };
            question.QuestionaryID = QuestionaryID;
            if (EFQuestionary.Questions.Where(q => q.QuestionaryID == QuestionaryID).Count() == 0)
            {
                question.QuestionID = QuestionaryID + (EFQuestionary.Questions.Where(q => q.QuestionaryID == QuestionaryID).Count() + 1);
            }
            else
            {
                int count;
                Question questionResult = EFQuestionary.Questions.Where(q => q.QuestionaryID == QuestionaryID).OrderByDescending(q => q.QuestionID).FirstOrDefault();
                string QuestionID = questionResult.QuestionID;
                count = Convert.ToInt32(QuestionID.Substring(QuestionaryID.Length, (QuestionID.Length - QuestionaryID.Length)));
                question.QuestionID = QuestionaryID + (count + 1);
                //if (QuestionID.Length <=15)
                //{
                //    count=Convert.ToInt32(QuestionID.Substring(QuestionID.Length - 1,1));
                //    count++;
                //    question.QuestionID = QuestionID.Substring(0, QuestionID.Length - 1) + count;
                //}
                //else
                //{
                //    count = Convert.ToInt32(QuestionID.Substring(QuestionID.Length - 2, 2));
                //    count++;
                //    question.QuestionID = QuestionID.Substring(0, QuestionID.Length - 2) + count;
                //}
            }
            if (EFQuestionary.AddQuestion(question))
            {
                QuestionViewModel = GetQuestionViewModel(QuestionaryID);
                return PartialView("_Questions", QuestionViewModel);
            }
            return View(false);
        }

        public ActionResult DelQuestion(string QuestionID)
        {
            if (!string.IsNullOrEmpty(QuestionID))
            {
                Question question = EFQuestionary.Questions.Where(q => q.QuestionID == QuestionID).FirstOrDefault();
                if (question != null)
                {
                    string QuestionaryID = question.QuestionaryID;
                    if (EFQuestionary.DelQuestion(question.QuestionID))
                    {
                        QuestionViewModel = GetQuestionViewModel(QuestionaryID);
                        return PartialView("_Questions", QuestionViewModel);
                    }
                }
            }
            return View(false);
        }

        public ActionResult AddSelect(string QuestionaryID, string QuestionID, bool Type, string SelectValue)
        {
            if (!Type)
            {
                if (string.IsNullOrEmpty(SelectValue))
                {
                    return View(false);
                }
            }
            if (QuestionID != null)
            {
                Select select = new Select()
                {
                    QuestionID = QuestionID,
                };
                int count;
                if (EFQuestionary.Selects.Where(s => s.QuestionID == QuestionID).Count() == 0)
                {
                    select.SelectID = QuestionID + 1;
                }
                else
                {
                    Select selectResult = EFQuestionary.Selects.Where(s => s.QuestionID == QuestionID).OrderByDescending(s => s.SelectID).FirstOrDefault();
                    string SelectID = selectResult.SelectID;
                    count = Convert.ToInt32(SelectID.Substring(QuestionID.Length, (SelectID.Length - QuestionID.Length)));
                    select.SelectID = QuestionID + (count + 1);
                }
                if (Type)
                {
                    select.Type = true;
                }
                else if (!string.IsNullOrEmpty(SelectValue))
                {
                    select.Type = false;
                    select.Value = SelectValue;
                }
                if (EFQuestionary.AddSelect(select))
                {
                    QuestionViewModel = GetQuestionViewModel(QuestionaryID);
                    return PartialView("_Questions", QuestionViewModel);
                }
            }
            return View(false);
        }

        public ActionResult DelSelect(string QuestionaryID, string SelectID)
        {
            if (!string.IsNullOrEmpty(SelectID))
            {
                if (EFQuestionary.DelSelect(SelectID))
                {
                    QuestionViewModel = GetQuestionViewModel(QuestionaryID);
                    return PartialView("_Questions", QuestionViewModel);
                }
            }
            return View(false);
        }


        public ActionResult ArticleManage(int PageIndex = 1, string keyword = null)
        {
            DealViewBag("ArticleManage");
            ArticleViewModel = new ArticleViewModel
            {
                PageIndex = PageIndex
            };
            if (!string.IsNullOrEmpty(keyword))
            {
                ArticleViewModel.ArticlePagedList = EFArticle.Articles.Where(a => a.Title.ToString().Contains(keyword)).OrderByDescending(a => a.ArticleID).ToPagedList(PageIndex, 10);
            }
            else
            {
                ArticleViewModel.ArticlePagedList = EFArticle.Articles.OrderByDescending(a => a.ArticleID).ToPagedList(PageIndex, 10);
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView("_Article", ArticleViewModel);
            }
            return View(ArticleViewModel);
        }

        public ActionResult AddArticle()
        {
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddArticle(string Context, string title)
        {
            if (!string.IsNullOrEmpty(Context) && !string.IsNullOrEmpty(title))
            {
                Article article = new Article
                {
                    Title = title,
                    Context = Server.HtmlEncode(Context),
                };
                if (EFArticle.AddArticle(article))
                {
                    return RedirectToAction("ArticleManage");
                }
            }
            return View();
        }

        public ActionResult DelArticle(int ArticleID)
        {
            if (ArticleID > 0)
            {
                if (EFArticle.DelArticle(ArticleID))
                {
                    return RedirectToAction("ArticleManage");
                }
            }
            return View();
        }
        [HttpGet]
        public ActionResult EditArticle(int ArticleID)
        {
            if (ArticleID > 0)
            {
                Article article = EFArticle.Articles.Where(a => a.ArticleID == ArticleID).FirstOrDefault();
                if (article != null)
                {
                    ArticleViewModel = new ArticleViewModel
                    {
                        Article = article,
                    };
                    ArticleViewModel.Article.Context = Server.HtmlDecode(ArticleViewModel.Article.Context);
                    return View(ArticleViewModel);
                }
            }
            return View();
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult EditArticle(int ArticleID, string Context, string Title)
        {
            if (!string.IsNullOrEmpty(Title) && !string.IsNullOrEmpty(Context))
            {
                Article article = new Article
                {
                    ArticleID = ArticleID,
                    Context = Context,
                    Title = Title,
                };
                article.Context = Server.HtmlEncode(article.Context);
                if (EFArticle.UpdateArticle(article))
                {
                    return RedirectToAction("ArticleManage");
                }
            }
            return View();
        }
        public ActionResult ShowArticle(int ArticleID)
        {
            if (ArticleID > 0)
            {
                ArticleViewModel = new ArticleViewModel
                {
                    Article = EFArticle.Articles.Where(a => a.ArticleID == ArticleID).FirstOrDefault(),
                };
                ArticleViewModel.Article.Context = Server.HtmlDecode(ArticleViewModel.Article.Context);
                return View(ArticleViewModel);
            }
            return View();
        }


        public ActionResult FeedBackManage(int PageIndex = 1)
        {
            DealViewBag("FeedBackManage");
            FeedBackViewModel feedBackViewModel = new FeedBackViewModel
            {
                Feedbacks = EFFeedBack.Feedbacks.OrderByDescending(f => f.FeedBackID).ToPagedList(PageIndex, 10),
                PageIndex = PageIndex,
            };
            if (Request.IsAjaxRequest())
            {
                return PartialView("_FeedBack", feedBackViewModel);
            }
            return View(feedBackViewModel);
        }

        public ActionResult DelFeedBack(int FeedBackID, int PageIndex)
        {
            if (FeedBackID > 0)
            {
                if (EFFeedBack.DelFeedback(FeedBackID))
                {
                    FeedBackViewModel feedBackViewModel = new FeedBackViewModel
                    {
                        Feedbacks = EFFeedBack.Feedbacks.OrderByDescending(f => f.FeedBackID).ToPagedList(PageIndex, 10),
                        PageIndex = PageIndex,
                    };
                    return PartialView("_FeedBack", feedBackViewModel);
                }
            }
            return View(false);
        }
        public ActionResult AddReply(int FeedBackID, string Reply, int PageIndex)
        {
            if (FeedBackID > 0 && !string.IsNullOrEmpty(Reply))
            {
                Feedback feedback = EFFeedBack.Feedback(FeedBackID);
                if (feedback != null)
                {
                    feedback.Reply = Reply;
                    if (EFFeedBack.UpdateFeedback(feedback))
                    {
                        FeedBackViewModel feedBackViewModel = new FeedBackViewModel
                        {
                            Feedbacks = EFFeedBack.Feedbacks.OrderByDescending(f => f.FeedBackID).ToPagedList(PageIndex, 10),
                            PageIndex = PageIndex,
                        };
                        return PartialView("_FeedBack", feedBackViewModel);
                    }
                }

            }
            return View(false);
        }

        public QuestionaryViewModel GetQuestionaryViewModel(int PageIndex)
        {
            QuestionaryViewModel = new QuestionaryViewModel
            {
                Questionaries = EFQuestionary.Questionaries.OrderByDescending(q => q.QuestionaryID).ToPagedList(PageIndex, 10),
                Questions = EFQuestionary.Questions,
                Selects = EFQuestionary.Selects,
                FillOuts = EFQuestionary.FillOuts,
                PageIndex = PageIndex,
            };
            return QuestionaryViewModel;
        }
        public QuestionaryViewModel UpdateQuestionaryViewModel(QuestionaryViewModel questionaryViewModel)
        {
            questionaryViewModel.Questionaries = EFQuestionary.Questionaries.OrderByDescending(q => q.QuestionaryID).ToPagedList(questionaryViewModel.PageIndex, 10);
            questionaryViewModel.Questions = EFQuestionary.Questions;
            questionaryViewModel.Selects = EFQuestionary.Selects;
            questionaryViewModel.FillOuts = EFQuestionary.FillOuts;
            return questionaryViewModel;
        }


        public QuestionViewModel GetQuestionViewModel(string QuestionaryID)
        {
            QuestionViewModel = new QuestionViewModel
            {
                Questionary = EFQuestionary.Questionaries.Where(q => q.QuestionaryID == QuestionaryID).FirstOrDefault(),
                Questions = EFQuestionary.Questions.Where(q => q.QuestionaryID == QuestionaryID),
                Selects = EFQuestionary.Selects,
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

        /// <summary>
        /// 处理菜单
        /// </summary>
        /// <param name="setMenu"></param>
        public void DealViewBag(string setMenu)
        {
            ViewBag.Relevant = "active open";
            if (setMenu.Equals("InformMessage"))
            {
                ViewBag.InformMessage = "active";
            }
            else if (setMenu.Equals("QuestionaryManage"))
            {
                ViewBag.QuestionaryManage = "active";
            }
            else if (setMenu.Equals("ArticleManage"))
            {
                ViewBag.ArticleManage = "active";
            }
            else if (setMenu.Equals("FeedBackManage"))
            {
                ViewBag.FeedBackManage = "active";
            }
        }
    }
}