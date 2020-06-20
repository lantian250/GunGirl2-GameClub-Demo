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

namespace GameClub.Controllers
{
    [User("MemberGroupManage")]
    public class MemberGroupController : Controller
    {
        IMemberGroup EFMemberGroup;//社团分组信息操纵
        IGameMember EFGameMember;//社团成员信息操纵
        ISignInfo EFSignInfo;
        IContribution EFContribution;

        MemberGroupViewModel MemberGroupViewModel;//社团分组表信息视图
        MemberGroupListViewModel MemberGroupListViewModel;//社团分组表视图

        public MemberGroupController(IMemberGroup memberGroup, IGameMember gameMember, ISignInfo signInfo, IContribution contribution)
        {
            EFMemberGroup = memberGroup;
            EFGameMember = gameMember;
            EFSignInfo = signInfo;
            EFContribution = contribution;
        }
        /// <summary>
        /// 社团分组表信息管理
        /// </summary>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <param name="Keyword"></param>
        /// <returns></returns>
        public ActionResult MemberGroupManage(int PageIndex = 1, int PageSize = 20, string Keyword = null, string SortBy = null, string CurrentSort = null, bool IsOrderBy = false)
        {
            DealViewbag("MemberGroupInfo");
            MemberGroupViewModel = GetMemberGroupViewModel(PageIndex, PageSize, Keyword, SortBy, CurrentSort, IsOrderBy);
            if (Request.IsAjaxRequest())
            {
                return PartialView("_MemberGroupInfo", MemberGroupViewModel);
            }
            return View(MemberGroupViewModel);
        }
        /// <summary>
        /// 社团分组表管理
        /// </summary>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <param name="Keyword"></param>
        /// <returns></returns>
        public ActionResult MemberGroupListManage(int PageIndex = 1, int PageSize = 20, string Keyword = null, string SortBy = null, string CurrentSort = null, bool IsOrderBy = false)
        {
            DealViewbag("MemberGroupList");
            MemberGroupListViewModel = GetMemberGroupListViewModel(PageIndex, PageSize, Keyword, SortBy, CurrentSort, IsOrderBy);
            if (Request.IsAjaxRequest())
            {
                return PartialView("_MemberGroupList", MemberGroupListViewModel);
            }
            return View(MemberGroupListViewModel);
        }

        public ActionResult GetAddMemberGroupInfo(MemberGroupViewModel memberGroupViewModel)
        {
            if (ModelState.IsValid)
            {
                memberGroupViewModel = UpdateMemerGroupViewModel(memberGroupViewModel);
                memberGroupViewModel.MemberGroupLists = EFMemberGroup.MemberGroupLists.OrderByDescending(m => m.MemberGroupID).Take(10);
                return PartialView("_AddMemberGroupInfo", memberGroupViewModel);
            }
            return View(false);
        }
        public ActionResult GetAddGameMember(MemberGroupList memberGroupList, MemberGroupViewModel memberGroupViewModel)
        {
            List<GameMember> gameMembers = new List<GameMember>();
            IEnumerable<MemberGroup> memberGroups = EFMemberGroup.MemberGroups.Where(m => m.MemberGroupID == memberGroupList.MemberGroupID);
            foreach (var item in EFGameMember.gameMembers.Where(g => g.IsDel != true).ToList())
            {
                if (memberGroups.Where(m => m.GameID == item.GameID).FirstOrDefault() == null)
                {
                    gameMembers.Add(item);
                }
            }
            memberGroupViewModel.GameMembers = gameMembers;
            memberGroupViewModel.MemberGroupList = EFMemberGroup.MemberGroupLists.Where(m => m.MemberGroupID == memberGroupList.MemberGroupID).FirstOrDefault();
            return PartialView("_GameMember", memberGroupViewModel);
        }
        /// <summary>
        /// 添加分组表信息
        /// </summary>
        /// <param name="memberGroup"></param>
        /// <param name="memberGroupViewModel"></param>
        /// <returns></returns>
        public ActionResult AddMemberGroup(MemberGroup memberGroup, MemberGroupViewModel memberGroupViewModel)
        {
            if (ModelState.IsValid)
            {
                if (EFMemberGroup.AddMemberGroup(memberGroup))
                {
                    memberGroupViewModel = UpdateMemerGroupViewModel(memberGroupViewModel);
                    return PartialView("_MemberGroupInfo", memberGroupViewModel);
                }
            }
            return View(false);
        }
        /// <summary>
        /// 删除分组表信息
        /// </summary>
        /// <param name="memberGroup"></param>
        /// <param name="memberGroupViewModel"></param>
        /// <returns></returns>
        public ActionResult DelMemberGroup(MemberGroup memberGroup, MemberGroupViewModel memberGroupViewModel)
        {
            if (EFMemberGroup.DelMemberGroup(memberGroup))
            {
                memberGroupViewModel = UpdateMemerGroupViewModel(memberGroupViewModel);
                return PartialView("_MemberGroupInfo", memberGroupViewModel);
            }
            return View(false);
        }
        /// <summary>
        /// 获取某个分组表信息
        /// </summary>
        /// <param name="memberGroup"></param>
        /// <param name="memberGroupViewModel"></param>
        /// <returns></returns>
        public ActionResult GetMemberGroup(MemberGroup memberGroup, MemberGroupViewModel memberGroupViewModel)
        {
            MemberGroup memberGroupResult = EFMemberGroup.MemberGroup(memberGroup.MemberGroupID, memberGroup.GameID);
            if (memberGroupResult != null)
            {
                memberGroupViewModel.MemberGroup = memberGroupResult;
                memberGroupViewModel.MemberGroupList = EFMemberGroup.MemberGroupLists.Where(m => m.MemberGroupID == memberGroup.MemberGroupID).FirstOrDefault();
                return PartialView("_EditMemberGroupInfo", memberGroupViewModel);
            }
            return View(false);
        }
        /// <summary>
        /// 更新分组表信息视图
        /// </summary>
        /// <param name="memberGroup"></param>
        /// <param name="memberGroupViewModel"></param>
        /// <returns></returns>
        public ActionResult UpdateMemberGroup(MemberGroup memberGroup, MemberGroupViewModel memberGroupViewModel)
        {
            if (ModelState.IsValid)
            {
                if (EFMemberGroup.UpdateMemberGroup(memberGroup))
                {
                    memberGroupViewModel = UpdateMemerGroupViewModel(memberGroupViewModel);
                    return PartialView("_MemberGroupInfo", memberGroupViewModel);
                }
            }
            return View(false);
        }
        /// <summary>
        /// 批量处理成员表信息
        /// </summary>
        /// <param name="ListID"></param>
        /// <param name="DealAction"></param>
        /// <param name="memberGroupViewModel"></param>
        /// <returns></returns>
        public ActionResult DealMemberGroup(List<string> ListID, string DealAction, MemberGroupViewModel memberGroupViewModel)
        {
            if (ListID != null)
            {
                if (EFMemberGroup.DealListMemberGroup(ListID, DealAction))
                {
                    memberGroupViewModel = UpdateMemerGroupViewModel(memberGroupViewModel);
                    return PartialView("_MemberGroupInfo", memberGroupViewModel);
                }
            }
            return View(false);
        }



        public ActionResult GetAddMemberGroupList(MemberGroupListViewModel memberGroupListViewModel)
        {
            memberGroupListViewModel.SignLists = EFSignInfo.signLists.Where(s => s.MemberGroupID == null).OrderByDescending(s => s.MemberGroupID).Take(10).ToList();
            memberGroupListViewModel.ContributionLists = EFContribution.ContributionLists.Where(c => c.MemberGroupID == null).OrderByDescending(c => c.ContributionID).ToList();
            return PartialView("_AddMemberGroupList", memberGroupListViewModel);
        }

        /// <summary>
        /// 添加分组表
        /// </summary>
        /// <param name="memberGroupList"></param>
        /// <param name="memberGroupListViewModel"></param>
        /// <returns></returns>
        public ActionResult AddMemberGroupList(MemberGroupList memberGroupList, MemberGroupListViewModel memberGroupListViewModel)
        {
            if (ModelState.IsValid)
            {
                if (EFMemberGroup.AddMemberGroupList(memberGroupList))
                {
                    SignList signList = EFSignInfo.signLists.Where(s => s.SignID == memberGroupList.SignID).FirstOrDefault();
                    ContributionList contributionList = EFContribution.ContributionLists.Where(c => c.ContributionID == memberGroupList.ContributionID).FirstOrDefault();
                    if (signList != null && contributionList != null)
                    {
                        signList.MemberGroupID = memberGroupList.MemberGroupID;
                        signList.ContributionID = memberGroupList.ContributionID;
                        contributionList.MemberGroupID = memberGroupList.MemberGroupID;
                        contributionList.SignID = memberGroupList.SignID;
                    }
                    else if (signList != null)
                    {
                        signList.MemberGroupID = memberGroupList.MemberGroupID;
                    }
                    else if (contributionList != null)
                    {
                        contributionList.MemberGroupID = memberGroupList.MemberGroupID;
                    }
                    EFSignInfo.UpdateSignList(signList);
                    EFContribution.UpdateContributionList(contributionList);
                    ViewBag.Success = "添加成功！";
                    memberGroupListViewModel = UpdateMemberGroupListViewModel(memberGroupListViewModel);
                }
                else
                {
                    ViewBag.Fault = "已存在该表！";
                }
            }
            else
            {
                ViewBag.Fault = "请检查信息！";
            }
            if (memberGroupListViewModel.MemberGroupListsPageLists == null)
            {
                memberGroupListViewModel = UpdateMemberGroupListViewModel(memberGroupListViewModel);
            }
            return PartialView("_MemberGroupList", memberGroupListViewModel);
        }
        /// <summary>
        /// 删除分组表
        /// </summary>
        /// <param name="memberGroupList"></param>
        /// <param name="memberGroupListViewModel"></param>
        /// <returns></returns>
        public ActionResult DelMemberGroupList(MemberGroupList memberGroupList, MemberGroupListViewModel memberGroupListViewModel)
        {
            if (EFMemberGroup.DelMemberGroupList(memberGroupList))
            {
                memberGroupListViewModel = UpdateMemberGroupListViewModel(memberGroupListViewModel);
                return PartialView("_MemberGroupList", memberGroupListViewModel);
            }
            return View(false);
        }
        /// <summary>
        /// 获取某个分组表
        /// </summary>
        /// <param name="memberGroupList"></param>
        /// <param name="memberGroupListViewModel"></param>
        /// <returns></returns>
        public ActionResult GetMemberGroupList(MemberGroupList memberGroupList, MemberGroupListViewModel memberGroupListViewModel)
        {
            MemberGroupList memberGroupListResult = EFMemberGroup.MemberGroupList(memberGroupList.MemberGroupID);
            memberGroupListViewModel.SignLists = EFSignInfo.signLists.Where(s => s.MemberGroupID == null).OrderByDescending(s => s.SignID).Take(10).ToList();
            memberGroupListViewModel.ContributionLists = EFContribution.ContributionLists.Where(c => c.MemberGroupID == null).OrderByDescending(c => c.MemberGroupID).Take(10).ToList();
            if (memberGroupListResult != null)
            {
                memberGroupListViewModel = UpdateMemberGroupListViewModel(memberGroupListViewModel);
                memberGroupListViewModel.MemberGroupList = memberGroupListResult;
                return PartialView("_EditMemberGroupList", memberGroupListViewModel);
            }
            return View(false);
        }
        /// <summary>
        /// 更新分组表
        /// </summary>
        /// <param name="memberGroupList"></param>
        /// <param name="memberGroupListViewModel"></param>
        /// <returns></returns>
        public ActionResult UpdateMemberGroupList(MemberGroupList memberGroupList, MemberGroupListViewModel memberGroupListViewModel)
        {
            if (ModelState.IsValid)
            {
                if (EFMemberGroup.UpdateMemberGroupList(memberGroupList))
                {
                    memberGroupListViewModel = UpdateMemberGroupListViewModel(memberGroupListViewModel);
                    return PartialView("_MemberGroupList", memberGroupListViewModel);
                }
            }
            return View(false);
        }
        /// <summary>
        /// 批量处理分组表
        /// </summary>
        /// <param name="ListID"></param>
        /// <param name="DealAction"></param>
        /// <param name="memberGroupListViewModel"></param>
        /// <returns></returns>
        public ActionResult DealMemberGroupList(List<string> ListID, string DealAction, MemberGroupListViewModel memberGroupListViewModel)
        {
            if (ListID != null)
            {
                if (EFMemberGroup.DealListMemberGroupList(ListID, DealAction))
                {
                    memberGroupListViewModel = UpdateMemberGroupListViewModel(memberGroupListViewModel);
                    return PartialView("_MemberGroupList", memberGroupListViewModel);
                }
            }
            return View(false);
        }
        /// <summary>
        /// 获取分组表信息视图
        /// </summary>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <param name="Keyword"></param>
        /// <returns></returns>
        public MemberGroupViewModel GetMemberGroupViewModel(int PageIndex, int PageSize, string Keyword, string SortBy, string CurrentSort, bool IsOrderBy)
        {
            MemberGroupViewModel = new MemberGroupViewModel
            {
                MemberGroupsPageLists = EFMemberGroup.GetMemberGroups(Keyword).OrderByDescending(m => m.MemberGroupID).ThenBy(m => m.GameID).ToPagedList(PageIndex, PageSize),
                PageIndex = PageIndex,
                PageSize = PageSize,
                Keyword = Keyword,
                CurrentSort = CurrentSort,
                SortBy = SortBy,
                IsOrderBy = IsOrderBy,
            };
            MemberGroupViewModel = SortMemberGroupViewModel(MemberGroupViewModel);
            return MemberGroupViewModel;
        }
        /// <summary>
        /// 更新分组表信息视图
        /// </summary>
        /// <param name="memberGroupViewModel"></param>
        /// <returns></returns>
        public MemberGroupViewModel UpdateMemerGroupViewModel(MemberGroupViewModel memberGroupViewModel)
        {
            memberGroupViewModel = SortMemberGroupViewModel(memberGroupViewModel);
            return memberGroupViewModel;
        }


        public MemberGroupViewModel SortMemberGroupViewModel(MemberGroupViewModel memberGroupViewModel)
        {
            if (!(string.IsNullOrEmpty(memberGroupViewModel.SortBy)))
            {
                if (memberGroupViewModel.IsOrderBy)
                {
                    memberGroupViewModel.MemberGroupsPageLists = SortMemberGroups(memberGroupViewModel.PageIndex, memberGroupViewModel.PageSize, memberGroupViewModel.Keyword, memberGroupViewModel.SortBy, memberGroupViewModel.CurrentSort);
                    if (memberGroupViewModel.SortBy.Equals(memberGroupViewModel.CurrentSort))
                    {
                        memberGroupViewModel.CurrentSort = null;
                    }
                    else
                    {
                        memberGroupViewModel.CurrentSort = memberGroupViewModel.SortBy;
                    }
                }
                else
                {
                    memberGroupViewModel.MemberGroupsPageLists = SortMemberGroups(memberGroupViewModel.PageIndex, memberGroupViewModel.PageSize, memberGroupViewModel.Keyword, memberGroupViewModel.SortBy, (string.IsNullOrEmpty(memberGroupViewModel.CurrentSort)) ? memberGroupViewModel.SortBy : null);
                }
            }
            else
            {
                memberGroupViewModel.MemberGroupsPageLists = EFMemberGroup.GetMemberGroups(memberGroupViewModel.Keyword).OrderByDescending(m => m.MemberGroupID).ThenBy(m => m.GameID).ToPagedList(memberGroupViewModel.PageIndex, memberGroupViewModel.PageSize);
            }
            return memberGroupViewModel;
        }
        public PagedList<MemberGroup> SortMemberGroups(int PageIndex, int PageSize, string Keyword, string SortBy, string CurrentSort)
        {
            if (SortBy.Equals("MemberGroupID"))
            {
                if (SortBy.Equals(CurrentSort))
                {
                    return EFMemberGroup.GetMemberGroups(Keyword).OrderByDescending(m => m.MemberGroupID).ToPagedList(PageIndex, PageSize);
                }
                else
                {
                    return EFMemberGroup.GetMemberGroups(Keyword).OrderBy(m => m.MemberGroupID).ToPagedList(PageIndex, PageSize);
                }
            }
            else if (SortBy.Equals("GameID"))
            {
                if (SortBy.Equals(CurrentSort))
                {
                    return EFMemberGroup.GetMemberGroups(Keyword).OrderByDescending(m => m.GameID).ThenByDescending(m => m.MemberGroupID).ToPagedList(PageIndex, PageSize);
                }
                else
                {
                    return EFMemberGroup.GetMemberGroups(Keyword).OrderBy(m => m.GameID).ThenByDescending(m => m.MemberGroupID).ToPagedList(PageIndex, PageSize);
                }
            }
            else if (SortBy.Equals("GroupName"))
            {
                if (SortBy.Equals(CurrentSort))
                {
                    return EFMemberGroup.GetMemberGroups(Keyword).OrderByDescending(m => m.GroupName).ThenByDescending(m => m.MemberGroupID).ToPagedList(PageIndex, PageSize);
                }
                else
                {
                    return EFMemberGroup.GetMemberGroups(Keyword).OrderBy(m => m.GroupName).ThenByDescending(m => m.MemberGroupID).ToPagedList(PageIndex, PageSize);
                }
            }
            return null;
        }

        /// <summary>
        /// 获取分组表视图
        /// </summary>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <param name="Keyword"></param>
        /// <returns></returns>
        public MemberGroupListViewModel GetMemberGroupListViewModel(int PageIndex, int PageSize, string Keyword, string SortBy, string CurrentSort, bool IsOrderBy)
        {
            MemberGroupListViewModel = new MemberGroupListViewModel
            {
                MemberGroupListsPageLists = EFMemberGroup.GetMemberGroupLists(Keyword).OrderByDescending(m => m.MemberGroupID).ToPagedList(PageIndex, PageSize),
                PageIndex = PageIndex,
                PageSize = PageSize,
                Keyword = Keyword,
                CurrentSort = CurrentSort,
                SortBy = SortBy,
                IsOrderBy = IsOrderBy,
            };
            MemberGroupListViewModel = SortMemberGroupListViewModel(MemberGroupListViewModel);
            return MemberGroupListViewModel;
        }
        /// <summary>
        /// 更新分组表视图
        /// </summary>
        /// <param name="memberGroupListViewModel"></param>
        /// <returns></returns>
        public MemberGroupListViewModel UpdateMemberGroupListViewModel(MemberGroupListViewModel memberGroupListViewModel)
        {
            memberGroupListViewModel = SortMemberGroupListViewModel(memberGroupListViewModel);
            return memberGroupListViewModel;
        }

        public MemberGroupListViewModel SortMemberGroupListViewModel(MemberGroupListViewModel memberGroupListViewModel)
        {
            if (!(string.IsNullOrEmpty(memberGroupListViewModel.SortBy)))
            {
                if (memberGroupListViewModel.IsOrderBy)
                {
                    memberGroupListViewModel.MemberGroupListsPageLists = SortMemberGroupLists(memberGroupListViewModel.PageIndex, memberGroupListViewModel.PageSize, memberGroupListViewModel.Keyword, memberGroupListViewModel.SortBy, memberGroupListViewModel.CurrentSort);
                    if (memberGroupListViewModel.SortBy.Equals(memberGroupListViewModel.CurrentSort))
                    {
                        memberGroupListViewModel.CurrentSort = null;
                    }
                    else
                    {
                        memberGroupListViewModel.CurrentSort = memberGroupListViewModel.SortBy;
                    }
                }
                else
                {
                    memberGroupListViewModel.MemberGroupListsPageLists = SortMemberGroupLists(memberGroupListViewModel.PageIndex, memberGroupListViewModel.PageSize, memberGroupListViewModel.Keyword, memberGroupListViewModel.SortBy, (string.IsNullOrEmpty(memberGroupListViewModel.CurrentSort)) ? memberGroupListViewModel.SortBy : null);
                }
            }
            else
            {
                memberGroupListViewModel.MemberGroupListsPageLists = EFMemberGroup.GetMemberGroupLists(memberGroupListViewModel.Keyword).OrderByDescending(m => m.MemberGroupID).ToPagedList(memberGroupListViewModel.PageIndex, memberGroupListViewModel.PageSize);
            }
            return memberGroupListViewModel;
        }
        public PagedList<MemberGroupList> SortMemberGroupLists(int PageIndex, int PageSize, string Keyword, string SortBy, string CurrentSort)
        {
            if (SortBy.Equals("MemberGroupID"))
            {
                if (SortBy.Equals(CurrentSort))
                {
                    return EFMemberGroup.GetMemberGroupLists(Keyword).OrderByDescending(m => m.MemberGroupID).ToPagedList(PageIndex, PageSize);
                }
                else
                {
                    return EFMemberGroup.GetMemberGroupLists(Keyword).OrderBy(m => m.MemberGroupID).ToPagedList(PageIndex, PageSize);
                }
            }
            else if (SortBy.Equals("Type"))
            {
                if (SortBy.Equals(CurrentSort))
                {
                    return EFMemberGroup.GetMemberGroupLists(Keyword).OrderByDescending(m => m.Type).ThenByDescending(m => m.MemberGroupID).ToPagedList(PageIndex, PageSize);
                }
                else
                {
                    return EFMemberGroup.GetMemberGroupLists(Keyword).OrderBy(m => m.Type).ThenByDescending(m => m.MemberGroupID).ToPagedList(PageIndex, PageSize);
                }
            }
            else if (SortBy.Equals("SignID"))
            {
                if (SortBy.Equals(CurrentSort))
                {
                    return EFMemberGroup.GetMemberGroupLists(Keyword).OrderByDescending(m => m.SignID).ThenByDescending(m => m.MemberGroupID).ToPagedList(PageIndex, PageSize);
                }
                else
                {
                    return EFMemberGroup.GetMemberGroupLists(Keyword).OrderBy(m => m.SignID).ThenByDescending(m => m.MemberGroupID).ToPagedList(PageIndex, PageSize);
                }
            }
            else if (SortBy.Equals("ContributionID"))
            {
                if (SortBy.Equals(CurrentSort))
                {
                    return EFMemberGroup.GetMemberGroupLists(Keyword).OrderByDescending(m => m.MemberGroupID).ThenByDescending(m => m.MemberGroupID).ToPagedList(PageIndex, PageSize);
                }
                else
                {
                    return EFMemberGroup.GetMemberGroupLists(Keyword).OrderBy(m => m.MemberGroupID).ThenByDescending(m => m.MemberGroupID).ToPagedList(PageIndex, PageSize);
                }
            }
            else if (SortBy.Equals("CreateDateTime"))
            {
                if (SortBy.Equals(CurrentSort))
                {
                    return EFMemberGroup.GetMemberGroupLists(Keyword).OrderByDescending(m => m.CreateDateTime).ThenByDescending(m => m.MemberGroupID).ToPagedList(PageIndex, PageSize);
                }
                else
                {
                    return EFMemberGroup.GetMemberGroupLists(Keyword).OrderBy(m => m.CreateDateTime).ThenByDescending(m => m.MemberGroupID).ToPagedList(PageIndex, PageSize);
                }
            }
            return null;
        }

        public ActionResult DealMemberGroupManage(string MemberGroupID)
        {
            DealViewbag("DealMemberGroupManage");
            MemberGroupList memberGroupList;
            if (string.IsNullOrEmpty(MemberGroupID))
            {
                memberGroupList = EFMemberGroup.MemberGroupLists.OrderByDescending(m => m.MemberGroupID).FirstOrDefault();
            }
            else
            {
                memberGroupList =  EFMemberGroup.MemberGroupLists.Where(m => m.MemberGroupID==MemberGroupID).FirstOrDefault();
            }
            MemberGroupViewModel = new MemberGroupViewModel
            {
                MemberGroupLists = EFMemberGroup.MemberGroupLists.OrderByDescending(m => m.MemberGroupID).Take(10).ToList(),
                MemberGroupList = memberGroupList,
                GameMembers = EFGameMember.gameMembers,
                MemberGroups = EFMemberGroup.MemberGroups.Where(m => m.MemberGroupID == memberGroupList.MemberGroupID).OrderBy(m => m.GameID).ToList(),
            };
            if (Request.IsAjaxRequest())
            {
                return PartialView("_TabContent", MemberGroupViewModel);
            }
            return View(MemberGroupViewModel);
        }
        public ActionResult DealMemberGroupManageSave(List<int> ListId,string MemberGroupID,string TabSign)
        {
            if (ListId != null)
            {
                foreach (var item in ListId)
                {
                    MemberGroup memberGroup = EFMemberGroup.MemberGroups.Where(m => m.MemberGroupID == MemberGroupID && m.GameID == item).FirstOrDefault();
                    if (memberGroup != null)
                    {
                        memberGroup.GroupName = TabSign;
                        EFMemberGroup.UpdateMemberGroup(memberGroup);
                    }
                }
            }
            MemberGroupViewModel = new MemberGroupViewModel
            {
                MemberGroupLists = EFMemberGroup.MemberGroupLists.OrderByDescending(m => m.MemberGroupID).Take(10).ToList(),
                MemberGroupList = EFMemberGroup.MemberGroupLists.Where(m => m.MemberGroupID == MemberGroupID).FirstOrDefault(),
                GameMembers = EFGameMember.gameMembers,
                MemberGroups = EFMemberGroup.MemberGroups.Where(m => m.MemberGroupID == MemberGroupID).OrderBy(m => m.GameID).ToList(),
            };
            return PartialView("_TabContent", MemberGroupViewModel);
        }

        public ActionResult Inherit(string MemberGroupID)
        {
            if (!string.IsNullOrEmpty(MemberGroupID))
            {
                MemberGroupList memberGroupList = EFMemberGroup.MemberGroupLists.Where(m => m.MemberGroupID == MemberGroupID).FirstOrDefault();
                if (memberGroupList != null)
                {
                    MemberGroupList memberGroupListResult = EFMemberGroup.MemberGroupLists.Where(m => m.CreateDateTime < memberGroupList.CreateDateTime&&m.Type==memberGroupList.Type).OrderByDescending(m => m.CreateDateTime).FirstOrDefault();
                    if (memberGroupListResult != null)
                    {
                        GameClubEntities gameClubEntities = new GameClubEntities();
                        IEnumerable<MemberGroup> memberGroupsNew = gameClubEntities.MemberGroup.Where(m => m.MemberGroupID == memberGroupList.MemberGroupID);
                        IEnumerable<MemberGroup> memberGroupsOld = gameClubEntities.MemberGroup.Where(m => m.MemberGroupID == memberGroupListResult.MemberGroupID);
                        foreach (var item in memberGroupsNew)
                        {
                            MemberGroup memberGroup = memberGroupsOld.Where(m => m.GameID == item.GameID).FirstOrDefault();
                            if (memberGroup != null)
                            {
                                item.GroupName = memberGroup.GroupName;
                            }
                        }
                        
                        gameClubEntities.SaveChanges();
                    }
                    MemberGroupViewModel = new MemberGroupViewModel
                    {
                        MemberGroupLists = EFMemberGroup.MemberGroupLists.OrderByDescending(m => m.MemberGroupID).Take(10).ToList(),
                        MemberGroupList = EFMemberGroup.MemberGroupLists.Where(m => m.MemberGroupID == MemberGroupID).FirstOrDefault(),
                        GameMembers = EFGameMember.gameMembers,
                        MemberGroups = EFMemberGroup.MemberGroups.Where(m => m.MemberGroupID == MemberGroupID).OrderBy(m => m.GameID).ToList(),
                    };
                    return PartialView("_TabContent", MemberGroupViewModel);
                }
            }
            return View();
        }

        /// <summary>
        /// 处理菜单隐藏显示
        /// </summary>
        /// <param name="setMenu"></param>
        public void DealViewbag(string setMenu)
        {
            ViewBag.MemberGroup = "active open";
            if ("MemberGroupInfo".Equals(setMenu))
            {
                ViewBag.MemberGroupInfo = "active";
            }
            else if ("MemberGroupList".Equals(setMenu))
            {
                ViewBag.MemberGroupList = "active";
            }
            else if ("DealMemberGroupManage".Equals(setMenu))
            {
                ViewBag.DealMemberGroupManage = "active";
            }
        }
    }
}