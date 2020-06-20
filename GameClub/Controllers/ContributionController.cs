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
    [User("ContributionManage")]
    public class ContributionController : Controller
    {
        IContribution EFContribution;//贡献表操纵
        IGameMember EFGameMember;//成员表操纵
        ISignInfo EFSignInfo;
        IMemberGroup EFMemberGroup;

        ContributionViewModel contributionViewModel;//贡献表视图
        ContributionListViewModel contributionListViewModel;//贡献表信息视图

        public ContributionController(IContribution contribution, IGameMember gameMember, ISignInfo signInfo, IMemberGroup memberGroup)
        {
            EFContribution = contribution;
            EFGameMember = gameMember;
            EFSignInfo = signInfo;
            EFMemberGroup = memberGroup;
        }
        /// <summary>
        /// 贡献表信息
        /// </summary>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <param name="Keyword"></param>
        /// <returns></returns>
        public ActionResult ContributionManage(int PageIndex = 1, int PageSize = 20, string Keyword = null, string SortBy = null, string CurrentSort = null, bool IsOrderBy = false)
        {
            DealViewBag("ContributionInfo");
            contributionViewModel = GetContributionViewModel(PageIndex, PageSize, Keyword, SortBy, CurrentSort, IsOrderBy);
            if (Request.IsAjaxRequest())
            {
                return PartialView("_ContributionInfo", contributionViewModel);
            }
            return View(contributionViewModel);
        }
        /// <summary>
        /// 贡献表
        /// </summary>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <param name="Keyword"></param>
        /// <returns></returns>
        public ActionResult ContributionListManage(int PageIndex = 1, int PageSize = 20, string Keyword = null, string SortBy = null, string CurrentSort = null, bool IsOrderBy = false)
        {
            DealViewBag("ContributionList");
            contributionListViewModel = GetContributionListViewModel(PageIndex, PageSize, Keyword, SortBy, CurrentSort, IsOrderBy);
            if (Request.IsAjaxRequest())
            {
                return PartialView("_ContributionList", contributionListViewModel);
            }
            return View(contributionListViewModel);
        }
        public ActionResult GetAddContributionInfo(ContributionViewModel contributionViewModel) 
        {
            if (ModelState.IsValid)
            {
                contributionViewModel = UpdateContributionViewModel(contributionViewModel);
                contributionViewModel.ContributionLists = EFContribution.ContributionLists.OrderByDescending(c=>c.ContributionID).Take(10);
                return PartialView("_AddContributionInfo", contributionViewModel);
            }
            return View(false);
        }
        public ActionResult GetAddGameMember(ContributionList contributionList, ContributionViewModel contributionViewModel) 
        {
            List<GameMember> gameMembers = new List<GameMember>();
            IEnumerable<Contribution> contributions= EFContribution.Contributions.Where(c => c.ContributionID == contributionList.ContributionID);
            foreach (var item in EFGameMember.gameMembers.Where(g => g.IsDel != true).ToList())
            {
                if (contributions.Where(c => c.GameID == item.GameID).FirstOrDefault() == null)
                {
                    gameMembers.Add(item);
                }
            }
            contributionViewModel.GameMembers = gameMembers;
            return PartialView("_GameMember", contributionViewModel);
        }
        /// <summary>
        /// 添加贡献信息
        /// </summary>
        /// <param name="contribution"></param>
        /// <param name="contributionViewModel"></param>
        /// <returns></returns>
        public ActionResult AddContributionInfo(Contribution contribution, ContributionViewModel contributionViewModel)
        {
            if (ModelState.IsValid)
            {
                if (EFContribution.AddContribution(contribution))
                {
                    contributionViewModel = UpdateContributionViewModel(contributionViewModel);
                    return PartialView("_ContributionInfo", contributionViewModel);
                }
            }
            return View(false);
        }
        /// <summary>
        /// 删除贡献信息
        /// </summary>
        /// <param name="contribution"></param>
        /// <param name="contributionViewModel"></param>
        /// <returns></returns>
        public ActionResult DelContributionInfo(Contribution contribution, ContributionViewModel contributionViewModel)
        {
            if (ModelState.IsValid)
            {
                if (EFContribution.DelContribution(contribution))
                {
                    contributionViewModel = UpdateContributionViewModel(contributionViewModel);
                    return PartialView("_ContributionInfo", contributionViewModel);
                }
            }
            return View(false);
        }
        /// <summary>
        /// 获取某个贡献信息
        /// </summary>
        /// <param name="contribution"></param>
        /// <param name="contributionViewModel"></param>
        /// <returns></returns>
        public ActionResult GetContributionInfo(Contribution contribution, ContributionViewModel contributionViewModel)
        {
            Contribution contributionResult = EFContribution.Contribution(contribution.ContributionID, contribution.GameID);
            if (contributionResult != null)
            {
                contributionViewModel.Contribution = contributionResult;
                contributionViewModel = UpdateContributionViewModel(contributionViewModel);
                return PartialView("_EditContributionInfo", contributionViewModel);
            }
            return View(false);
        }
        /// <summary>
        /// 更新贡献信息
        /// </summary>
        /// <param name="contribution"></param>
        /// <param name="contributionViewModel"></param>
        /// <returns></returns>
        public ActionResult UpdateContributionInfo(Contribution contribution, ContributionViewModel contributionViewModel)
        {
            if (ModelState.IsValid)
            {
                if (EFContribution.UpdateContribution(contribution))
                {
                    contributionViewModel = UpdateContributionViewModel(contributionViewModel);
                    return PartialView("_ContributionInfo", contributionViewModel);
                }
            }
            return View(false);
        }
        /// <summary>
        /// 处理多项贡献信息
        /// </summary>
        /// <param name="ListID"></param>
        /// <param name="DealAction"></param>
        /// <param name="contributionViewModel"></param>
        /// <returns></returns>
        public ActionResult DealContributionInfo(List<string> ListID, string DealAction, ContributionViewModel contributionViewModel)
        {
            if (ModelState.IsValid)
            {
                if (EFContribution.DealListContribution(ListID, DealAction))
                {
                    contributionViewModel = UpdateContributionViewModel(contributionViewModel);
                    return PartialView("_ContributionInfo", contributionViewModel);
                }
            }
            return View(false);
        }

        public ActionResult GetAddContributionList(ContributionListViewModel contributionListViewModel)
        {
            contributionListViewModel.SignLists = EFSignInfo.signLists.Where(s => s.ContributionID == null).OrderByDescending(s => s.ContributionID).Take(10).ToList();
            contributionListViewModel.MemberGroupLists = EFMemberGroup.MemberGroupLists.Where(m => m.ContributionID == null).OrderByDescending(m => m.MemberGroupID).Take(10).ToList();
            return PartialView("_AddContributionList", contributionListViewModel);
        }

        /// <summary>
        /// 添加贡献表
        /// </summary>
        /// <param name="contributionList"></param>
        /// <param name="contributionListViewModel"></param>
        /// <returns></returns>
        public ActionResult AddContributionList(ContributionList contributionList, ContributionListViewModel contributionListViewModel)
        {
            if (ModelState.IsValid)
            {
                if (EFContribution.AddContributionList(contributionList))
                {
                    SignList signList = EFSignInfo.signLists.Where(s => s.SignID == contributionList.SignID).FirstOrDefault();
                    MemberGroupList memberGroupList = EFMemberGroup.MemberGroupLists.Where(m => m.MemberGroupID == contributionList.MemberGroupID).FirstOrDefault();
                    if (signList != null&&memberGroupList!=null)
                    {
                        signList.ContributionID = contributionList.ContributionID;
                        signList.MemberGroupID = contributionList.MemberGroupID;
                        memberGroupList.ContributionID = contributionList.ContributionID;
                        memberGroupList.SignID = contributionList.SignID;
                    }
                    else if (memberGroupList != null)
                    {
                        memberGroupList.ContributionID = contributionList.ContributionID;
                    }
                    else if(signList != null)
                    {
                        signList.ContributionID = contributionList.ContributionID;
                    }
                    EFSignInfo.UpdateSignList(signList);
                    EFMemberGroup.UpdateMemberGroupList(memberGroupList);
                    ViewBag.Success = "贡献表添加成功！";
                    contributionListViewModel = UpdateContributionListViewModel(contributionListViewModel);
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
            if (contributionListViewModel.ContributionListsPageLists == null)
            {
                contributionListViewModel = UpdateContributionListViewModel(contributionListViewModel);
            }
            return PartialView("_ContributionList", contributionListViewModel);
        }
        /// <summary>
        /// 删除贡献表
        /// </summary>
        /// <param name="contributionList"></param>
        /// <param name="contributionListViewModel"></param>
        /// <returns></returns>
        public ActionResult DelContributionList(ContributionList contributionList, ContributionListViewModel contributionListViewModel)
        {
            if (ModelState.IsValid)
            {
                if (EFContribution.DelContributionList(contributionList))
                {
                    ViewBag.Success = "删除贡献表成功！";
                    contributionListViewModel = UpdateContributionListViewModel(contributionListViewModel);
                }
                else
                {
                    ViewBag.Fault = "删除贡献表失败！";
                }
            }
            if (contributionListViewModel.ContributionListsPageLists == null)
            {
                contributionListViewModel = UpdateContributionListViewModel(contributionListViewModel);
            }
            return PartialView("_ContributionList", contributionListViewModel);
        }
        /// <summary>
        /// 获取某个贡献表
        /// </summary>
        /// <param name="contributionList"></param>
        /// <param name="contributionListViewModel"></param>
        /// <returns></returns>
        public ActionResult GetContributionList(ContributionList contributionList, ContributionListViewModel contributionListViewModel)
        {
            ContributionList contributionListResult = EFContribution.ContributionList(contributionList.ContributionID);
            contributionListViewModel.SignLists = EFSignInfo.signLists.Where(s => s.ContributionID == null).OrderByDescending(s => s.SignID).Take(10).ToList();
            contributionListViewModel.MemberGroupLists = EFMemberGroup.MemberGroupLists.Where(m => m.ContributionID == null).OrderByDescending(m => m.MemberGroupID).Take(10).ToList();
            if (contributionListResult != null)
            {
                contributionListViewModel.ContributionList = contributionListResult;
                contributionListViewModel = UpdateContributionListViewModel(contributionListViewModel);
                return PartialView("_EditContributionList", contributionListViewModel);
            }
            return View(false);
        }
        /// <summary>
        /// 更新贡献表
        /// </summary>
        /// <param name="contributionList"></param>
        /// <param name="contributionListViewModel"></param>
        /// <returns></returns>
        public ActionResult UpdateContributionList(ContributionList contributionList, ContributionListViewModel contributionListViewModel)
        {
            if (ModelState.IsValid)
            {
                if (EFContribution.UpdateContributionList(contributionList))
                {
                    contributionListViewModel = UpdateContributionListViewModel(contributionListViewModel);
                    return PartialView("_ContributionList", contributionListViewModel);
                }
            }
            return View(false);
        }
        /// <summary>
        /// 批量处理贡献表
        /// </summary>
        /// <param name="ListID"></param>
        /// <param name="DealAction"></param>
        /// <param name="contributionListViewModel"></param>
        /// <returns></returns>
        public ActionResult DealContributionList(List<string> ListID, string DealAction, ContributionListViewModel contributionListViewModel)
        {
            if (ListID != null)
            {
                if (EFContribution.DealListContributionList(ListID, DealAction))
                {
                    contributionListViewModel = UpdateContributionListViewModel(contributionListViewModel);
                    return PartialView("_ContributionList", contributionListViewModel);
                }
            }
            return View(false);
        }

        /// <summary>
        /// 获取签到表视图
        /// </summary>
        /// <param name="PageIdex"></param>
        /// <param name="PageSize"></param>
        /// <param name="Keyword"></param>
        /// <returns></returns>
        public ContributionListViewModel GetContributionListViewModel(int PageIdex, int PageSize, string Keyword, string SortBy, string CurrentSort, bool IsOrderBy)
        {
            contributionListViewModel = new ContributionListViewModel
            {
                ContributionListsPageLists = EFContribution.GetContributionLists(Keyword).OrderByDescending(c => c.ContributionID).ToPagedList(PageIdex, PageSize),
                PageIndex = PageIdex,
                PageSize = PageSize,
                Keyword = Keyword,
                CurrentSort = CurrentSort,
                SortBy = SortBy,
                IsOrderBy = IsOrderBy,
            };
            contributionListViewModel = SortContributionListViewModel(contributionListViewModel);
            return contributionListViewModel;
        }
        /// <summary>
        /// 更新签到表视图
        /// </summary>
        /// <param name="contributionListViewModel"></param>
        /// <returns></returns>
        public ContributionListViewModel UpdateContributionListViewModel(ContributionListViewModel contributionListViewModel)
        {
            contributionListViewModel = SortContributionListViewModel(contributionListViewModel);
            return contributionListViewModel;
        }

        public ContributionListViewModel SortContributionListViewModel(ContributionListViewModel contributionListViewModel)
        {
            if (!(string.IsNullOrEmpty(contributionListViewModel.SortBy)))
            {
                if (contributionListViewModel.IsOrderBy)
                {
                    contributionListViewModel.ContributionListsPageLists = SortContributionLists(contributionListViewModel.PageIndex, contributionListViewModel.PageSize, contributionListViewModel.Keyword, contributionListViewModel.SortBy, contributionListViewModel.CurrentSort);
                    if (contributionListViewModel.SortBy.Equals(contributionListViewModel.CurrentSort))
                    {
                        contributionListViewModel.CurrentSort = null;
                    }
                    else
                    {
                        contributionListViewModel.CurrentSort = contributionListViewModel.SortBy;
                    }
                }
                else
                {
                    contributionListViewModel.ContributionListsPageLists = SortContributionLists(contributionListViewModel.PageIndex, contributionListViewModel.PageSize, contributionListViewModel.Keyword, contributionListViewModel.SortBy, (string.IsNullOrEmpty(contributionListViewModel.CurrentSort)) ? contributionListViewModel.SortBy : null);
                }
            }
            else
            {
                contributionListViewModel.ContributionListsPageLists = EFContribution.GetContributionLists(contributionListViewModel.Keyword).OrderByDescending(c => c.ContributionID).ToPagedList(contributionListViewModel.PageIndex, contributionListViewModel.PageSize);
            }
            return contributionListViewModel;
        }
        public PagedList<ContributionList> SortContributionLists(int PageIndex, int PageSize, string Keyword, string SortBy, string CurrentSort)
        {
            if (SortBy.Equals("ContributionID"))
            {
                if (SortBy.Equals(CurrentSort))
                {
                    return EFContribution.GetContributionLists(Keyword).OrderByDescending(c => c.ContributionID).ToPagedList(PageIndex, PageSize);
                }
                else
                {
                    return EFContribution.GetContributionLists(Keyword).OrderBy(c => c.ContributionID).ToPagedList(PageIndex, PageSize);
                }
            }
            else if (SortBy.Equals("Type"))
            {
                if (SortBy.Equals(CurrentSort))
                {
                    return EFContribution.GetContributionLists(Keyword).OrderByDescending(c => c.Type).ThenByDescending(c => c.ContributionID).ToPagedList(PageIndex, PageSize);
                }
                else
                {
                    return EFContribution.GetContributionLists(Keyword).OrderBy(c => c.Type).ThenByDescending(c => c.ContributionID).ToPagedList(PageIndex, PageSize);
                }
            }
            else if (SortBy.Equals("Time"))
            {
                if (SortBy.Equals(CurrentSort))
                {
                    return EFContribution.GetContributionLists(Keyword).OrderByDescending(c => c.Time).ThenByDescending(c => c.ContributionID).ToPagedList(PageIndex, PageSize);
                }
                else
                {
                    return EFContribution.GetContributionLists(Keyword).OrderBy(c => c.Time).ThenByDescending(c => c.ContributionID).ToPagedList(PageIndex, PageSize);
                }
            }
            else if (SortBy.Equals("SignID"))
            {
                if (SortBy.Equals(CurrentSort))
                {
                    return EFContribution.GetContributionLists(Keyword).OrderByDescending(c => c.SignID).ThenByDescending(c => c.ContributionID).ToPagedList(PageIndex, PageSize);
                }
                else
                {
                    return EFContribution.GetContributionLists(Keyword).OrderBy(c => c.SignID).ThenByDescending(c => c.ContributionID).ToPagedList(PageIndex, PageSize);
                }
            }
            else if (SortBy.Equals("MemberGroupID"))
            {
                if (SortBy.Equals(CurrentSort))
                {
                    return EFContribution.GetContributionLists(Keyword).OrderByDescending(c => c.MemberGroupID).ThenByDescending(c => c.ContributionID).ToPagedList(PageIndex, PageSize);
                }
                else
                {
                    return EFContribution.GetContributionLists(Keyword).OrderBy(c => c.MemberGroupID).ThenByDescending(c => c.ContributionID).ToPagedList(PageIndex, PageSize);
                }
            }
            else if (SortBy.Equals("CreateDateTime"))
            {
                if (SortBy.Equals(CurrentSort))
                {
                    return EFContribution.GetContributionLists(Keyword).OrderByDescending(c => c.CreateDateTime).ThenByDescending(c => c.ContributionID).ToPagedList(PageIndex, PageSize);
                }
                else
                {
                    return EFContribution.GetContributionLists(Keyword).OrderBy(c => c.CreateDateTime).ThenByDescending(c => c.ContributionID).ToPagedList(PageIndex, PageSize);
                }
            }
            return null;
        }

        /// <summary>
        /// 获取签到表信息视图
        /// </summary>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <param name="Keyword"></param>
        /// <returns></returns>
        public ContributionViewModel GetContributionViewModel(int PageIndex, int PageSize, string Keyword, string SortBy, string CurrentSort, bool IsOrderBy)
        {
            contributionViewModel = new ContributionViewModel
            {
                ContributionsPageLists = EFContribution.GetContributions(Keyword).OrderByDescending(c => c.ContributionID).ThenBy(c => c.GameID).ToPagedList(PageIndex, PageSize),
                PageIndex = PageIndex,
                PageSize = PageSize,
                Keyword = Keyword,
                GameMembers = EFGameMember.gameMembers,
                CurrentSort = CurrentSort,
                SortBy = SortBy,
                IsOrderBy = IsOrderBy
            };
            contributionViewModel = SortContributionViewModel(contributionViewModel);
            return contributionViewModel;
        }
        /// <summary>
        /// 更新签到表信息视图
        /// </summary>
        /// <param name="contributionViewModel"></param>
        /// <returns></returns>
        public ContributionViewModel UpdateContributionViewModel(ContributionViewModel contributionViewModel)
        {
            contributionViewModel.GameMembers = EFGameMember.gameMembers;
            contributionViewModel = SortContributionViewModel(contributionViewModel);
            return contributionViewModel;
        }

        public ContributionViewModel SortContributionViewModel(ContributionViewModel contributionViewModel)
        {
            if (!(string.IsNullOrEmpty(contributionViewModel.SortBy)))
            {
                if (contributionViewModel.IsOrderBy)
                {
                    contributionViewModel.ContributionsPageLists = SortContributions(contributionViewModel.PageIndex, contributionViewModel.PageSize, contributionViewModel.Keyword, contributionViewModel.SortBy, contributionViewModel.CurrentSort);
                    if (contributionViewModel.SortBy.Equals(contributionViewModel.CurrentSort))
                    {
                        contributionViewModel.CurrentSort = null;
                    }
                    else
                    {
                        contributionViewModel.CurrentSort = contributionViewModel.SortBy;
                    }
                }
                else
                {
                    contributionViewModel.ContributionsPageLists = SortContributions(contributionViewModel.PageIndex, contributionViewModel.PageSize, contributionViewModel.Keyword, contributionViewModel.SortBy, (string.IsNullOrEmpty(contributionViewModel.CurrentSort)) ? contributionViewModel.SortBy : null);
                }
            }
            else
            {
                contributionViewModel.ContributionsPageLists = EFContribution.GetContributions(contributionViewModel.Keyword).OrderByDescending(c => c.ContributionID).ThenBy(c => c.GameID).ToPagedList(contributionViewModel.PageIndex, contributionViewModel.PageSize);
            }
            return contributionViewModel;
        }
        public PagedList<Contribution> SortContributions(int PageIndex, int PageSize, string Keyword, string SortBy, string CurrentSort)
        {
            if (SortBy.Equals("ContributionID"))
            {
                if (SortBy.Equals(CurrentSort))
                {
                    return EFContribution.GetContributions(Keyword).OrderByDescending(c => c.ContributionID).ToPagedList(PageIndex, PageSize);
                }
                else
                {
                    return EFContribution.GetContributions(Keyword).OrderBy(c => c.ContributionID).ToPagedList(PageIndex, PageSize);
                }
            }
            else if (SortBy.Equals("GameID"))
            {
                if (SortBy.Equals(CurrentSort))
                {
                    return EFContribution.GetContributions(Keyword).OrderByDescending(c => c.GameID).ThenByDescending(c => c.ContributionID).ToPagedList(PageIndex, PageSize);
                }
                else
                {
                    return EFContribution.GetContributions(Keyword).OrderBy(c => c.GameID).ThenByDescending(c => c.ContributionID).ToPagedList(PageIndex, PageSize);
                }
            }
            else if (SortBy.Equals("AllContribution"))
            {
                if (SortBy.Equals(CurrentSort))
                {
                    return EFContribution.GetContributions(Keyword).OrderByDescending(c => c.AllContribution).ThenByDescending(c => c.ContributionID).ToPagedList(PageIndex, PageSize);
                }
                else
                {
                    return EFContribution.GetContributions(Keyword).OrderBy(c => c.AllContribution).ThenByDescending(c => c.ContributionID).ToPagedList(PageIndex, PageSize);
                }
            }
            else if (SortBy.Equals("MinSpeed"))
            {
                if (SortBy.Equals(CurrentSort))
                {
                    return EFContribution.GetContributions(Keyword).OrderByDescending(c => c.MinSpeed).ThenByDescending(c => c.ContributionID).ToPagedList(PageIndex, PageSize);
                }
                else
                {
                    return EFContribution.GetContributions(Keyword).OrderBy(c => c.MinSpeed).ThenByDescending(c => c.ContributionID).ToPagedList(PageIndex, PageSize);
                }
            }
            return null;
        }


        public ActionResult DealContributionManage(ContributionList contributionList)
        {
            DealViewBag("DealContributionManage");
            if (string.IsNullOrEmpty(contributionList.ContributionID))
            {
                contributionList = EFContribution.ContributionLists.OrderByDescending(c => c.ContributionID).FirstOrDefault();
            }
            contributionViewModel = new ContributionViewModel
            {
                ContributionLists = EFContribution.ContributionLists.OrderByDescending(c => c.ContributionID).Take(10).ToList(),
                Contributions = EFContribution.Contributions.Where(c => c.ContributionID == contributionList.ContributionID).OrderBy(c => c.GameID).ToList(),
                GameMembers = EFGameMember.gameMembers,
                ContributionList=contributionList,
            };
            if (Request.IsAjaxRequest())
            {
                return PartialView("_TabContent", contributionViewModel);
            }
            return View(contributionViewModel);
        }
        public ActionResult DealContributionManageSave(List<int> GameID,List<decimal> AllContribution,string ContributionID)
        {
            int n = 0;
            decimal[] allContribution = AllContribution.ToArray();
            foreach (var item in GameID)
            {
                if (allContribution[n] != 0&& allContribution[n]<200)
                {
                    Contribution contribution = EFContribution.Contributions.Where(c => c.ContributionID == ContributionID && c.GameID == item).FirstOrDefault();
                    if (contribution != null)
                    {
                        contribution.AllContribution = allContribution[n];
                        EFContribution.UpdateContribution(contribution);
                    }
                }
                n++;
            }
            ContributionList contributionList= EFContribution.ContributionLists.Where(c=>c.ContributionID==ContributionID).FirstOrDefault();
            contributionViewModel = new ContributionViewModel
            {
                ContributionLists = EFContribution.ContributionLists.OrderByDescending(c => c.ContributionID).Take(10).ToList(),
                Contributions = EFContribution.Contributions.Where(c => c.ContributionID == ContributionID).OrderBy(c => c.GameID).ToList(),
                GameMembers = EFGameMember.gameMembers,
                ContributionList = contributionList,
            };
            return PartialView("_TabContent",contributionViewModel);
        }

        public FileResult ExportContribution(string ContributionID)
        {
            //创建Excel文件的对象
            HSSFWorkbook xSSFWorkbook = new HSSFWorkbook();
            //添加一个sheet
            ISheet sheet1 = xSSFWorkbook.CreateSheet("Sheet1");
            if (string.IsNullOrEmpty(ContributionID))
            {
                IRow row1 = sheet1.CreateRow(0);
                row1.CreateCell(0).SetCellValue("GameID");
                row1.CreateCell(1).SetCellValue("AllContribution");
                IRow rowtemp = sheet1.CreateRow(1);
                rowtemp.CreateCell(0).SetCellValue("123456");
                rowtemp.CreateCell(1).SetCellValue("20.5");
            }
            else
            {
                //获取list数据
                List<Contribution> contributions = EFContribution.Contributions.Where(c => c.ContributionID== ContributionID).OrderBy(c => c.AllContribution).ToList();
                //给sheet1添加第一行的头部标题
                IRow row1 = sheet1.CreateRow(0);
                row1.CreateCell(0).SetCellValue("GameID");
                row1.CreateCell(1).SetCellValue("AllContribution");
                row1.CreateCell(2).SetCellValue("MinSpeed");
                //将数据逐步写入sheet1各个行
                for (int i = 0; i < contributions.Count; i++)
                {
                    IRow rowtemp = sheet1.CreateRow(i + 1);
                    rowtemp.CreateCell(0).SetCellValue(contributions[i].GameID.ToString());
                    rowtemp.CreateCell(1).SetCellValue(contributions[i].AllContribution.ToString());
                    rowtemp.CreateCell(2).SetCellValue(contributions[i].MinSpeed.ToString());
                }
            }
            // 写入到客户端 
            MemoryStream ms = new MemoryStream();
            xSSFWorkbook.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/vnd.ms-excel", "社团贡献数据-" + (ContributionID==null?"样本文件":ContributionID.ToString()) + ".xls");
        }

        public ActionResult ImportContribution(HttpPostedFileBase file, string ContributionID, ContributionListViewModel contributionListViewModel)
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
                List<Contribution> contributions = toList.convertToList<Contribution>(dataTable);
                IEnumerable<Contribution> contributionsResult= gameClubEntities.Contribution.Where(c => c.ContributionID== ContributionID);
                ContributionList contributionList = gameClubEntities.ContributionList.Where(c => c.ContributionID == ContributionID).FirstOrDefault();
                foreach (var item in contributions)
                {
                    Contribution contribution= contributionsResult.Where(c => c.GameID == item.GameID).FirstOrDefault();
                    if (contribution != null)
                    {
                        if (item.AllContribution!=null)
                        {
                            contribution.AllContribution = item.AllContribution;
                            if (contributionList!=null&&contributionList.Time != null && contributionList.Time > 0)
                            {
                                contribution.MinSpeed = (contribution.AllContribution) / contributionList.Time * decimal.Parse("60");
                            }
                        }
                    }
                }
                gameClubEntities.SaveChanges();
            }
            contributionListViewModel = UpdateContributionListViewModel(contributionListViewModel);
            return PartialView("_ContributionList", contributionListViewModel);
        }

        /// <summary>
        /// 设置菜单列表
        /// </summary>
        /// <param name="setMenu"></param>
        public void DealViewBag(string setMenu)
        {
            ViewBag.Contribution = "active open";
            if ("ContributionInfo".Equals(setMenu))
            {
                ViewBag.ContributionInfo = "active";
            }
            else if ("ContributionList".Equals(setMenu))
            {
                ViewBag.ContributionList = "active";
            }
            else if ("DealContributionManage".Equals(setMenu))
            {
                ViewBag.DealContributionManage = "active";
            }
        }
    }
}