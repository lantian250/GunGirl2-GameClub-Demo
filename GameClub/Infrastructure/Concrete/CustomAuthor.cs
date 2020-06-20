using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GameClub.Infrastructure.Abstract;
using GameClub.Models;
using GameClub.Abstract;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GameClub.Infrastructure.Concrete
{
    public class CustomAuthor : IAuthProvider
    {
        GameClubEntities db = new GameClubEntities();
        IUserRecord EFUserRecord;
        public CustomAuthor(IUserRecord userRecord)
        {
            EFUserRecord = userRecord;
        }

        public int Authenticate(string username, string password)
        {
            UserInfo result = db.UserInfo.Where(u => (u.UserName == username) && u.PassWord == password).FirstOrDefault();
            if (result == null)
            {
                bool f = true;
                foreach (var item in username)
                {
                    if (!char.IsNumber(item))
                    {
                        f = false;
                        break;
                    }
                }
                if (f&&username.Length <= 10)
                {
                    int UserID = Convert.ToInt32(username);
                    result = db.UserInfo.Where(u => (u.UserID == UserID) && u.PassWord == password).FirstOrDefault();
                }
               
            }
            if (result != null)
            {
                if (result.IsDel == true)
                {
                    return 2;
                }
                HttpContext.Current.Session["UserID"] = result.UserID;
                HttpContext.Current.Session["UserName"] = result.UserName;
                HttpContext.Current.Session["Authority"] = result.Authority;

                UserLoginRecord userLoginRecord = new UserLoginRecord
                {
                    UserID = result.UserID,
                    UserName = result.UserName,
                    HostIP = GetWebClientIp(),
                    RecordTime = DateTime.Now,
                };
                if (userLoginRecord.HostIP.ToString().Equals("::1"))
                {
                    userLoginRecord.HostIP = "127.0.0.1";
                    userLoginRecord.Area = "本地局域网访问";
                }
                else
                {
                    string api = "https://apis.map.qq.com/ws/location/v1/ip?ip=" + userLoginRecord.HostIP.ToString() + "&key=IHRBZ-LKGAR-I4KWJ-W2YF7-2NBU3-SEBL3";
                    JObject json = (JObject)JsonConvert.DeserializeObject(HttpGet(api));
                    if (json["status"].ToString().Equals("0"))
                    {
                        userLoginRecord.Area = json["result"]["ad_info"]["nation"].ToString() + json["result"]["ad_info"]["province"].ToString() + json["result"]["ad_info"]["city"].ToString() + json["result"]["ad_info"]["district"].ToString();
                    }
                    else
                    {
                        userLoginRecord.Area = json["message"].ToString();
                    }
                }
                EFUserRecord.AddUserLoginRecord(userLoginRecord);
                return 0;
            }
            else
            {
                return 1;
            }
        }

        /// <summary>
        /// get获取json
        /// </summary>
        /// <param name="api"></param>
        /// <returns></returns>
        private string HttpGet(string api)
        {
            string serviceAddress = api;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(serviceAddress);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
            //返回json字符串
            return myStreamReader.ReadToEnd();
        }


        /// <summary>
        /// 获取web客户端ip
        /// </summary>
        /// <returns></returns>
        public static string GetWebClientIp()
        {

            string userIP = "未获取用户IP";

            try
            {
                if (System.Web.HttpContext.Current == null
                 || System.Web.HttpContext.Current.Request == null
                 || System.Web.HttpContext.Current.Request.ServerVariables == null)
                {
                    return "";
                }

                string CustomerIP = "";

                //CDN加速后取到的IP simone 090805
                CustomerIP = System.Web.HttpContext.Current.Request.Headers["Cdn-Src-Ip"];
                if (!string.IsNullOrEmpty(CustomerIP))
                {
                    return CustomerIP;
                }

                CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                if (!String.IsNullOrEmpty(CustomerIP))
                {
                    return CustomerIP;
                }

                if (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
                {
                    CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                    if (CustomerIP == null)
                    {
                        CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    }
                }
                else
                {
                    CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }

                if (string.Compare(CustomerIP, "unknown", true) == 0 || String.IsNullOrEmpty(CustomerIP))
                {
                    return System.Web.HttpContext.Current.Request.UserHostAddress;
                }
                return CustomerIP;
            }
            catch { }

            return userIP;

        }
    }
}