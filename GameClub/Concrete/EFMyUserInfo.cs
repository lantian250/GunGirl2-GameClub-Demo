using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GameClub.Abstract;
using GameClub.Models;

namespace GameClub.Concrete
{
    public class EFMyUserInfo : IMyUserInfo
    {
        GameClubEntities gameClubEntities = new GameClubEntities();
        EFUserRecord EFUserRecord = new EFUserRecord();
        public UserInfo UserInfo(int userID)
        {
            UserInfo userInfo = gameClubEntities.UserInfo.Where(u => u.UserID == userID).FirstOrDefault();
            return userInfo;
        }
        public UserInfo DeleteMyUserInfo(UserInfo userInfo)
        {
            UserInfo userInfoDel = gameClubEntities.UserInfo.Where(u => u.UserID == userInfo.UserID).FirstOrDefault();
            if (userInfoDel != null)
            {
                gameClubEntities.UserInfo.Remove(userInfoDel);
                gameClubEntities.SaveChanges();
                EFUserRecord.AddUserOperateRecord("删除个人信息");
            }
            return userInfoDel;
        }

        public void SaveMyUserInfo(UserInfo userInfo)
        {
            UserInfo userInfoResult = gameClubEntities.UserInfo.Where(u => u.UserID == userInfo.UserID).FirstOrDefault();
            if (userInfoResult != null)
            {
                userInfoResult.UserName = userInfo.UserName;
                userInfoResult.PassWord = userInfo.PassWord;
                userInfoResult.ImageData = userInfoResult.ImageData;
                userInfoResult.ImageMimeType = userInfoResult.ImageMimeType;
            }
            EFUserRecord.AddUserOperateRecord("更新个人信息");
            gameClubEntities.SaveChanges();
        }

        public bool ChangePassword(int UserID, string NewPassword)
        {
            UserInfo userInfoResult = gameClubEntities.UserInfo.Where(u => u.UserID == UserID).FirstOrDefault();
            if (userInfoResult != null)
            {
                userInfoResult.PassWord = NewPassword;
                gameClubEntities.SaveChanges();
                EFUserRecord.AddUserOperateRecord("修改个人密码");
                return true;
            }
            return false;
        }
        public IEnumerable<UserLoginRecord> UserLoginRecords(int UserID)
        {
            return gameClubEntities.UserLoginRecord.Where(u => u.UserID == UserID);
        }

        public IEnumerable<UserOperateRecord> UserOperateRecords(int UserID)
        {
            return gameClubEntities.UserOperateRecord.Where(u => u.UserID == UserID);
        }
    }
}