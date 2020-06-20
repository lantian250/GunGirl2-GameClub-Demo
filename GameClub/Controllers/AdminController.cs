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
using System.IO;
using Webdiyer.WebControls.Mvc;

namespace GameClub.Controllers
{
    [User(null)]
    public class AdminController : Controller
    {
        //数据操纵
        IMyUserInfo myUserInfo;
        IInformMessage EFInformMessage;
        //视图模型
        MyUserInfoViewModel myUserInfoView;
        ChangePasswordViewModel changePasswordView;


        public AdminController(IMyUserInfo myUserInfo,IAllUserInfo allUserInfo,IInformMessage informMessage)
        {
            this.myUserInfo = myUserInfo;
            EFInformMessage = informMessage;
        }
        /// <summary>
        /// 首页
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index(bool IsRead=false)
        {
            ViewBag.Index = "active";
            myUserInfoView = new MyUserInfoViewModel();
            if (!IsRead)
            {
                InformMessage informMessage = EFInformMessage.InformMessages.OrderByDescending(i => i.CreateTime).FirstOrDefault();
                if (informMessage != null)
                {
                    if (DateTime.Now.AddDays(-3).CompareTo(Convert.ToDateTime(informMessage.CreateTime)) < 0)
                    {
                        myUserInfoView.InformMessage = informMessage;
                    }
                }
            }
            return View(myUserInfoView);
            
        }
        /// <summary>
        /// 个人信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult MyUserInfo()
        {
            ViewBag.UserInfo = "active open";
            ViewBag.MyUserInfo = "active";
            TempData.Clear();
            int userID = Convert.ToInt32(Session["UserID"].ToString());
            myUserInfoView = new MyUserInfoViewModel
            {
                UserInfo = myUserInfo.UserInfo(userID),
            };
            return View(myUserInfoView);
        }
        /// <summary>
        /// 个人信息
        /// </summary>
        /// <param name="userInfo">个人信息</param>
        /// <param name="image">上传图片数据</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult MyUserInfo(MyUserInfoViewModel userInfo, HttpPostedFileBase image = null)
        {
            ViewBag.UserInfo = "active open";
            ViewBag.MyUserInfo = "active";
            if (ModelState.IsValid)
            {
                UserInfo userInfos = myUserInfo.UserInfo(userInfo.UserID);
                userInfos.UserName = userInfo.UserName;
                if (image != null)
                {
                    userInfos.ImageMimeType = image.ContentType;
                    userInfos.ImageData = new byte[image.ContentLength];
                    image.InputStream.Read(userInfos.ImageData, 0, image.ContentLength);
                }
                myUserInfo.SaveMyUserInfo(userInfos);
                myUserInfoView = new MyUserInfoViewModel
                {
                    UserInfo = userInfos,
                };
                ViewBag.Success = "true";
                return View(myUserInfoView);
            }
            else
            {
                myUserInfoView = new MyUserInfoViewModel
                {
                    UserInfo = myUserInfo.UserInfo(userInfo.UserID),
                };
                ViewBag.Fault = "False";
                return View(myUserInfoView);
            }
        }
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ChangePassword()
        {
            ViewBag.UserInfo = "active open";
            ViewBag.ChangePassword = "active";
            int userID = Convert.ToInt32(Session["UserID"].ToString());
            changePasswordView = new ChangePasswordViewModel{ UserID = userID };
            return View( changePasswordView);
        }
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="changePasswordView"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordViewModel changePasswordView)
        {
            ViewBag.UserInfo = "active open";
            ViewBag.ChangePassword = "active";
            if (ModelState.IsValid)
            {
                UserInfo user = myUserInfo.UserInfo(changePasswordView.UserID);
                if(user.PassWord== changePasswordView.Password)
                {
                    user.PassWord = changePasswordView.ChangePassword;
                    myUserInfo.SaveMyUserInfo(user);
                    ViewBag.Success = "true";
                    return View(changePasswordView);
                }
                else
                {
                    ModelState.AddModelError("PasswordError", "原密码错误！");
                    //ViewBag.Fault = "False";
                    return View(changePasswordView);
                }
                
            }
            else
            {
                return View(changePasswordView);
            }
        }


        public ActionResult UserLoginRecord(int PageIndex = 1)
        {
            ViewBag.UserInfo = "active open";
            ViewBag.MyUserLoginRecord = "active";
            int userID = Convert.ToInt32(Session["UserID"].ToString());
            var model = myUserInfo.UserLoginRecords(userID).OrderByDescending(u=>u.RecordTime).ToPagedList(PageIndex,20);
            if (Request.IsAjaxRequest())
            {
                return PartialView("_UserLoginRecord", model);
            }
            return View(model);
        }

        public ActionResult UserOperateRecord(int PageIndex = 1,string keyword=null)
        {
            ViewBag.UserInfo = "active open";
            ViewBag.MyUserOperateRecord = "active";
            int userID = Convert.ToInt32(Session["UserID"].ToString());
            var model = myUserInfo.UserOperateRecords(userID).OrderByDescending(u => u.RecordTime).ToPagedList(PageIndex, 20);
            if (keyword != null)
            {
                model = myUserInfo.UserOperateRecords(userID).Where(u => u.OperateContext.ToString().Contains(keyword)).OrderByDescending(u => u.RecordTime).ToPagedList(PageIndex, 20);
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView("_UserOperateRecord", model);
            }
            return View(model);
        }


        /// <summary>
        /// 获取当前用户的图片信息
        /// </summary>
        /// <returns></returns>
        public FileContentResult GetImage()
        {
            int userID = Convert.ToInt32(Session["UserID"].ToString());
            UserInfo userInfo = myUserInfo.UserInfo(userID);
            if (userInfo.ImageData != null)
            {
                return File(userInfo.ImageData, userInfo.ImageMimeType);
            }
            else
            {
                FileStream fs = new FileStream(Server.MapPath("..\\Content\\images\\users\\avatar.png"), FileMode.Open, FileAccess.Read);
                byte[] image = new byte[fs.Length];
                fs.Read(image, 0, image.Length);
                fs.Close();
                return File(image,"image/png");
            }
        }

    }
}