using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc.Filters;
using System.Web.Mvc;
using GameClub.Models;
using System.Reflection;

namespace GameClub.Filters
{
    public class UserAttribute : FilterAttribute,IActionFilter
    {
        private string authorityString;
        GameClubEntities gameClubEntities = new GameClubEntities();
        public UserAttribute(string authority)
        {
            this.authorityString = authority;
        }


        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session["UserID"] == null)//检查Session是否有用户信息
            {
                filterContext.Result = new RedirectResult("/Account/Login");
            }
            else if (!string.IsNullOrEmpty(authorityString))//判断当前控制器传递过来的权限是否为空
            {
                int Number = Convert.ToInt32(filterContext.HttpContext.Session["Authority"].ToString());
                //获取该用户的权限标识信息
                Authority authority = gameClubEntities.Authority.Where(a => a.Number == Number).FirstOrDefault();
                if (authority != null)
                {
                    //提取权限信息列表
                    PropertyInfo[] propertyInfos = authority.GetType().GetProperties();
                    //在列表中查找控制器所需要的权限
                    PropertyInfo ce = propertyInfos.Where(p => p.Name == authorityString).FirstOrDefault();
                    object IsAuthoity = ce.GetValue(authority, null);
                    //判断该用户的权限标识的权限信息是否允许访问当前界面
                    if (IsAuthoity == null || (!Convert.ToBoolean(IsAuthoity)))
                    {
                        filterContext.Result = new RedirectResult("/Account/login");
                    }
                }
                else
                {
                    filterContext.Result = new RedirectResult("/Account/login");
                }
            }
        }
    }
}