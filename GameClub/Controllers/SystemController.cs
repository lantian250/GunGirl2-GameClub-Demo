using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GameClub.Models;
using GameClub.Abstract;
using Webdiyer.WebControls.Mvc;
using GameClub.Filters;

namespace GameClub.Controllers
{
    [User(null)]
    public class SystemController : Controller
    {
        IUserRecord EFUserRecord;
        IRecover EFRecover;
        public SystemController(IUserRecord userRecord,IRecover recover)
        {
            EFUserRecord = userRecord;
            EFRecover = recover;
        }

        public ActionResult UserLoginRecord(int PageIndex=1)
        {
            DealViewBag("UserLoginRecord");
            var model = EFUserRecord.UserLoginRecords.OrderByDescending(u => u.ID).ToPagedList(PageIndex, 20);
            if (Request.IsAjaxRequest())
            {
                return PartialView("_UserLoginRecord", model);
            }
            return View(model);
        }

        public ActionResult UserOperateRecord(int PageIndex = 1,string keyword=null)
        {
            DealViewBag("UserOperateRecord");
            var model = EFUserRecord.UserOperateRecords.OrderByDescending(u => u.ID).ToPagedList(PageIndex, 20);
            if(keyword!=null)
            {
                model=EFUserRecord.UserOperateRecords.Where(u=>u.OperateContext.ToString().Contains(keyword)||u.UserID.ToString().Contains(keyword)||u.UserName.ToString().Contains(keyword)).OrderByDescending(u => u.ID).ToPagedList(PageIndex, 20);
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView("_UserOperateRecord", model);
            }
            return View(model);
        }

        public ActionResult UserDel(int PageIndex = 1)
        {
            ViewBag.PageIndex = PageIndex;
            DealViewBag("UserDel");
            var model = EFRecover.UserInfos.ToPagedList(PageIndex, 20);
            if (Request.IsAjaxRequest())
            {
                return PartialView("_UserDel", model);
            }
            return View(model);
        }

        public ActionResult DelUser(int UserID, int PageIndex=1)
        {
            ViewBag.PageIndex = PageIndex;
            if (EFRecover.DelUserInfo(UserID))
            {
                var model = EFRecover.UserInfos.ToPagedList(PageIndex, 20);
                return PartialView("_UserDel", model);
            }
            return View();
        }
        public ActionResult ResetUser(int UserID, int PageIndex = 1)
        {
            ViewBag.PageIndex = PageIndex;
            if (EFRecover.ResetUserInfo(UserID))
            {
                var model = EFRecover.UserInfos.ToPagedList(PageIndex, 20);
                return PartialView("_UserDel", model);
            }
            return View();
        }

        public ActionResult GameMemberDel(int PageIndex = 1)
        {
            DealViewBag("GameMemberDel");
            ViewBag.PageIndex = PageIndex;
            var model = EFRecover.GameMembers.ToPagedList(PageIndex, 20);
            if (Request.IsAjaxRequest())
            {
                return View("_GameMemberDel", model);
            }
            return View(model);
        }

        public ActionResult DelGameMember(int GameID,int PageIndex=1)
        {
            ViewBag.PageIndex = PageIndex;
            if (EFRecover.DelGameMember(GameID))
            {
                var model = EFRecover.GameMembers.ToPagedList(PageIndex, 20);
                return PartialView("_GameMemberDel", model);
            }
            return View();
        }
        public ActionResult ResetGameMember(int GameID, int PageIndex = 1)
        {
            ViewBag.PageIndex = PageIndex;
            if (EFRecover.ResetGameMember(GameID))
            {
                var model = EFRecover.GameMembers.ToPagedList(PageIndex, 20);
                return PartialView("_GameMemberDel", model);
            }
            return View();
        }
        public void DealViewBag(string setMenu)
        {
            ViewBag.System = "active open";
            if (setMenu.Equals("UserLoginRecord"))
            {
                ViewBag.UserLoginRecord = "active";
            }
            else if (setMenu.Equals("UserOperateRecord"))
            {
                ViewBag.UserOperateRecord = "active";
            }
            else if (setMenu.Equals("UserDel"))
            {
                ViewBag.Recover = "active open";
                ViewBag.UserDel = "active";
            }
            else if (setMenu.Equals("GameMemberDel"))
            {
                ViewBag.Recover = "active open";
                ViewBag.GameMemberDel = "active";
            }
        }
    }
}