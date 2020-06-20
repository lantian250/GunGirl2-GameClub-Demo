using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GameClub.Abstract;
using GameClub.Models;

namespace GameClub.Concrete
{
    public class EFUserRecord : IUserRecord
    {
        GameClubEntities gameClubEntities = new GameClubEntities();
        public IEnumerable<UserLoginRecord> UserLoginRecords => gameClubEntities.UserLoginRecord;

        public IEnumerable<UserOperateRecord> UserOperateRecords => gameClubEntities.UserOperateRecord;

        public bool AddUserLoginRecord(UserLoginRecord userLoginRecord)
        {
            if (userLoginRecord != null)
            {
                gameClubEntities.UserLoginRecord.Add(userLoginRecord);
                gameClubEntities.SaveChanges();
                return true;
            }
            return false;
        }

        public bool AddUserOperateRecord(string OperateContext)
        {
            if (!string.IsNullOrEmpty(OperateContext))
            {
                UserOperateRecord userOperateRecord = new UserOperateRecord
                {
                    UserID = Convert.ToInt32(HttpContext.Current.Session["UserID"].ToString()),
                    UserName = HttpContext.Current.Session["UserName"].ToString(),
                    OperateContext = OperateContext,
                    RecordTime = DateTime.Now,
                };
                gameClubEntities.UserOperateRecord.Add(userOperateRecord);
                gameClubEntities.SaveChanges();
                return true;
            }
            return false;
        }
    }
}