using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GameClub.Abstract;
using GameClub.Models;

namespace GameClub.Concrete
{
    public class EFAllUserInfo : IAllUserInfo
    {
        GameClubEntities gameClubEntities = new GameClubEntities();
        EFUserRecord EFUserRecord = new EFUserRecord();
        public IEnumerable<UserInfo> UserInfos { get { return gameClubEntities.UserInfo.Where(u => u.IsDel != true).OrderBy(u=>u.Authority).ThenBy(u=>u.UserID); } }

        public List<Authority> Authoritys { get { return gameClubEntities.Authority.ToList(); } }

        UserInfo IAllUserInfo.UserInfo(int UserID)
        {
            return gameClubEntities.UserInfo.Where(u => u.UserID == UserID).FirstOrDefault();
        }

        public int AddUserInfo(UserInfo userInfo)
        {
            UserInfo FinduserInfo = gameClubEntities.UserInfo.Where(u => u.UserID == userInfo.UserID).FirstOrDefault();
            if (FinduserInfo == null)
            {
                userInfo.PassWord = userInfo.UserID.ToString();
                gameClubEntities.UserInfo.Add(userInfo);
                gameClubEntities.SaveChanges();
                EFUserRecord.AddUserOperateRecord("添加了" + userInfo.UserID +" "+ userInfo.UserName + "的信息");
                return 1;
            }
            else return 0;
        }

        public UserInfo DeleteUserInfo(int UserID)
        {
            UserInfo userInfoDel = gameClubEntities.UserInfo.Where(u => u.UserID == UserID).FirstOrDefault();
            if (userInfoDel != null)
            {
                userInfoDel.IsDel = true;
                userInfoDel.DelTime = DateTime.Now;
                gameClubEntities.SaveChanges();
                EFUserRecord.AddUserOperateRecord("删除了" + userInfoDel.UserID + " " + userInfoDel.UserName + "的信息");
            }
            return userInfoDel;
        }

        public void SaveUserInfo(UserInfo userInfo,int oldUserID)
        {
            UserInfo userInfoResult = gameClubEntities.UserInfo.Where(u => u.UserID == oldUserID).FirstOrDefault();
            if (userInfoResult != null)
            {
                gameClubEntities.UserInfo.Remove(userInfoResult);
                gameClubEntities.SaveChanges();
                userInfoResult.UserID = userInfo.UserID;
                userInfoResult.UserName = userInfo.UserName;
                userInfoResult.Authority = userInfo.Authority;
                gameClubEntities.UserInfo.Add(userInfoResult);
                gameClubEntities.SaveChanges();
                EFUserRecord.AddUserOperateRecord("更新了" + userInfo.UserID + " " + userInfo.UserName + "的信息");
            }
        }

        public IEnumerable<UserInfo> SearchUser(string keyword)
        {
            if (keyword != null)
            {
                var query = gameClubEntities.UserInfo.AsQueryable();
                query = query.Where(u => u.UserName.Contains(keyword) || u.UserID.ToString().Contains(keyword)).Where(u => u.IsDel != true).OrderBy(u => u.Authority).ThenBy(u => u.UserID);
                return query;
            }
            else
            {
                return UserInfos;
            }
            
        }

        public void ResetPassword(int UserID)
        {
            UserInfo user = gameClubEntities.UserInfo.Where(u => u.UserID == UserID).FirstOrDefault();
            user.PassWord = user.UserID.ToString();
            gameClubEntities.SaveChanges();
            EFUserRecord.AddUserOperateRecord("重置了" + user.UserID + " " + user.UserName + "的密码");
        }

        public int DeleteAuthority(int number)
        {
            Authority authority = gameClubEntities.Authority.Where(u => u.Number == number).FirstOrDefault();
            if (authority != null)
            {
                if (gameClubEntities.UserInfo.Where(u => u.Authority == number).FirstOrDefault() == null)
                {
                    gameClubEntities.Authority.Remove(authority);
                    gameClubEntities.SaveChanges();
                    EFUserRecord.AddUserOperateRecord("删除了" + authority.AuthorityString + "的权限");
                    return 1;
                }
                else return 0;
            }
            else
            {
                return 0;
            }
            
        }

        public int AddAuthority(Authority authority)
        {
            Authority authoritySearch = gameClubEntities.Authority.Where(u => u.Number == authority.Number).FirstOrDefault();
            if (authoritySearch == null)
            {
                gameClubEntities.Authority.Add(authority);
                gameClubEntities.SaveChanges();
                EFUserRecord.AddUserOperateRecord("添加了" + authority.AuthorityString + "的权限");
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public int SaveAuthority(Authority authority)
        {
            Authority authorityResult = gameClubEntities.Authority.Where(a => a.Number == authority.Number).FirstOrDefault();
            if (authorityResult != null)
            {
                authorityResult.AuthorityString = authority.AuthorityString;
                authorityResult.UserManage = authority.UserManage;
                authorityResult.GameMemberManage = authority.GameMemberManage;
                authorityResult.SignManage = authority.SignManage;
                authorityResult.ContributionManage = authority.ContributionManage;
                authorityResult.MemberGroupManage = authority.MemberGroupManage;
                authorityResult.RelevantManage = authority.RelevantManage;
                gameClubEntities.SaveChanges();
                EFUserRecord.AddUserOperateRecord("更新了" + authority.AuthorityString + "的权限");
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public Authority SearchAuthority(int number)
        {
            return gameClubEntities.Authority.Where(a => a.Number == number).FirstOrDefault();
        }
    }
}