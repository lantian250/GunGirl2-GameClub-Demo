using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GameClub.Abstract;
using GameClub.Models;
using GameClub.ViewModels;
using Webdiyer.WebControls.Mvc;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using GameClub.MyHelper;
using System.Data;
using GameClub.Filters;

namespace GameClub.Controllers
{
    [User("SignManage")]
    public class SignController : Controller
    {
        ISignInfo EFSignInfo;//SignInfo表数据操纵
        IGameMember EFGameMember;
        IContribution EFContribution;
        IMemberGroup EFMemberGroup;
        SignInfoViewModel signInfoViewModel;//签到表视图模型
        SignListViewModel signListViewModel;//签到信息视图模型

        public SignController(ISignInfo signInfo,IGameMember gameMember,IContribution contribution,IMemberGroup memberGroup)
        {
            EFSignInfo = signInfo;
            EFGameMember = gameMember;
            EFContribution = contribution;
            EFMemberGroup = memberGroup;
        }
        /// <summary>
        /// 签到表管理
        /// </summary>
        /// <param name="pageID"></param>
        /// <param name="pageSize"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public ActionResult SignListManage(int pageID=1,int pageSize=20,string keyword=null,string sortBy=null,string currentSort=null,bool isOrderBy=false)
        {
            DealViewBag("SignList");
            signListViewModel = GetSignListViewModel(pageID, pageSize, keyword, sortBy, currentSort, isOrderBy);
            if (Request.IsAjaxRequest())
            {
                return PartialView("_SignList", signListViewModel);
            }  
            return View(signListViewModel);
        }
        /// <summary>
        /// 签到表搜索结果
        /// </summary>
        /// <param name="pageID"></param>
        /// <param name="pageSize"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public ActionResult SearchSignList(int pageID = 1, int pageSize = 20, string keyword = null, string sortBy = null, string currentSort = null, bool isOrderBy = false)
        {
            DealViewBag("SignList");
            signListViewModel = GetSignListViewModel(pageID, pageSize, keyword, sortBy, currentSort, isOrderBy);
            return PartialView("_SignList", signListViewModel);
        }

        public ActionResult GetAddSignList(SignListViewModel signListViewModel)
        {
            signListViewModel.ContributionLists = EFContribution.ContributionLists.Where(c => c.SignID == null).OrderByDescending(c => c.ContributionID).Take(10).ToList();
            signListViewModel.MemberGroupLists = EFMemberGroup.MemberGroupLists.Where(m => m.SignID == null).OrderByDescending(m => m.MemberGroupID).Take(10).ToList();
            return PartialView("_SignListAdd", signListViewModel);
        }

        /// <summary>
        /// 添加签到表
        /// </summary>
        /// <param name="signList"></param>
        /// <param name="signListViewModel"></param>
        /// <returns></returns>
        public ActionResult AddSignList(SignList signList,SignListViewModel signListViewModel,bool CreateContribution=false,bool CreateMemberGroup = false)
        {
            DealViewBag("SignList");
            bool f = true;
            if (ModelState.IsValid)
            {
                if (signList.EndDateTime < signList.StartDateTime)
                {
                    ViewBag.Fault = "结束时间不能小于开始时间!";
                    f = false;
                }
                if (f&&signList.Active == true)
                {
                    if (signList.StartDateTime < DateTime.Now && signList.EndDateTime < DateTime.Now)
                    {
                        ViewBag.Fault = "选定的时间起止范围请不要小于当前时间，除非把该表设为禁用状态 !";
                        f = false;
                    }
                }
                if (f&&!string.IsNullOrEmpty(signList.ContributionID))
                {
                    if (CreateContribution)
                    {
                        ViewBag.Fault = "不要同时选择和创建贡献表!";
                        f = false;
                    }
                }
                else
                {
                    if (CreateContribution)
                    {
                        signList.ContributionID = signList.SignID;
                    }
                }
                if (f&&!string.IsNullOrEmpty(signList.MemberGroupID))
                {
                    if (CreateMemberGroup)
                    {
                        ViewBag.Fault = "不要同时选择和创建分组表!";
                        f = false;
                    }
                }
                else
                {
                    if (CreateMemberGroup)
                    {
                        signList.MemberGroupID = signList.SignID;
                    }
                }
                if (f)
                {
                    signList.CreateTime = DateTime.Now;
                    if (!string.IsNullOrEmpty(signList.ContributionID))
                    {
                        ContributionList contributionList = new ContributionList
                        {
                            ContributionID = signList.ContributionID,
                            Type = signList.Type,
                            CreateDateTime = signList.CreateTime,
                        };
                        EFContribution.AddContributionList(contributionList);
                    }
                    if (!string.IsNullOrEmpty(signList.MemberGroupID))
                    {
                        MemberGroupList memberGroupList = new MemberGroupList
                        {
                            MemberGroupID = signList.MemberGroupID,
                            Type = signList.Type,
                            CreateDateTime = signList.CreateTime,
                        };
                        EFMemberGroup.AddMemberGroupList(memberGroupList);
                    }
                    if (EFSignInfo.AddSignList(signList))
                    {
                        if (signList.ContributionID != null)
                        {
                            ContributionList contributionListResult = EFContribution.ContributionList(signList.ContributionID);
                            ContributionList contributionList = new ContributionList
                            {
                                ContributionID = contributionListResult.ContributionID,
                                Type = contributionListResult.Type,
                                CreateDateTime = contributionListResult.CreateDateTime,
                                SignID = signList.SignID,
                                MemberGroupID = signList.MemberGroupID
                            };
                            EFContribution.UpdateContributionList(contributionList);
                        }
                        if (signList.MemberGroupID != null)
                        {
                            MemberGroupList memberGroupListResult = EFMemberGroup.MemberGroupList(signList.MemberGroupID);
                            MemberGroupList memberGroupList = new MemberGroupList
                            {
                                MemberGroupID = memberGroupListResult.MemberGroupID,
                                Type = memberGroupListResult.Type,
                                CreateDateTime = memberGroupListResult.CreateDateTime,
                                SignID = signList.SignID,
                                ContributionID = signList.ContributionID,
                            };
                            EFMemberGroup.UpdateMemberGroupList(memberGroupList);
                        }
                        signListViewModel = UpdateSignListViewModel(signListViewModel);
                        ViewBag.Success = "添加签到表成功！";
                    }
                    else
                    {
                        ViewBag.Fault = "添加签到表失败!";
                        f = false;
                    }
                }
                
            }
            else
            {
                ViewBag.Fault = "输入数据有误!";
            }
            if (!f)
            {
                signListViewModel = UpdateSignListViewModel(signListViewModel);
            }
            return PartialView("_SignList", signListViewModel);

        }
        /// <summary>
        /// 获取某个签到表信息
        /// </summary>
        /// <param name="SignID"></param>
        /// <returns></returns>
        public ActionResult GetSignList(string SignID,SignListViewModel signListViewModel)
        {
            SignList signList = EFSignInfo.signList(SignID);
            signListViewModel.signList = signList;
            signListViewModel.ContributionLists = EFContribution.ContributionLists.Where(c => c.SignID == null).OrderByDescending(c => c.ContributionID).Take(10).ToList();
            signListViewModel.MemberGroupLists = EFMemberGroup.MemberGroupLists.Where(m => m.SignID == null).OrderByDescending(m => m.MemberGroupID).Take(10).ToList();
            signListViewModel = UpdateSignListViewModel(signListViewModel);
            if (signList!=null)
            {
                return PartialView("_SignListEdit",signListViewModel);
            }
            else
            {
                return View(false);
            }
        }
        /// <summary>
        /// 修改更新签到表
        /// </summary>
        /// <param name="signList"></param>
        /// <param name="signListViewModel"></param>
        /// <returns></returns>
        public ActionResult UpdateSignList(SignList signList,SignListViewModel signListViewModel)
        {
            if (ModelState.IsValid)
            {
                if (signList.EndDateTime < signList.StartDateTime)
                {
                    ViewBag.Fault = "结束时间不能小于开始时间!";
                    return View(false);
                }
                if (signList.Active == true)
                {
                    if (signList.StartDateTime < DateTime.Now && signList.EndDateTime < DateTime.Now)
                    {
                        ViewBag.Fault = "选定的时间起止范围请不要小于当前时间，除非把该表设为禁用状态 !";
                        return View(false);
                    }
                }
                if (EFSignInfo.UpdateSignList(signList))
                {
                    signListViewModel = UpdateSignListViewModel(signListViewModel);
                    return PartialView("_SignList", signListViewModel);
                }
                else
                {
                    return View(false);
                }
            }
            else
            {
                return View(false);
            }
        }
        /// <summary>
        /// 删除签到表
        /// </summary>
        /// <param name="signList"></param>
        /// <param name="signListViewModel"></param>
        /// <returns></returns>
        public ActionResult DelSignList(SignList signList,SignListViewModel signListViewModel)
        {
            if (ModelState.IsValid)
            {
                if(EFSignInfo.DelSignList(signList))
                {
                    ViewBag.Message = "删除签到表成功！";
                    signListViewModel = UpdateSignListViewModel(signListViewModel);
                    return PartialView("_SignList", signListViewModel);
                }
                else
                {
                    return PartialView("_SignList",false);
                }
            }
            else
            {
                return PartialView("_SignList", false);

            }
        }
        /// <summary>
        /// 批量处理签到表
        /// </summary>
        /// <param name="ListID"></param>
        /// <param name="formsubmit"></param>
        /// <param name="signListViewModel"></param>
        /// <returns></returns>
        public ActionResult DealSignList(List<string> ListID, string formsubmit, SignListViewModel signListViewModel)
        {
            if (ListID != null)
            {
                if (formsubmit == "删除")
                {
                    if (EFSignInfo.DelSignList(ListID)==false)
                    {
                        return View(false);
                    }
                }
                else if (formsubmit=="启用")
                {
                    if (EFSignInfo.ActiveAbleList(ListID) == false)
                    {
                        return View(false);
                    }
                }
                else if (formsubmit=="禁用")
                {
                    if (EFSignInfo.ActiveDisableList(ListID) == false)
                    {
                        return View(false);
                    }
                }
            }
            signListViewModel = UpdateSignListViewModel(signListViewModel);
            return PartialView("_SignList", signListViewModel);
        }

        
        /// <summary>
        /// 签到信息管理
        /// </summary>
        /// <param name="pageID"></param>
        /// <param name="pageSize"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public ActionResult SignInfoManage(int pageID=1,int pageSize=20,string keyword= null,string sortBy= null, string currentSort = null, bool isOrderBy = false)
        {
            DealViewBag("SignInfo");
            signInfoViewModel = GetSignInfoViewModel(pageID,pageSize,keyword,  sortBy,  currentSort,  isOrderBy);
            if (Request.IsAjaxRequest())
            {
                return PartialView("_SignInfo", signInfoViewModel);
            }
            return View(signInfoViewModel);
        }
        public ActionResult GetAddSignInfo(SignInfoViewModel signInfoViewModel)
        {
            if (ModelState.IsValid)
            {
                signInfoViewModel = UpdateSignInfoViewModel(signInfoViewModel);
                signInfoViewModel.SignLists = EFSignInfo.signLists.OrderByDescending(s=>s.SignID).Take(10);
                return PartialView("_AddSignInfo", signInfoViewModel);
            }
            return View(false);
        }
        public ActionResult GetAddGameMember(SignList signList,SignInfoViewModel signInfoViewModel)
        {
            List<GameMember> gameMembers=new List<GameMember>();
            IEnumerable<SignInfo> signInfos = EFSignInfo.signInfos.Where(s => s.SignID == signList.SignID);
            foreach (var item in EFGameMember.gameMembers.Where(g=>g.IsDel!=true).ToList())
            {
                if (signInfos.Where(s => s.GameID == item.GameID).FirstOrDefault() == null)
                {
                    gameMembers.Add(item);
                }
            }
            signInfoViewModel.GameMembers = gameMembers;
            return PartialView("_GameMember", signInfoViewModel);
        }
        /// <summary>
        /// 添加签到信息
        /// </summary>
        /// <param name="signInfo"></param>
        /// <param name="signInfoViewModel"></param>
        /// <returns></returns>
        public ActionResult AddSignInfo(SignInfo signInfo,SignInfoViewModel signInfoViewModel)
        {
            if (ModelState.IsValid)
            {
                if (EFSignInfo.AddSignInfo(signInfo))
                {
                    signInfoViewModel = UpdateSignInfoViewModel(signInfoViewModel);
                    return PartialView("_SignInfo", signInfoViewModel);
                }
            }
            return View();
        }
        /// <summary>
        /// 获取签到信息
        /// </summary>
        /// <param name="SignID"></param>
        /// <param name="GameID"></param>
        /// <param name="signInfoViewModel"></param>
        /// <returns></returns>
        public ActionResult GetSignInfo(string SignID,int GameID,SignInfoViewModel signInfoViewModel)
        {
            if (ModelState.IsValid)
            {
                SignInfo signInfo = EFSignInfo.signInfo(SignID, GameID);
                if (signInfo!=null)
                {
                    signInfoViewModel = UpdateSignInfoViewModel(signInfoViewModel);
                    signInfoViewModel.SignInfo = signInfo;
                    return PartialView("_EditSignInfo", signInfoViewModel);
                }
            }
            return View(false);
        }
        /// <summary>
        /// 更新签到信息
        /// </summary>
        /// <param name="signInfo"></param>
        /// <param name="signInfoViewModel"></param>
        /// <returns></returns>
        public ActionResult UpdateSignInfo(SignInfo signInfo,SignInfoViewModel signInfoViewModel)
        {
            if (ModelState.IsValid)
            {
                if (EFSignInfo.UpdateSignInfo(signInfo))
                {
                    signInfoViewModel = UpdateSignInfoViewModel(signInfoViewModel);
                    return PartialView("_SignInfo", signInfoViewModel);
                }
            }
            return View(false);
        }

        /// <summary>
        /// 删除签到信息
        /// </summary>
        /// <param name="signInfo"></param>
        /// <param name="signInfoViewModel"></param>
        /// <returns></returns>
        public ActionResult DelSignInfo(SignInfo signInfo,SignInfoViewModel signInfoViewModel)
        {
            if (EFSignInfo.DelSignInfo(signInfo))
            {
                signInfoViewModel = UpdateSignInfoViewModel(signInfoViewModel);
                return PartialView("_SignInfo", signInfoViewModel);
            }
            return View();
        }

        public ActionResult DealSignInfoTable(List<string> ListID,string formsubmit,SignInfoViewModel signInfoViewModel)
        {
            if (ListID != null)
            {
                if (formsubmit == "删除")
                {
                    if (EFSignInfo.DelSignInfo(ListID) == false)
                    {
                        return View(false);
                    }
                }
            }
            signInfoViewModel = UpdateSignInfoViewModel(signInfoViewModel);
            return PartialView("_SignInfo", signInfoViewModel);
        }

        /// <summary>
        /// 获取设置签到表视图模型数据
        /// </summary>
        /// <param name="pageID"></param>
        /// <param name="pageSize"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public SignListViewModel GetSignListViewModel(int pageID,int pageSize,string keyword, string sortBy, string currentSort, bool isOrderBy)
        {
            TempData["PageSize"] = pageSize;
            signListViewModel = new SignListViewModel
            {
                signLists = EFSignInfo.signLists,
                signListsPageList = EFSignInfo.searchSignLists(keyword).OrderByDescending(s => s.SignID).ToPagedList(pageID, pageSize),
                pageID = pageID,
                pageSize = pageSize,
                keyword = keyword,
                CurrentSort=currentSort,
                SortBy=sortBy,
                IsOrderBy=isOrderBy
            };
            signListViewModel = SortSignListViewModel(signListViewModel);
            return signListViewModel;
        }
        /// <summary>
        /// 更新签到表视图模型数据
        /// </summary>
        /// <param name="signListViewModel"></param>
        /// <returns></returns>
        public SignListViewModel UpdateSignListViewModel(SignListViewModel signListViewModel)
        {
            TempData["PageSize"] = signListViewModel.pageSize;
            signListViewModel.signLists = EFSignInfo.signLists;
            signListViewModel = SortSignListViewModel(signListViewModel);
            return signListViewModel;
        }

        public SignListViewModel SortSignListViewModel(SignListViewModel signListViewModel)
        {
            if (!(string.IsNullOrEmpty(signListViewModel.SortBy)))
            {
                if (signListViewModel.IsOrderBy)
                {
                    signListViewModel.signListsPageList = SortSignLists(signListViewModel.pageID, signListViewModel.pageSize, signListViewModel.keyword, signListViewModel.SortBy, signListViewModel.CurrentSort);
                    if (signListViewModel.SortBy.Equals(signListViewModel.CurrentSort))
                    {
                        signListViewModel.CurrentSort = null;
                    }
                    else
                    {
                        signListViewModel.CurrentSort = signListViewModel.SortBy;
                    }
                }
                else
                {
                    signListViewModel.signListsPageList = SortSignLists(signListViewModel.pageID, signListViewModel.pageSize, signListViewModel.keyword, signListViewModel.SortBy, (string.IsNullOrEmpty(signListViewModel.CurrentSort))?signListViewModel.SortBy:null);
                }
            }
            else
            {
                signListViewModel.signListsPageList = EFSignInfo.searchSignLists(signListViewModel.keyword).OrderByDescending(s => s.SignID).ToPagedList(signListViewModel.pageID, signListViewModel.pageSize);
            }
            return signListViewModel;
        }
        public PagedList<SignList> SortSignLists(int pageID, int pageSize, string keyword, string sortBy, string currentSort)
        {
            if (sortBy.Equals("SignID"))
            {
                if (sortBy.Equals(currentSort))
                {
                    return EFSignInfo.searchSignLists(keyword).OrderByDescending(s => s.SignID).ToPagedList(pageID, pageSize);
                }
                else
                {
                    return EFSignInfo.searchSignLists(keyword).OrderBy(s => s.SignID).ToPagedList(pageID, pageSize);
                }
            }
            else if (sortBy.Equals("Type"))
            {
                if (sortBy.Equals(currentSort))
                {
                    return EFSignInfo.searchSignLists(keyword).OrderByDescending(s=>s.Type).ThenByDescending(s => s.SignID).ToPagedList(pageID, pageSize);
                }
                else
                {
                    return EFSignInfo.searchSignLists(keyword).OrderBy(s => s.Type).ThenByDescending(s => s.SignID).ToPagedList(pageID, pageSize);
                }
            }
            else if (sortBy.Equals("Active"))
            {
                if (sortBy.Equals(currentSort))
                {
                    return EFSignInfo.searchSignLists(keyword).OrderByDescending(s => s.Active).ThenByDescending(s => s.SignID).ToPagedList(pageID, pageSize);
                }
                else
                {
                    return EFSignInfo.searchSignLists(keyword).OrderBy(s => s.Active).ThenByDescending(s => s.SignID).ToPagedList(pageID, pageSize);
                }
            }
            else if (sortBy.Equals("ContributionID"))
            {
                if (sortBy.Equals(currentSort))
                {
                    return EFSignInfo.searchSignLists(keyword).OrderByDescending(s => s.ContributionID).ThenByDescending(s => s.SignID).ToPagedList(pageID, pageSize);
                }
                else
                {
                    return EFSignInfo.searchSignLists(keyword).OrderBy(s => s.ContributionID).ThenByDescending(s => s.SignID).ToPagedList(pageID, pageSize);
                }
            }
            else if (sortBy.Equals("MemberGroupID"))
            {
                if (sortBy.Equals(currentSort))
                {
                    return EFSignInfo.searchSignLists(keyword).OrderByDescending(s => s.MemberGroupID).ThenByDescending(s => s.SignID).ToPagedList(pageID, pageSize);
                }
                else
                {
                    return EFSignInfo.searchSignLists(keyword).OrderBy(s => s.MemberGroupID).ThenByDescending(s => s.SignID).ToPagedList(pageID, pageSize);
                }
            }
            else if (sortBy.Equals("StartDateTime"))
            {
                if (sortBy.Equals(currentSort))
                {
                    return EFSignInfo.searchSignLists(keyword).OrderByDescending(s => s.StartDateTime).ThenByDescending(s => s.SignID).ToPagedList(pageID, pageSize);
                }
                else
                {
                    return EFSignInfo.searchSignLists(keyword).OrderBy(s => s.StartDateTime).ThenByDescending(s => s.SignID).ToPagedList(pageID, pageSize);
                }
            }
            else if (sortBy.Equals("EndDateTime"))
            {
                if (sortBy.Equals(currentSort))
                {
                    return EFSignInfo.searchSignLists(keyword).OrderByDescending(s => s.EndDateTime).ThenByDescending(s => s.SignID).ToPagedList(pageID, pageSize);
                }
                else
                {
                    return EFSignInfo.searchSignLists(keyword).OrderBy(s => s.EndDateTime).ThenByDescending(s => s.SignID).ToPagedList(pageID, pageSize);
                }
            }
            else if (sortBy.Equals("CreateTime"))
            {
                if (sortBy.Equals(currentSort))
                {
                    return EFSignInfo.searchSignLists(keyword).OrderByDescending(s => s.CreateTime).ThenByDescending(s=>s.SignID).ToPagedList(pageID, pageSize);
                }
                else
                {
                    return EFSignInfo.searchSignLists(keyword).OrderBy(s => s.CreateTime).ThenByDescending(s => s.SignID).ToPagedList(pageID, pageSize);
                }
            }
            return null;
        }

        /// <summary>
        /// 获取签到信息视图模型
        /// </summary>
        /// <param name="pageID"></param>
        /// <param name="pageSize"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public SignInfoViewModel GetSignInfoViewModel(int pageID,int pageSize,string keyword, string sortBy, string currentSort, bool isOrderBy)
        {
            ViewBag.PageSize = pageSize;
            signInfoViewModel = new SignInfoViewModel
            {
                GameMembers=EFGameMember.gameMembers,
                SignInfos = EFSignInfo.signInfos,
                SignInfoPageTable = EFSignInfo.searchSignInfos(keyword).OrderByDescending(s => s.SignID).ToPagedList(pageID, pageSize),
                PageID = pageID,
                PageSize=pageSize,
                Keyword=keyword,
                CurrentSort=currentSort,
                SortBy=sortBy,
                IsOrderBy=isOrderBy
            };
            signInfoViewModel = SortSignInfoViewModel(signInfoViewModel);
            return signInfoViewModel;
        }
        /// <summary>
        /// 更新签到信息视图模型
        /// </summary>
        /// <param name="signInfoViewModel"></param>
        /// <returns></returns>
        public SignInfoViewModel UpdateSignInfoViewModel(SignInfoViewModel signInfoViewModel)
        {
            ViewBag.PageSize = signInfoViewModel.PageSize;
            signInfoViewModel.SignInfos = EFSignInfo.signInfos;
            signInfoViewModel.GameMembers = EFGameMember.gameMembers;
            signInfoViewModel = SortSignInfoViewModel(signInfoViewModel);
            return signInfoViewModel;
        }
        public SignInfoViewModel SortSignInfoViewModel(SignInfoViewModel signInfoViewModel)
        {
            if (!(string.IsNullOrEmpty(signInfoViewModel.SortBy)))
            {
                if (signInfoViewModel.IsOrderBy)
                {
                    signInfoViewModel.SignInfoPageTable = SortSignInfos(signInfoViewModel.PageID, signInfoViewModel.PageSize, signInfoViewModel.Keyword, signInfoViewModel.SortBy, signInfoViewModel.CurrentSort);
                    if (signInfoViewModel.SortBy.Equals(signInfoViewModel.CurrentSort))
                    {
                        signInfoViewModel.CurrentSort = null;
                    }
                    else
                    {
                        signInfoViewModel.CurrentSort = signInfoViewModel.SortBy;
                    }
                }
                else
                {
                    signInfoViewModel.SignInfoPageTable = SortSignInfos(signInfoViewModel.PageID, signInfoViewModel.PageSize, signInfoViewModel.Keyword, signInfoViewModel.SortBy, (string.IsNullOrEmpty(signInfoViewModel.CurrentSort)) ? signInfoViewModel.SortBy : null);
                }
            }
            else
            {
                signInfoViewModel.SignInfoPageTable = EFSignInfo.searchSignInfos(signInfoViewModel.Keyword).OrderByDescending(s => s.SignID).ThenBy(s=>s.GameID).ToPagedList(signInfoViewModel.PageID, signInfoViewModel.PageSize);
            }
            return signInfoViewModel;
        }
        public PagedList<SignInfo> SortSignInfos(int pageID, int pageSize, string keyword, string sortBy, string currentSort)
        {
            if (sortBy.Equals("SignID"))
            {
                if (sortBy.Equals(currentSort))
                {
                    return EFSignInfo.searchSignInfos(keyword).OrderByDescending(s => s.SignID).ToPagedList(pageID, pageSize);
                }
                else
                {
                    return EFSignInfo.searchSignInfos(keyword).OrderBy(s => s.SignID).ToPagedList(pageID, pageSize);
                }
            }
            else if (sortBy.Equals("GameID"))
            {
                if (sortBy.Equals(currentSort))
                {
                    return EFSignInfo.searchSignInfos(keyword).OrderByDescending(s => s.GameID).ThenByDescending(s => s.SignID).ToPagedList(pageID, pageSize);
                }
                else
                {
                    return EFSignInfo.searchSignInfos(keyword).OrderBy(s => s.GameID).ThenByDescending(s => s.SignID).ToPagedList(pageID, pageSize);
                }
            }
            else if (sortBy.Equals("SignCondition"))
            {
                if (sortBy.Equals(currentSort))
                {
                    return EFSignInfo.searchSignInfos(keyword).OrderByDescending(s => s.SignCondition).ThenByDescending(s => s.SignID).ToPagedList(pageID, pageSize);
                }
                else
                {
                    return EFSignInfo.searchSignInfos(keyword).OrderBy(s => s.SignCondition).ThenByDescending(s => s.SignID).ToPagedList(pageID, pageSize);
                }
            }
            else if (sortBy.Equals("SignDatetime"))
            {
                if (sortBy.Equals(currentSort))
                {
                    return EFSignInfo.searchSignInfos(keyword).OrderByDescending(s => s.SignDatetime).ThenByDescending(s => s.SignID).ToPagedList(pageID, pageSize);
                }
                else
                {
                    return EFSignInfo.searchSignInfos(keyword).OrderBy(s => s.SignDatetime).ThenByDescending(s => s.SignID).ToPagedList(pageID, pageSize);
                }
            }
            else if (sortBy.Equals("VoiceCondition"))
            {
                if (sortBy.Equals(currentSort))
                {
                    return EFSignInfo.searchSignInfos(keyword).OrderByDescending(s => s.VoiceCondition).ThenByDescending(s => s.SignID).ToPagedList(pageID, pageSize);
                }
                else
                {
                    return EFSignInfo.searchSignInfos(keyword).OrderBy(s => s.VoiceCondition).ThenByDescending(s => s.SignID).ToPagedList(pageID, pageSize);
                }
            }
            else if (sortBy.Equals("IsLeave"))
            {
                if (sortBy.Equals(currentSort))
                {
                    return EFSignInfo.searchSignInfos(keyword).OrderByDescending(s => s.IsLeave).ThenByDescending(s => s.SignID).ToPagedList(pageID, pageSize);
                }
                else
                {
                    return EFSignInfo.searchSignInfos(keyword).OrderBy(s => s.IsLeave).ThenByDescending(s => s.SignID).ToPagedList(pageID, pageSize);
                }
            }
            else if (sortBy.Equals("Deal"))
            {
                if (sortBy.Equals(currentSort))
                {
                    return EFSignInfo.searchSignInfos(keyword).OrderByDescending(s => s.Deal).ThenByDescending(s => s.SignID).ToPagedList(pageID, pageSize);
                }
                else
                {
                    return EFSignInfo.searchSignInfos(keyword).OrderBy(s => s.Deal).ThenByDescending(s => s.SignID).ToPagedList(pageID, pageSize);
                }
            }
            return null;
        }



        public FileResult ExportSignInfo(string SignID)
        {
            //创建Excel文件的对象
            HSSFWorkbook xSSFWorkbook = new HSSFWorkbook();
            //添加一个sheet
            ISheet sheet1 = xSSFWorkbook.CreateSheet("Sheet1");
            if (string.IsNullOrEmpty(SignID))
            {
                IRow row1 = sheet1.CreateRow(0);
                row1.CreateCell(0).SetCellValue("GameID");
                row1.CreateCell(1).SetCellValue("VoiceCondition");
                IRow rowtemp = sheet1.CreateRow(1);
                rowtemp.CreateCell(0).SetCellValue("123456");
                rowtemp.CreateCell(1).SetCellValue("语音");
                rowtemp = sheet1.CreateRow(2);
                rowtemp.CreateCell(0).SetCellValue("123456");
                rowtemp.CreateCell(1).SetCellValue("未语音");
            }
            else
            {
                //获取list数据
                List<SignInfo> signInfos = EFSignInfo.signInfos.Where(s => s.SignID == SignID).OrderBy(s => s.GameID).ToList();
                //给sheet1添加第一行的头部标题
                IRow row1 = sheet1.CreateRow(0);
                row1.CreateCell(0).SetCellValue("GameID");
                row1.CreateCell(1).SetCellValue("SignCondition");
                row1.CreateCell(2).SetCellValue("SignDatetime");
                row1.CreateCell(3).SetCellValue("VoiceCondition");
                row1.CreateCell(4).SetCellValue("IsLeave");
                row1.CreateCell(5).SetCellValue("Deal");
                //将数据逐步写入sheet1各个行
                for (int i = 0; i < signInfos.Count; i++)
                {
                    IRow rowtemp = sheet1.CreateRow(i + 1);
                    rowtemp.CreateCell(0).SetCellValue(signInfos[i].GameID.ToString());
                    rowtemp.CreateCell(1).SetCellValue(signInfos[i].SignCondition.ToString());
                    rowtemp.CreateCell(2).SetCellValue(signInfos[i].SignDatetime.ToString());
                    rowtemp.CreateCell(3).SetCellValue(signInfos[i].VoiceCondition.ToString());
                    rowtemp.CreateCell(4).SetCellValue(signInfos[i].IsLeave.ToString());
                    rowtemp.CreateCell(5).SetCellValue(signInfos[i].Deal==null?null: signInfos[i].Deal.ToString());
                }
            }
            // 写入到客户端 
            MemoryStream ms = new MemoryStream();
            xSSFWorkbook.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/vnd.ms-excel", "游戏社团人签到名单-" + (SignID==null?"样本文件":SignID.ToString()) + ".xls");
        }

        public ActionResult ImportSignInfo(HttpPostedFileBase file,string SignID, SignListViewModel signListViewModel)
        {
            DataTable dataTable = new DataTable();
            var fileName = file.FileName;
            var filePath = Server.MapPath(string.Format("~/{0}", "Files"));
            string path = Path.Combine(filePath, fileName);
            file.SaveAs(path);
            dataTable = ImportExcel.GetExcelDataTable(path);
            if (dataTable.Rows.Count > 0)
            {
                GameClubEntities gameClubEntities = new GameClubEntities();
                CovertListHelper toList = new CovertListHelper();
                List<SignInfo> signInfos= toList.convertToList<SignInfo>(dataTable);
                IEnumerable<SignInfo> signInfosResult = gameClubEntities.SignInfo.Where(s => s.SignID == SignID);
                foreach (var item in signInfos)
                {
                    SignInfo signInfo = signInfosResult.Where(s => s.GameID == item.GameID).FirstOrDefault();
                    if (signInfo != null)
                    {
                        if (!string.IsNullOrEmpty(item.VoiceCondition))
                        {
                            signInfo.VoiceCondition = item.VoiceCondition;
                        }
                        else
                        {
                            signInfo.VoiceCondition = "未语音";
                        }
                    }
                }
                gameClubEntities.SaveChanges();
            }
            signListViewModel = UpdateSignListViewModel(signListViewModel);
            return PartialView("_SignList", signListViewModel);
        }

        /// <summary>
        /// 处理ViewBag
        /// </summary>
        /// <param name="setMenu"></param>
        public void DealViewBag(string setMenu)
        {
            ViewBag.Sign = "active open";
            if(setMenu.Equals("SignList"))
            {
                ViewBag.SignList = "active";
            }
            else if (setMenu.Equals("SignInfo"))
            {
                ViewBag.SignInfo = "active";
            }
        }
    }
}