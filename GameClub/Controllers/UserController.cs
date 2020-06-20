using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GameClub.Abstract;
using GameClub.ViewModels;
using System.ComponentModel.DataAnnotations;
using GameClub.Filters;
using GameClub.Models;
using Webdiyer.WebControls.Mvc;

namespace GameClub.Controllers
{
    [User("UserManage")]
    public class UserController : Controller
    {
        //数据操纵
        IAllUserInfo allUserInfo;
        IGameMember EFGameMember;
        //视图模型
        AllUserInfoViewModel AllUserInfoView;
        public UserController(IAllUserInfo allUserInfo, IGameMember gameMember)
        {
            this.allUserInfo = allUserInfo;
            this.EFGameMember = gameMember;
        }
        /// <summary>
        /// 用户管理
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <param name="id"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public ActionResult Index(string returnUrl, int id = 1, string keyword = null, int pagesize = 20, string sortBy = null, string currentSort = null)
        {
            DealViewBag("AllUserInfoManage");
            AllUserInfoView = updateAllUserInfoViewModel(returnUrl, id, keyword, pagesize, sortBy, currentSort, false);
            return View(AllUserInfoView);
        }
        public ActionResult IndexOrder(string returnUrl, int id = 1, string keyword = null, int pagesize = 20, string sortBy = null, string currentSort = null)
        {
            DealViewBag("AllUserInfoManage");
            AllUserInfoView = updateAllUserInfoViewModel(returnUrl, id, keyword, pagesize, sortBy, currentSort, true);
            return View("Index",AllUserInfoView);
        }
        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <param name="id"></param>
        /// <param name="keyword"></param>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public ActionResult Add(string returnUrl, int id, string keyword, UserInfo userInfo, int pagesize, string sortBy, string currentSort)
        {
            DealViewBag("AllUserInfoManage");
            if (userInfo.UserID == 0)
            {
                ModelState.AddModelError("UserIDError", "用户ID不能为空!");
            }
            if (string.IsNullOrEmpty(userInfo.UserName))
            {
                ModelState.AddModelError("UserName", "用户昵称不能为空!");
            }
            AllUserInfoView = updateAllUserInfoViewModel(returnUrl, id, keyword, pagesize, sortBy, currentSort, false);
            if (ModelState.IsValid)
            {
                if (allUserInfo.AddUserInfo(userInfo) == 1)
                {
                    TempData["Success"] = "true";
                    return RedirectToAction("index", new { returnUrl, id, keyword, pagesize, sortBy, currentSort });
                }
                else
                {
                    //TempData["Fault"] = "false";
                    TempData["Fault"] = "该用户ID已存在！";
                    TempData["Modal"] = "AddModal";
                    return View("Index", AllUserInfoView);
                }
            }
            else
            {
                //TempData["Fault"] = "false";
                TempData["Fault"] = "添加失败！";
                TempData["Modal"] = "AddModal";
                return View("Index", AllUserInfoView);
            }
        }
        /// <summary>
        /// 打开编辑用户信息
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="returnUrl"></param>
        /// <param name="id"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Edit(int UserID, string returnUrl, int id, string keyword, int pagesize, string sortBy, string currentSort)
        {
            DealViewBag("AllUserInfoManage");
            UserInfo userInfo = allUserInfo.UserInfo(UserID);
            ViewBag.UserName = userInfo.UserName;
            ViewBag.UserID = userInfo.UserID;
            ViewBag.Authority = userInfo.Authority;
            ViewBag.EditModal = "true";
            AllUserInfoView = updateAllUserInfoViewModel(returnUrl, id, keyword, pagesize, sortBy, currentSort,false);
            return View("Index", AllUserInfoView);
        }
        /// <summary>
        /// 提交保存用户信息
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <param name="id"></param>
        /// <param name="keyword"></param>
        /// <param name="userInfo"></param>
        /// <param name="oldUserID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(string returnUrl, int id, string keyword, int pagesize, UserInfo userInfo, int oldUserID, string sortBy, string currentSort)
        {
            DealViewBag("AllUserInfoManage");
            ViewBag.UserName = oldUserID;
            ViewBag.UserID = userInfo.UserID;
            ViewBag.Authority = userInfo.Authority;
            if (userInfo.UserID == 0)
            {
                ModelState.AddModelError("UserIDError", "用户ID不能为空!");
            }
            if (string.IsNullOrEmpty(userInfo.UserName))
            {
                ModelState.AddModelError("UserName", "用户昵称不能为空!");
            }
            AllUserInfoView = updateAllUserInfoViewModel(returnUrl, id, keyword, pagesize, sortBy, currentSort, false);
            if (ModelState.IsValid)
            {
                TempData["Success"] = "添加成功！";
                allUserInfo.SaveUserInfo(userInfo, oldUserID);
                //return RedirectToAction("Index", new { id });
                return RedirectToAction("index", new { returnUrl, id, keyword, pagesize, sortBy, currentSort });
            }
            else
            {
                //TempData["Fault"] = "false";
                TempData["Fault"] = "修改失败";
                TempData["Modal"] = "EditModal";
                return View("Index", AllUserInfoView);
            }
        }
        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public ActionResult Delete(string returnUrl, int id, int userID, string keyword, int pagesize, string sortBy, string currentSort)
        {
            DealViewBag("AllUserInfoManage");
            AllUserInfoView = updateAllUserInfoViewModel(returnUrl, id, keyword, pagesize, sortBy, currentSort, false);
            if (userID != 0)
            {
                if (userID == Convert.ToInt32(Session["UserID"].ToString()))
                {
                    //TempData["Fault"] = "false";
                    TempData["Fault"] = "禁止删除自己!";
                    return View("Index", AllUserInfoView);
                }
                else
                {
                    allUserInfo.DeleteUserInfo(userID);
                    TempData["Success"] = "删除成功！";
                    //return RedirectToAction("Index",new { id });
                    return RedirectToAction("Index", new { returnUrl, id, keyword, pagesize, sortBy, currentSort });
                }

            }
            else
            {
                return View("Index", AllUserInfoView);
            }

        }
        /// <summary>
        /// 多选删除用户
        /// </summary>
        /// <param name="ListID"></param>
        /// <param name="returnUrl"></param>
        /// <param name="id"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public ActionResult DeleteList(List<int> ListID, string returnUrl, int id, string keyword, int pagesize, string sortBy, string currentSort)
        {
            DealViewBag("AllUserInfoManage");
            if (ListID != null)
            {
                foreach (int item in ListID)
                {
                    if(item == Convert.ToInt32(Session["UserID"].ToString()))
                    {
                        continue;
                    }
                    allUserInfo.DeleteUserInfo(item);
                }
                TempData["Success"] = "删除成功";
            }
            //return RedirectToAction("Index",new { id});
            return RedirectToAction("index", new { returnUrl, id, keyword, pagesize, sortBy, currentSort });
        }
        /// <summary>
        /// 重置用户密码
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>

        public ActionResult Reset(string returnUrl, int id, int userID, string keyword, int pagesize, string sortBy, string currentSort)
        {
            //DealViewBag("AllUserInfoManage");
            allUserInfo.ResetPassword(userID);
            TempData["Success"] = "重置密码成功!";
            return RedirectToAction("index", new { returnUrl, id, keyword, pagesize, sortBy, currentSort });
        }

        /// <summary>
        /// 获取所有权限职责信息
        /// </summary>
        /// <returns></returns>
        public ActionResult Authority()
        {
            DealViewBag("UserAuthority");
            return View(allUserInfo.Authoritys);
        }
        /// <summary>
        /// 添加职权信息
        /// </summary>
        /// <param name="authority"></param>
        /// <returns></returns>
        public ActionResult AddAuthority(Authority authority)
        {
            DealViewBag("UserAuthority");
            if (ModelState.IsValid)
            {
                if (allUserInfo.AddAuthority(authority) == 1)
                {
                    TempData["Success"] = "添加成功";
                }
                else
                {
                    TempData["Fault"] = "添加失败，该编号已存在！";
                    ViewBag.Number = authority.Number;
                    ViewBag.AuthorityString = authority.AuthorityString;
                }
            }
            else
            {
                TempData["Fault"] = "添加失败！请检查输入内容！";
            }
            return View("Authority", allUserInfo.Authoritys);
        }
        /// <summary>
        /// 删除职权信息
        /// </summary>
        /// <param name="authority"></param>
        /// <returns></returns>
        public ActionResult DelAuthority(int number)
        {
            DealViewBag("UserAuthority");
            if (number != 1)
            {
                if (allUserInfo.DeleteAuthority(number) == 1)
                {
                    TempData["Success"] = "删除成功！";
                }
                else
                {
                    TempData["Fault"] = "删除失败,部分用户正在使用该权限！";
                }
            }
            else
            {
                TempData["Fault"] = "删除失败,至少要保留一个管理员权限！";
            }
            return View("Authority", allUserInfo.Authoritys);
        }
        [HttpGet]
        public ActionResult EditAuthority(int number)
        {
            DealViewBag("UserAuthority");
            Authority authority = allUserInfo.SearchAuthority(number);
            ViewBag.Number = authority.Number;
            ViewBag.AuthorityString = authority.AuthorityString;
            ViewBag.EditAuthorityModal = true;
            ViewBag.UserManage = authority.UserManage;
            ViewBag.GameMemberManage = authority.GameMemberManage;
            ViewBag.SignManage = authority.SignManage;
            ViewBag.ContributionManage = authority.ContributionManage;
            ViewBag.MemberGroupManage = authority.MemberGroupManage;
            ViewBag.RelevantManage = authority.RelevantManage;
            return View("Authority", allUserInfo.Authoritys);
        }
        /// <summary>
        /// 修改职权信息
        /// </summary>
        /// <param name="authority"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditAuthority(Authority authority)
        {
            DealViewBag("UserAuthority");
            if (ModelState.IsValid)
            {
                if (allUserInfo.SaveAuthority(authority) == 1)
                {
                    TempData["Success"] = "修改成功！";
                }
                else
                {
                    TempData["Fault"] = "修改失败！";
                    ViewBag.EditAuthorityModal = true;
                }
            }
            else
            {
                TempData["Fault"] = "修改失败！";
                ViewBag.EditAuthorityModal = true;
            }
            return View("Authority", allUserInfo.Authoritys);
        }


        //public AllUserInfoViewModel updateAllUserInfoViewModel(string returnUrl, int id)
        //{
        //    AllUserInfoView = new AllUserInfoViewModel
        //    {
        //        UserInfos = allUserInfo.UserInfos,
        //        Authorities = allUserInfo.Authoritys,
        //        UserInfosPageList = allUserInfo.UserInfos.ToPagedList(id, 15),
        //        PageID = id,
        //        returnUrl=returnUrl,
        //    };
        //    return AllUserInfoView;
        //}
        /// <summary>
        /// 更新视图模型信息
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <param name="id"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public AllUserInfoViewModel updateAllUserInfoViewModel(string returnUrl, int id, string keyword, int pagesize, string sortBy, string currentSort,bool isorderby)
        {
            ViewBag.PageSize = pagesize;
            //if (keyword == null)
            //{
            //    AllUserInfoView = new AllUserInfoViewModel
            //    {
            //        UserInfos = allUserInfo.UserInfos,
            //        Authorities = allUserInfo.Authoritys,
            //        UserInfosPageList = allUserInfo.UserInfos.ToPagedList(id, pagesize),
            //        PageID = id,
            //        returnUrl = returnUrl,
            //        SortBy=sortBy
            //    };
            //}
            //else
            //{
            AllUserInfoView = new AllUserInfoViewModel
            {
                UserInfos = allUserInfo.UserInfos.Where(u => u.IsDel != true),
                Authorities = allUserInfo.Authoritys,
                UserInfosPageList = allUserInfo.SearchUser(keyword).Where(u => u.IsDel != true).ToPagedList(id, pagesize),
                PageID = id,
                PageSize = pagesize,
                returnUrl = returnUrl,
                keyword = keyword,
                SortBy = sortBy,
                GameMembers = GetGM(),
                CurrentSort=currentSort
            };
            //}
            if (sortBy != null)
            {
                if (isorderby)
                {
                    if (sortBy.Equals(currentSort))
                    {
                        AllUserInfoView.CurrentSort = null;
                    }
                    else
                    {
                        AllUserInfoView.CurrentSort = sortBy;
                    }
                    AllUserInfoView.UserInfosPageList = sortUserInfo(id, keyword, pagesize, sortBy, currentSort);
                }
                else
                {
                    if (sortBy.Equals(currentSort))
                    {
                        currentSort = null;
                    }
                    else
                    {
                        currentSort = sortBy;
                    }
                    AllUserInfoView.UserInfosPageList = sortUserInfo(id, keyword, pagesize, sortBy, currentSort);
                }
            }
            return AllUserInfoView;
        }

        public PagedList<UserInfo> sortUserInfo(int id, string keyword, int pagesize, string sortBy, string currentSort)
        {
            if (sortBy.Equals("UserID"))
            {
                if (sortBy.Equals(currentSort))
                {
                    return allUserInfo.SearchUser(keyword).Where(u => u.IsDel != true).OrderByDescending(u => u.UserID).ToPagedList(id, pagesize);
                }
                else
                {
                    return allUserInfo.SearchUser(keyword).Where(u => u.IsDel != true).OrderBy(u => u.UserID).ToPagedList(id, pagesize);
                    //return allUserInfo.SearchUser(keyword).ToPagedList(id, pagesize);
                }
            }
            else if (sortBy.Equals("UserName"))
            {
                if (sortBy.Equals(currentSort))
                {
                    return allUserInfo.SearchUser(keyword).Where(u => u.IsDel != true).OrderByDescending(u => u.UserName).ToPagedList(id, pagesize);
                }
                else
                {
                    return allUserInfo.SearchUser(keyword).Where(u => u.IsDel != true).OrderBy(u => u.UserName).ToPagedList(id, pagesize);
                }
            }
            else if (sortBy.Equals("Authority"))
            {
                if (sortBy.Equals(currentSort))
                {
                    return allUserInfo.SearchUser(keyword).Where(u => u.IsDel != true).OrderByDescending(u => u.Authority).ToPagedList(id, pagesize);
                }
                else
                {
                    return allUserInfo.SearchUser(keyword).Where(u => u.IsDel != true).OrderBy(u => u.Authority).ToPagedList(id, pagesize);
                }
            }
            return null;
        }

        public List<GameMember> GetGM()
        {
            List<GameMember> gameMembers = new List<GameMember>();
            foreach (var item in EFGameMember.gameMembers)
            {
                if (allUserInfo.UserInfo(item.GameID) == null)
                {
                    gameMembers.Add(item);
                }
            }
            return gameMembers;
        }

        public ActionResult AddGM(string returnUrl, int id, string keyword, List<string> GameMember, int pagesize, string sortBy, string currentSort)
        {
            bool f = true;
            DealViewBag("AllUserInfoManage");
            AllUserInfoView = updateAllUserInfoViewModel(returnUrl, id, keyword, pagesize, sortBy, currentSort,false);
            if (ModelState.IsValid)
            {
                foreach (var item in GameMember)
                {
                    string[] Temp = item.Split('.');
                    UserInfo userInfo = new UserInfo
                    {
                        UserID = Convert.ToInt32(Temp[0]),
                        UserName = Temp[1],
                        Authority=allUserInfo.Authoritys.Count
                    };
                    if (allUserInfo.AddUserInfo(userInfo) == 0)
                    {
                        f = false;
                    }
                }
                if (f)
                {
                    TempData["Success"] = "添加成功";
                    return RedirectToAction("index", new { returnUrl, id, keyword, pagesize, sortBy, currentSort });
                }
                else
                {
                    TempData["Fault"] = "部分团员已存在账号！";
                    TempData["Modal"] = "AddGMModal";
                    return View("Index", AllUserInfoView);
                }
            }
            else
            {
                //TempData["Fault"] = "false";
                TempData["Fault"] = "添加失败！";
                TempData["Modal"] = "AddGMModal";
                return View("Index", AllUserInfoView);
            }
        }


        /// <summary>
        /// 更新网页端动态变化
        /// </summary>
        /// <param name="setMenu"></param>
        public void DealViewBag(string setMenu)
        {
            ViewBag.AllUserInfo = "active open";
            if (setMenu.Equals("AllUserInfoManage"))
            {
                ViewBag.AllUserInfoManage = "active";
            }
            else if (setMenu.Equals("UserAuthority"))
            {
                ViewBag.UserAuthority = "active";
            }
        }
    }
}