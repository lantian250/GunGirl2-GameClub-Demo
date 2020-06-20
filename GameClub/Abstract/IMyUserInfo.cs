using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameClub.Models;

namespace GameClub.Abstract
{
    public interface IMyUserInfo
    {
        UserInfo UserInfo(int userID);
        void SaveMyUserInfo(UserInfo userInfo);
        UserInfo DeleteMyUserInfo(UserInfo userInfo);
        bool ChangePassword(int UserID,string NewPassword);
        IEnumerable<UserOperateRecord> UserOperateRecords(int UserID);
        IEnumerable<UserLoginRecord> UserLoginRecords(int UserID);
    }
}
