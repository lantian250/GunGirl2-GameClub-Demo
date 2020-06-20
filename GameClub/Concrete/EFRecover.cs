using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GameClub.Models;
using GameClub.Abstract;
namespace GameClub.Concrete
{
    public class EFRecover : IRecover
    {
        GameClubEntities gameClubEntities = new GameClubEntities();
        EFUserRecord EFUserRecord=new EFUserRecord();
        public IEnumerable<UserInfo> UserInfos => gameClubEntities.UserInfo.Where(u=>u.IsDel==true).OrderByDescending(u=>u.DelTime);

        public IEnumerable<GameMember> GameMembers => gameClubEntities.GameMember.Where(g=>g.IsDel==true).OrderByDescending(g=>g.DelTime);

        public bool DelGameMember(int GameID)
        {
            GameMember gameMember = gameClubEntities.GameMember.Where(g => g.GameID == GameID).FirstOrDefault();
            if (gameMember!=null)
            {
                IEnumerable<SignInfo> signInfos = gameClubEntities.SignInfo.Where(s => s.GameID == GameID);
                foreach (var item in signInfos)
                {
                    gameClubEntities.SignInfo.Remove(item);
                }
                IEnumerable<Contribution> contributions =gameClubEntities.Contribution.Where( c=> c.GameID == GameID);
                foreach (var item in contributions)
                {
                    gameClubEntities.Contribution.Remove(item);
                }
                IEnumerable<MemberGroup> memberGroups = gameClubEntities.MemberGroup.Where(m => m.GameID == GameID);
                foreach (var item in memberGroups)
                {
                    gameClubEntities.MemberGroup.Remove(item);
                }
                IEnumerable <FillOut> fillOuts = gameClubEntities.FillOut.Where( f=> f.GameID == GameID);
                foreach (var item in fillOuts)
                {
                    gameClubEntities.FillOut.Remove(item);
                }
                gameClubEntities.GameMember.Remove(gameMember);
                gameClubEntities.SaveChanges();
                EFUserRecord.AddUserOperateRecord("永久删除了游戏ID为" + GameID+ "的所有信息");
                return true;
            }
            return false;
        }

        public bool DelUserInfo(int UserID)
        {
            UserInfo userInfo = gameClubEntities.UserInfo.Where(u => u.UserID == UserID).FirstOrDefault();
            if (userInfo!=null)
            {
                IEnumerable<Feedback> feedbacks = gameClubEntities.Feedback.Where(f => f.UserID == UserID);
                foreach (var item in feedbacks)
                {
                    gameClubEntities.Feedback.Remove(item);
                }
                IEnumerable<UserLoginRecord> userLoginRecords = gameClubEntities.UserLoginRecord.Where(u => u.UserID == UserID);
                foreach (var item in userLoginRecords)
                {
                    gameClubEntities.UserLoginRecord.Remove(item);
                }
                IEnumerable<UserOperateRecord> userOperateRecords = gameClubEntities.UserOperateRecord.Where(u => u.UserID == UserID);
                foreach (var item in userOperateRecords)
                {
                    gameClubEntities.UserOperateRecord.Remove(item);
                }
                gameClubEntities.UserInfo.Remove(userInfo);
                gameClubEntities.SaveChanges();
                EFUserRecord.AddUserOperateRecord("永久删除了用户ID为" + UserID+ "的所有信息");
                return true;
            }
            return false;
        }

        public bool ResetGameMember(int GameID)
        {
            if (GameID > 0)
            {
                GameMember gameMember = gameClubEntities.GameMember.Where(g => g.GameID == GameID).FirstOrDefault();
                if (gameMember != null)
                {
                    gameMember.IsDel = null;
                    gameMember.DelTime = null;
                    gameClubEntities.SaveChanges();
                    EFUserRecord.AddUserOperateRecord("恢复了游戏ID为" + GameID + "的所有信息");
                    return true;
                }
            }
            return false;
        }

        public bool ResetUserInfo(int UserID)
        {
            if (UserID > 0)
            {
                UserInfo userInfo = gameClubEntities.UserInfo.Where(u => u.UserID == UserID).FirstOrDefault();
                if (userInfo != null)
                {
                    userInfo.IsDel = null;
                    userInfo.DelTime = null;
                    gameClubEntities.SaveChanges();
                    EFUserRecord.AddUserOperateRecord("恢复了游戏ID为" + UserID + "的所有信息");
                    return true;
                }
            }
            return false;
        }
    }
}