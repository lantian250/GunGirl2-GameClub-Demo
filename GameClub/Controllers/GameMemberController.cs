using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using GameClub.Abstract;
using GameClub.Models;
using GameClub.ViewModels;
using Webdiyer.WebControls.Mvc;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using GameClub.MyHelper;
using System.Web;
using System.Data;
using GameClub.Filters;

namespace GameClub.Controllers
{
    [User("GameMemberManage")]
    public class GameMemberController : Controller
    {
        IGameMember EFGameMember;
        IAllUserInfo EFAllUserInfo;
        IRecover Recover;
        GameMemberViewModel gameMemberViewModel;
        public GameMemberController(IGameMember EFGameMember,IAllUserInfo allUserInfo,IRecover recover)
        {
            this.EFGameMember = EFGameMember;
            this.EFAllUserInfo = allUserInfo;
            Recover = recover;
        }
        /// <summary>
        /// 获取社团团员信息
        /// </summary>
        /// <param name="pageID">索引页</param>
        /// <param name="keyword">查询关键字</param>
        /// <param name="pageSize">页大小</param>
        /// <returns></returns>
        public ActionResult GameMemberManage(int pageID = 1, int pageSize = 20, string keyword = null,string sortBy=null,string currentSort=null,bool IsOrderBy=false)
        {
            DealViewBag("GameNumberManage");
            gameMemberViewModel = getGameMemberViewModel(pageID,pageSize, keyword,sortBy,currentSort,IsOrderBy);
            return View(gameMemberViewModel);
        }
        /// <summary>
        /// 搜索结果页面
        /// </summary>
        /// <param name="pageID"></param>
        /// <param name="pageSize"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public ActionResult Search(int pageID = 1, int pageSize = 20, string keyword = null,string sortBy= null, string currentSort = null, bool IsOrderBy=false)
        {
            DealViewBag("GameNumberManage");
            gameMemberViewModel = getGameMemberViewModel(pageID, pageSize, keyword, sortBy, currentSort, IsOrderBy);
            return View("GameMemberManage",gameMemberViewModel);
        }
        /// <summary>
        /// 添加社团团员
        /// </summary>
        /// <param name="gameMember"></param>
        /// <returns></returns>
        public ActionResult Add(GameMember gameMember,int pageID, int pagesize, string keyword, string sortBy, string currentSort,bool CreateAccount=false, bool IsOrderBy=false)
        {
            DealViewBag("GameNumberManage");
            gameMemberViewModel = getGameMemberViewModel(pageID, pagesize, keyword, sortBy, currentSort, IsOrderBy);
            if (ModelState.IsValid)
            {
                if (EFGameMember.addGameMember(gameMember) == 1)
                {
                    TempData["Success"] = "添加成功！";
                    if (CreateAccount)
                    {
                        UserInfo userInfo = new UserInfo
                        {
                            UserID = gameMember.GameID,
                            PassWord = gameMember.GameID.ToString(),
                            UserName=gameMember.GameName,
                            Authority = EFGameMember.gameAuthorities.Count,
                        };
                        EFAllUserInfo.AddUserInfo(userInfo);
                    }
                    return RedirectToAction("GameMemberManage",new { pageID,pagesize,keyword, sortBy, currentSort, IsOrderBy });
                }
                else
                {
                    TempData["Fault"] = "添加失败！";
                    TempData["Modal"] = "AddModal";
                    return View("GameMemberManage", gameMemberViewModel);
                }
                
            }
            else
            {
                TempData["Fault"] = "请检查输入！";
                TempData["Modal"] ="AddModal";
                return View("GameMemberManage",gameMemberViewModel);
            }
        }
        /// <summary>
        /// 获取要编辑社团团员的信息
        /// </summary>
        /// <param name="gameID"></param>
        /// <param name="pageID"></param>
        /// <param name="pagesize"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Edit(int gameID,int pageID, int pagesize, string keyword, string sortBy, string currentSort, bool IsOrderBy)
        {
            DealViewBag("GameNumberManage");
            gameMemberViewModel = getGameMemberViewModel(pageID, pagesize, keyword, sortBy, currentSort, IsOrderBy);
            GameMember gameMember = EFGameMember.gameMember(gameID);
            ViewBag.GameID = gameMember.GameID;
            ViewBag.GameName = gameMember.GameName;
            ViewBag.Authority = gameMember.Authority;
            ViewBag.JoinDate = gameMember.JoinDate.ToString("yyyy-MM-dd");
            ViewBag.EditModal = true;
            return View("GameMemberManage", gameMemberViewModel);
        }
        /// <summary>
        /// 编辑保存团员信息
        /// </summary>
        /// <param name="gameMember"></param>
        /// <param name="pageID"></param>
        /// <param name="pagesize"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(GameMember gameMember,int oldGameID ,int pageID, int pagesize, string keyword, string sortBy, string currentSort, bool IsOrderBy)
        {
            DealViewBag("GameNumberManage");
            gameMemberViewModel = getGameMemberViewModel(pageID, pagesize, keyword, sortBy, currentSort, IsOrderBy);
            if (ModelState.IsValid)
            {
                if(EFGameMember.updateGameMember(gameMember,oldGameID)==1)
                {
                    TempData["Success"] = "保存成功！";
                    return RedirectToAction("GameMemberManage",new {pageID,pagesize,keyword, sortBy, currentSort, IsOrderBy });
                }
                else
                {
                    TempData["Fault"] = "保存失败！";
                    TempData["Modal"] = "EditModal";
                    return View("GameMemberManage", gameMemberViewModel);
                }
            }
            else
            {
                TempData["Fault"] = "请检查输入！";
                TempData["Modal"] = "EditModal";
                return View("GameMemberManage", gameMemberViewModel);
            }
        }
        /// <summary>
        /// 删除团员信息
        /// </summary>
        /// <param name="gameID"></param>
        /// <param name="pageID"></param>
        /// <param name="pagesize"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public ActionResult Del(int gameID, int pageID, int pagesize, string keyword, string sortBy, string currentSort, bool IsOrderBy)
        {
            DealViewBag("GameNumberManage");
            gameMemberViewModel = getGameMemberViewModel(pageID, pagesize, keyword, sortBy, currentSort, IsOrderBy);
            if (ModelState.IsValid)
            {
                if (EFGameMember.deleteGameMember(gameID)==1)
                {
                    TempData["Success"] = "删除成功！";
                    return RedirectToAction("GameMemberManage", new { pageID, pagesize, keyword, sortBy, currentSort, IsOrderBy });
                }
                else
                {
                    TempData["Fault"] = "删除失败！";
                    return View("GamememberManage", gameMemberViewModel);
                }
            }
            else
            {
                TempData["Fault"] = "请检查删除项！";
                return View("GameMemberManage",gameMemberViewModel);
            }
        }
        public ActionResult DealList(List<int> ListID,string formsubmit, GameMemberViewModel gameMemberViewModel)
        {
            if (ListID != null)
            {
                if (formsubmit == "删除")
                {
                    foreach (int item in ListID)
                    {
                        EFGameMember.deleteGameMember(item);
                    }
                    TempData["Success"] = "删除成功！";
                    return RedirectToAction("GameMemberManage",new {gameMemberViewModel.PageID,gameMemberViewModel.PageSize,gameMemberViewModel.keyword });
                }
            }
            else
            {
                TempData["Fault"] = "您未选中任何项！";
            }
            DealViewBag("GameMemberManage");
            gameMemberViewModel = updateGameMemberViewModel(gameMemberViewModel);
            return View("GameMemberManage",gameMemberViewModel);
        }

        /// <summary>
        /// 返回社团职位信息
        /// </summary>
        /// <returns></returns>
        public ActionResult Authority()
        {
            DealViewBag("GameNumberAuthority");
            return View(EFGameMember.gameAuthorities);
        }
        /// <summary>
        /// 添加职位信息
        /// </summary>
        /// <param name="gameAuthority"></param>
        /// <returns></returns>
        public ActionResult AddAuthority(GameAuthority gameAuthority)
        {
            DealViewBag("GameNumberAuthority");
            TempData["ShowAdd"] = gameAuthority.Number;
            if (ModelState.IsValid)
            {
                if (EFGameMember.addGameAuthority(gameAuthority) == 1)
                {
                    TempData["Success"] = "添加成功！";
                    TempData["ShowNumber"] = gameAuthority.Number;
                    TempData["ShowAdd"] = null;
                }
                else
                {
                    TempData["Fault"] = "已存在相同编号的职位!";
                }
            }
            else
            {
                TempData["Fault"] = "请检查输入!";
            }
            return View("Authority", EFGameMember.gameAuthorities);
        }
        /// <summary>
        /// 删除职位信息
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public ActionResult DelAuthority(int number)
        {
            DealViewBag("GameNumberAuthority");
            if (ModelState.IsValid)
            {
                if (EFGameMember.deleteGameAuthority(number) == 1)
                {
                    TempData["Success"] = "删除成功！";
                }
                else
                {
                    TempData["Fault"] = "删除失败,部分社团成员正在使用该权限!";
                }
            }
            else
            {
                TempData["Fault"] = "删除失败!";
            }
            return View("Authority", EFGameMember.gameAuthorities);
        }
        /// <summary>
        /// 更新职位信息
        /// </summary>
        /// <param name="gameAuthority"></param>
        /// <returns></returns>
        public ActionResult UpdateAuthority(GameAuthority gameAuthority)
        {
            DealViewBag("GameNumberAuthority");
            TempData["ShowNumber"] = gameAuthority.Number;
            if (ModelState.IsValid)
            {
                if (EFGameMember.updateGameAuthority(gameAuthority)==1)
                {
                    TempData["Success"] = "修改成功！";
                }
                else
                {
                    TempData["Fault"] = "修改失败，不存在此编号！";
                }
            }
            else
            {
                TempData["Fault"] = "修改失败，请检查输入！";
            }
            return View("Authority", EFGameMember.gameAuthorities);
        }
        /// <summary>
        /// 获取设置视图模型数据
        /// </summary>
        /// <param name="pageID"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public GameMemberViewModel getGameMemberViewModel(int pageID, int pageSize, string keyword,string sortBy,string currentSort, bool IsOrderBy)
        {
            ViewBag.PageSize = pageSize;
            gameMemberViewModel = new GameMemberViewModel
            {
                GameMembers = EFGameMember.gameMembers.Where(g=>g.IsDel!=true),
                GameMembersPagedList = EFGameMember.searchGameMembers(keyword).Where(g => g.IsDel != true).ToPagedList(pageID, pageSize),
                GameAuthorities = EFGameMember.gameAuthorities,
                PageID = pageID,
                PageSize = pageSize,
                keyword = keyword,
                SortBy = sortBy,
                IsOrderBy = IsOrderBy,
            };
            if (sortBy != null)
            {
                if (IsOrderBy)
                {
                    if (sortBy.Equals(currentSort))
                    {
                        gameMemberViewModel.CurrentSort = null;
                    }
                    else
                    {
                        gameMemberViewModel.CurrentSort = sortBy;
                    }
                    gameMemberViewModel.GameMembersPagedList = sortGamember(pageID, pageSize, keyword, sortBy, currentSort, IsOrderBy);
                }
                else
                {
                    gameMemberViewModel.GameMembersPagedList = sortGamember(pageID, pageSize, keyword, sortBy, (currentSort==null)?sortBy:null, IsOrderBy);
                }
                
            }
            return gameMemberViewModel;
        }
        public GameMemberViewModel updateGameMemberViewModel(GameMemberViewModel gameMemberViewModel)
        {
            ViewBag.PageSize = gameMemberViewModel.PageSize;
            gameMemberViewModel.GameMembers = EFGameMember.gameMembers;
            if (gameMemberViewModel.SortBy != null)
            {
                if (gameMemberViewModel.IsOrderBy)
                {
                    if (gameMemberViewModel.SortBy.Equals(gameMemberViewModel.CurrentSort))
                    {
                        gameMemberViewModel.CurrentSort = null;
                    }
                    else
                    {
                        gameMemberViewModel.CurrentSort = gameMemberViewModel.SortBy;
                    }
                    gameMemberViewModel.GameMembersPagedList = sortGamember(gameMemberViewModel.PageID, gameMemberViewModel.PageSize, gameMemberViewModel.keyword, gameMemberViewModel.SortBy, gameMemberViewModel.CurrentSort, gameMemberViewModel.IsOrderBy);
                }
                else
                {
                    gameMemberViewModel.GameMembersPagedList = sortGamember(gameMemberViewModel.PageID, gameMemberViewModel.PageSize, gameMemberViewModel.keyword, gameMemberViewModel.SortBy, (gameMemberViewModel.CurrentSort==null)?gameMemberViewModel.SortBy:null, gameMemberViewModel.IsOrderBy);
                }
                
            }
            gameMemberViewModel.GameAuthorities = EFGameMember.gameAuthorities;
            return gameMemberViewModel;
        }

        public PagedList<GameMember> sortGamember(int pageID, int pageSize, string keyword, string sortBy, string currentSort, bool IsOrderBy)
        {
            if (sortBy.Equals("GameID"))
            {
                if (sortBy.Equals(currentSort))
                {
                    return EFGameMember.searchGameMembers(keyword).Where(g => g.IsDel != true).OrderByDescending(g=>g.GameID).ToPagedList(pageID, pageSize);
                }
                else
                {
                    return EFGameMember.searchGameMembers(keyword).Where(g => g.IsDel != true).OrderBy(g => g.GameID).ToPagedList(pageID, pageSize);
                }
            }
            else if (sortBy.Equals("GameName"))
            {
                if (sortBy.Equals(currentSort))
                {
                    return EFGameMember.searchGameMembers(keyword).Where(g => g.IsDel != true).OrderByDescending(g => g.GameName).ToPagedList(pageID, pageSize);
                }
                else
                {
                    return EFGameMember.searchGameMembers(keyword).Where(g => g.IsDel != true).OrderBy(g => g.GameName).ToPagedList(pageID, pageSize);
                }
            }
            else if (sortBy.Equals("Authority"))
            {
                if (sortBy.Equals(currentSort))
                {
                    return EFGameMember.searchGameMembers(keyword).Where(g => g.IsDel != true).OrderByDescending(g => g.Authority).ToPagedList(pageID, pageSize);
                }
                else
                {
                    return EFGameMember.searchGameMembers(keyword).Where(g => g.IsDel != true).OrderBy(g => g.Authority).ToPagedList(pageID, pageSize);
                }
            }
            else if (sortBy.Equals("JoinDate"))
            {
                if (sortBy.Equals(currentSort))
                {
                    return EFGameMember.searchGameMembers(keyword).Where(g => g.IsDel != true).OrderByDescending(g => g.JoinDate).ToPagedList(pageID, pageSize);
                }
                else
                {
                    return EFGameMember.searchGameMembers(keyword).Where(g => g.IsDel != true).OrderBy(g => g.JoinDate).ToPagedList(pageID, pageSize);
                }
            }
            return null;
        }


        /// <summary>
        /// 批量导出社团团员
        /// </summary>
        /// <returns></returns>
        public FileResult ExportGameMember(bool IsExample=false)
        {
            //创建Excel文件的对象
            HSSFWorkbook xSSFWorkbook = new HSSFWorkbook();
            //添加一个sheet
            ISheet sheet1 = xSSFWorkbook.CreateSheet("Sheet1");
            //获取list数据
            List<GameMember> gameMembers = EFGameMember.gameMembers.Where(g=>g.IsDel!=true).OrderBy(g=>g.GameID).ToList();
            //给sheet1添加第一行的头部标题
            IRow row1 = sheet1.CreateRow(0);
            row1.CreateCell(0).SetCellValue("GameID");
            row1.CreateCell(1).SetCellValue("GameName");
            //将数据逐步写入sheet1各个行
            if (IsExample)
            {
                IRow rowtemp = sheet1.CreateRow(1);
                rowtemp.CreateCell(0).SetCellValue("141223");
                rowtemp.CreateCell(1).SetCellValue("测试昵称");
            }
            else
            {
                for (int i = 0; i < gameMembers.Count; i++)
                {
                    IRow rowtemp = sheet1.CreateRow(i + 1);
                    rowtemp.CreateCell(0).SetCellValue(gameMembers[i].GameID.ToString());
                    rowtemp.CreateCell(1).SetCellValue(gameMembers[i].GameName.ToString());
                }
            }
            // 写入到客户端 
            MemoryStream ms = new MemoryStream();
            xSSFWorkbook.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            if (IsExample)
            {
                return File(ms, "application/vnd.ms-excel", "游戏社团人员名单样本.xls");
            }
            return File(ms, "application/vnd.ms-excel", "游戏社团人员名单-"+DateTime.Now.ToString("yyyy年MM月dd日")+ ".xls");
        }

        public ActionResult ImportGameMember(HttpPostedFileBase file, int pageID, int pagesize, string keyword, string sortBy, string currentSort, bool IsOrderBy)
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
                List<GameMember> gameMembers = toList.convertToList<GameMember>(dataTable);
                foreach (var item in gameMembers)//对比数据库中的信息，把存在于Excel表中且不存在于数据库表的成员添加上去，如果存在且处于被删除的状态，设为未删除状态
                {
                    GameMember game = gameClubEntities.GameMember.Where(g=>g.GameID==item.GameID).FirstOrDefault();
                    if (game == null)
                    {
                        GameMember gameMember = new GameMember
                        {
                            GameID = item.GameID,
                            GameName = item.GameName,
                            JoinDate = DateTime.Now,
                        };
                        if (item.Authority>0)
                        {
                            gameMember.Authority = item.Authority;
                        }
                        else
                        {
                            gameMember.Authority = EFGameMember.gameAuthorities.Count;
                        }
                        gameClubEntities.GameMember.Add(gameMember);
                    }
                    else
                    {
                        game.IsDel = null;
                        game.DelTime = null;
                    }
                }
                foreach (var item in gameClubEntities.GameMember)//把存在于数据库表中且不存在于Excel表的成员设为删除状态
                {
                    if (gameMembers.Where(g => g.GameID == item.GameID).FirstOrDefault() == null)
                    {
                        item.IsDel = true;
                        item.DelTime = DateTime.Now;
                    }
                }
                gameClubEntities.SaveChanges();
            } 
            return RedirectToAction("GameMemberManage", new { pageID, pagesize, keyword, sortBy, currentSort, IsOrderBy });
        }




        /// <summary>
        /// 处理页面Menu显示
        /// </summary>
        /// <param name="setMenu"></param>
        public void DealViewBag(string setMenu)
        {
            ViewBag.GameMember = "active open";
            if (setMenu.Equals("GameNumberManage"))
            {
                ViewBag.GameNumberManage = "active";
            }
            else if (setMenu.Equals("GameNumberAuthority"))
            {
                ViewBag.GameNumberAuthority = "active";
            }
        }
    }
}