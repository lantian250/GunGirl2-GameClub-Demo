using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GameClub.Abstract;
using Webdiyer.WebControls.Mvc;
using GameClub.Models;
using GameClub.ViewModels;
using GameClub.Filters;

namespace GameClub.Controllers
{
    [User(null)]
    public class PersonActivityController : Controller
    {
        static int GameID = 0;
        IPerson EFPerson;
        ISignInfo EFSign;
        IMemberGroup EFMemberGroup;
        public PersonActivityController(HttpContext httpContext ,IPerson person,ISignInfo signInfo,IMemberGroup memberGroup)
        {
            if (httpContext.Session["UserID"]!=null)
            {
                GameID = Convert.ToInt32(httpContext.Session["UserID"].ToString());
            }
            EFPerson = person;
            EFSign = signInfo;
            EFMemberGroup = memberGroup;
        }
        public ActionResult DoSign()
        {
            DealViewBag("DoSign");
            IEnumerable<SignList> signLists = EFSign.signLists.OrderByDescending(s => s.SignID).Take(3);
            List<SignInfo> signInfos=new List<SignInfo>();
            List<MemberGroup> memberGroups = new List<MemberGroup>();
            foreach (var item in signLists)
            {
                SignInfo signInfo = EFSign.signInfos.Where(s => s.SignID == item.SignID && s.GameID == GameID).FirstOrDefault();
                if (signInfo != null)
                {
                    signInfos.Add(signInfo);
                    MemberGroup memberGroup = EFMemberGroup.MemberGroups.Where(m => m.MemberGroupID == item.MemberGroupID && m.GameID == GameID).FirstOrDefault();
                    if (memberGroup != null)
                    {
                        memberGroups.Add(memberGroup);
                    }
                }
            }
            DoSignViewModel doSignViewModel = new DoSignViewModel
            {
                signInfos = signInfos,
                signLists = signLists,
                MemberGroups=memberGroups,
                
            };
            return View(doSignViewModel);
        }
        public ActionResult UpdateSign(string SignID,string SignCondition, bool IsLeave)
        {
            SignInfo signInfoResult = EFSign.signInfos.Where(s => s.SignID == SignID && s.GameID == GameID).FirstOrDefault();
            if (signInfoResult != null)
            {
                if (IsLeave)
                {
                    signInfoResult.IsLeave = true;
                    signInfoResult.SignDatetime = DateTime.Now;
                }
                else
                {
                    signInfoResult.IsLeave = false;
                    signInfoResult.SignDatetime = DateTime.Now;
                    signInfoResult.SignCondition = SignCondition;
                }
                if (EFSign.UpdateSignInfo(signInfoResult))
                {
                    IEnumerable<SignList> signLists = EFSign.signLists.OrderByDescending(s => s.SignID).Take(3);
                    List<SignInfo> signInfos = new List<SignInfo>();
                    List<MemberGroup> memberGroups = new List<MemberGroup>();
                    foreach (var item in signLists)
                    {
                        SignInfo signInfo = EFSign.signInfos.Where(s => s.SignID == item.SignID && s.GameID == GameID).FirstOrDefault();
                        if (signInfo != null)
                        {
                            signInfos.Add(signInfo);
                            MemberGroup memberGroup = EFMemberGroup.MemberGroups.Where(m => m.MemberGroupID == item.MemberGroupID && m.GameID == GameID).FirstOrDefault();
                            if (memberGroup != null)
                            {
                                memberGroups.Add(memberGroup);
                            }
                        }
                    }
                    DoSignViewModel doSignViewModel = new DoSignViewModel
                    {
                        signInfos = signInfos,
                        signLists = signLists,
                        MemberGroups = memberGroups,
                    };
                    return PartialView("_DoSign", doSignViewModel);
                }
            }
            return View(false);
            
        }
        public ActionResult SignRecord()
        {
            DealViewBag("SignRecord");
            var model = EFPerson.SignInfos(GameID).OrderByDescending(s => s.SignID);
            return View(model);
        }
        public ActionResult ContributionRecord()
        {
            DealViewBag("ContributionRecord");
            var model = EFPerson.Contributions(GameID).OrderByDescending(c => c.ContributionID);
            return View(model);
        }
        public ActionResult MemberGroupRecord()
        {
            DealViewBag("MemberGroupRecord");
            var model = EFPerson.MemberGroups(GameID).OrderByDescending(m => m.MemberGroupID);
            return View(model);
        }

        public ActionResult ShowGameMember(int GameID)
        {
            ShowGameMemberViewModel showGameMemberViewModel = new ShowGameMemberViewModel
            {
                GameMember=EFPerson.GameMember(GameID),
                SignInfos=EFPerson.SignInfos(GameID).OrderByDescending(s=>s.SignID).Take(10),
                Contributions=EFPerson.Contributions(GameID).OrderByDescending(c=>c.ContributionID).Take(10),
                MemberGroups=EFPerson.MemberGroups(GameID).OrderByDescending(m=>m.MemberGroupID).Take(10),
            };
            List<SignList> signLists = new List<SignList>();
            foreach (var item in showGameMemberViewModel.SignInfos)
            {
                signLists.Add(EFSign.signLists.Where(s => s.SignID == item.SignID).FirstOrDefault());
            }
            showGameMemberViewModel.SignLists = signLists;
            return View(showGameMemberViewModel);
        }

        public void DealViewBag(string setMenu)
        {
            ViewBag.PersonActivity = "active open";
            if (setMenu.Equals("DoSign"))
            {
                ViewBag.DoSign = "active";
            }
            else if (setMenu.Equals("SignRecord"))
            {
                ViewBag.SignRecord = "active";
            }
            else if (setMenu.Equals("ContributionRecord"))
            {
                ViewBag.ContributionRecord = "active";
            }
            else if (setMenu.Equals("MemberGroupRecord"))
            {
                ViewBag.MemberGroupRecord = "active";
            }
        }
    }
}