using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GameClub.MyHelper;
using GameClub.Infrastructure.Abstract;
using GameClub.ViewModels;
using GameClub.Abstract;
using GameClub.Models;

namespace GameClub.Controllers
{
    public class AccountController : Controller
    {
        IAuthProvider authProvider;
        IMyUserInfo EFMyUserInfo;
        IAllUserInfo EFAllUserInfo;

        public AccountController(IAuthProvider authProvider, IMyUserInfo myUserInfo, IAllUserInfo allUserInfo)
        {
            this.authProvider = authProvider;
            EFMyUserInfo = myUserInfo;
            EFAllUserInfo = allUserInfo;
        }
        /// <summary>
        /// 登录页
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(LoginViewModel loginViewModel)
        {
            string code = Session["verifyCode"].ToString().ToLower();
            if (ModelState.IsValid)
            {
                if (code.Equals(loginViewModel.verifyCode.ToLower()))
                {
                    int sign = authProvider.Authenticate(loginViewModel.UserName, loginViewModel.Password);
                    if (sign==0)
                    {
                        if (loginViewModel.UserName.Equals(loginViewModel.Password))
                        {
                            return RedirectToAction("SetPassword", new { loginViewModel.UserName });
                        }
                        return Redirect(Url.Action("Index", "Admin"));
                    }
                    else if(sign==1)
                    {
                        ModelState.AddModelError("", "用户名或密码不正确");
                        return View(loginViewModel);
                    }
                    else
                    {
                        ModelState.AddModelError("", "您的账号已被删除，请联系社团管理员！");
                        return View(loginViewModel);
                    }

                }
                else
                {
                    ModelState.AddModelError("", "验证码输入错误");
                    return View(loginViewModel);
                }
            }
            else
            {
                return View();
            }
        }
        /// <summary>
        /// 退出登录
        /// </summary>
        /// <returns></returns>
        public ActionResult Logout()
        {
            Session.Clear();
            return View("Login");
        }
        /// <summary>
        /// 验证码
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAuthCode()
        {
            VerifyCode verifyCode = new VerifyCode();
            byte[] img = verifyCode.GetVerifyCode();
            return File(img, @"image/png");
        }


        public ActionResult SetPassword(string UserName)
        {
            UserInfo result = EFAllUserInfo.UserInfos.Where(u => u.UserName == UserName).FirstOrDefault();
            if (result == null)
            {
                int UserID = Convert.ToInt32(UserName);
                result = EFAllUserInfo.UserInfos.Where(u => u.UserID == UserID).FirstOrDefault();
            }
            return View(result);
        }
        [HttpPost]
        public ActionResult SavePassword(UserInfo userInfo,string FirstPassword, string SecondPassword)
        {
            if (string.IsNullOrEmpty(FirstPassword) || string.IsNullOrEmpty(SecondPassword))
            {
                ModelState.AddModelError("", "密码不要为空！");
            }
            if (ModelState.IsValid)
            {
                if (FirstPassword.Equals(SecondPassword))
                {
                    if (!userInfo.PassWord.Equals(FirstPassword))
                    {
                        if (EFMyUserInfo.ChangePassword(userInfo.UserID, SecondPassword))
                        {
                            ViewBag.Success = "密码更新成功，正在跳转登录页！";
                        }
                        else
                        {
                            ViewBag.Fault = "不存在此用户！";
                        }
                    }
                    else
                    {
                        ViewBag.Fault = "与原密码相同!";
                    }


                }
                else
                {
                    ViewBag.Fault = "两次输入的密码不一致!";
                }
            }
            else
            {
                ViewBag.Fault = "密码不要为空！";
            }

            return PartialView("Error");
        }
    }
}