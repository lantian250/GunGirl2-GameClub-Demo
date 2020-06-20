using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameClub.Models;

namespace GameClub.Abstract
{
    public interface IAllUserInfo
    {
        IEnumerable<UserInfo> UserInfos { get; }
        UserInfo UserInfo(int UserID);
        List<Authority> Authoritys { get; }
        void SaveUserInfo(UserInfo userInfo,int oldUserID);
        UserInfo DeleteUserInfo(int UserID);
        int AddUserInfo(UserInfo userInfo);
        IEnumerable<UserInfo> SearchUser(string keyword);
        void ResetPassword(int UserID);
        int DeleteAuthority(int number);
        int AddAuthority(Authority authority);
        int SaveAuthority(Authority authority);
        Authority SearchAuthority(int number);
    }
}
