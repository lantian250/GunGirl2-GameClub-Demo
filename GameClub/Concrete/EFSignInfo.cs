using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GameClub.Abstract;
using GameClub.Models;

namespace GameClub.Concrete
{
    public class EFSignInfo : ISignInfo
    {
        GameClubEntities gameClubEntities = new GameClubEntities();
        EFUserRecord EFUserRecord = new EFUserRecord();
        public IEnumerable<SignInfo> signInfos { get { return gameClubEntities.SignInfo.OrderByDescending(s=>s.SignID).ThenBy(s=>s.GameID); } }
        public IEnumerable<SignList> signLists { get { return gameClubEntities.SignList.OrderByDescending(s => s.SignID); } }

        public bool ActiveAbleList(List<string> ListID)
        {
            bool f = true;
            foreach (var item in ListID)
            {
                SignList signList = gameClubEntities.SignList.Where(s => s.SignID == item).FirstOrDefault();
                if (signList != null)
                {
                    signList.Active = true;
                    gameClubEntities.SaveChanges();
                }
                else
                {
                    f = false;
                }
                EFUserRecord.AddUserOperateRecord("启用签到表ID为" + signList.SignID+"的签到表");
            }
            return f;
        }

        public bool ActiveDisableList(List<string> ListID)
        {
            bool f = true;
            foreach (var item in ListID)
            {
                SignList signList = gameClubEntities.SignList.Where(s => s.SignID == item).FirstOrDefault();
                if (signList != null)
                {
                    signList.Active = false;
                    gameClubEntities.SaveChanges();
                }
                else
                {
                    f = false;
                }
                EFUserRecord.AddUserOperateRecord("禁用签到表ID为" + signList.SignID + "的签到表");
            }
            return f;
        }

        /// <summary>
        /// 增加一条签到信息记录
        /// </summary>
        /// <param name="signInfo"></param>
        /// <returns>true插入成功，false插入失败，已存在信息记录</returns>
        public bool AddSignInfo(SignInfo signInfo)
        {
            SignInfo signInfoResult = gameClubEntities.SignInfo.Where(s => (s.SignID == signInfo.SignID)&&(s.GameID==signInfo.GameID)).FirstOrDefault();
            if (signInfoResult == null)
            {
                gameClubEntities.SignInfo.Add(signInfo);
                gameClubEntities.SaveChanges();
                EFUserRecord.AddUserOperateRecord("添加游戏ID为" + signInfo.GameID+ "的签到信息");
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 增加一个签到表
        /// </summary>
        /// <param name="signList"></param>
        /// <returns>true增加成功，false已存在该签到表</returns>
        public bool AddSignList(SignList signList)
        {
            SignList signListResult = gameClubEntities.SignList.Where(s => s.SignID == signList.SignID).FirstOrDefault();
            if (signListResult == null)
            {
                gameClubEntities.SaveChanges();
                gameClubEntities.SignList.Add(signList);
                gameClubEntities.SaveChanges();
                
                foreach (var item in gameClubEntities.GameMember.Where(g => g.IsDel != true))
                {
                    SignInfo signInfo = new SignInfo
                    {
                        SignID = signList.SignID,
                        GameID=item.GameID,
                        SignCondition = "未签到",
                        VoiceCondition = "未语音",
                        IsLeave = false,
                    };
                    gameClubEntities.SignInfo.Add(signInfo);
                    
                }
                gameClubEntities.SaveChanges();
                EFUserRecord.AddUserOperateRecord("添加签到表ID为" + signList.SignID + "的签到表信息");
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool DelSignList(List<string> ListID)
        {
            bool f = true;
            foreach (var item in ListID)
            {
                SignList signList = gameClubEntities.SignList.Where(s => s.SignID == item).FirstOrDefault();
                if (signList != null)
                {
                    if (DelSignList(signList) == false)
                    {
                        f = false;
                    }
                }
                else
                {
                    f = false;
                }
            }
            return f;
        }

        /// <summary>
        /// 删除一条签到信息记录
        /// </summary>
        /// <param name="signInfo"></param>
        /// <returns>true删除记录成功，false删除记录失败，不存在该记录</returns>
        public bool DelSignInfo(SignInfo signInfo)
        {
            SignInfo signInfoResult = gameClubEntities.SignInfo.Where(s => (s.SignID == signInfo.SignID) && (s.GameID == signInfo.GameID)).FirstOrDefault();
            if (signInfoResult != null)
            {
                gameClubEntities.SignInfo.Remove(signInfoResult);
                gameClubEntities.SaveChanges();
                EFUserRecord.AddUserOperateRecord("删除了游戏ID为" + signInfo.GameID + "的签到信息");
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 删除签到表
        /// </summary>
        /// <param name="signList"></param>
        /// <returns>true表示删除成功，false表示删除失败，不存在该表</returns>
        public bool DelSignList(SignList signList)
        {
            SignList signListResult = gameClubEntities.SignList.Where(s => s.SignID == signList.SignID).FirstOrDefault();
            if (signListResult != null)
            {
                ContributionList contributionList= gameClubEntities.ContributionList.Where(c => c.ContributionID == signListResult.ContributionID).FirstOrDefault();
                MemberGroupList memberGroupList= gameClubEntities.MemberGroupList.Where(m => m.MemberGroupID == signListResult.MemberGroupID).FirstOrDefault();

                if (contributionList != null)
                {
                    contributionList.SignID = null;
                }
                if (memberGroupList != null)
                {
                    memberGroupList.SignID = null;
                }
                IEnumerable<SignInfo> signInfos = gameClubEntities.SignInfo.Where(s => s.SignID == signListResult.SignID);
                foreach (var item in signInfos)
                {
                    gameClubEntities.SignInfo.Remove(item);
                }
                gameClubEntities.SignList.Remove(signListResult);
                gameClubEntities.SaveChanges();
                EFUserRecord.AddUserOperateRecord("删除签到表ID为" + signList.SignID + "的签到表信息");
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 搜索签到信息
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public IEnumerable<SignInfo> searchSignInfos(string keyword)
        {
            if (keyword == null)
            {
                return signInfos;
            }
            else
            {
                return gameClubEntities.SignInfo.Where(s => (s.SignID.ToString().Contains(keyword)) || (s.GameID.ToString().Contains(keyword)));
            }
        }
        /// <summary>
        /// 搜索签到信息表
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public IEnumerable<SignList> searchSignLists(string keyword)
        {
            if (keyword == null)
            {
                return signLists;
            }
            else
            {
                return gameClubEntities.SignList.Where(s => (s.SignID.ToString().Contains(keyword))||(s.Type.Contains(keyword)));
            }
        }

        /// <summary>
        /// 获取某个某个成员某次签到的信息
        /// </summary>
        /// <param name="SignID"></param>
        /// <returns></returns>
        public SignInfo signInfo(string SignID,int GameID)
        {
            return gameClubEntities.SignInfo.Where(s => (s.SignID == SignID) && (s.GameID == GameID)).FirstOrDefault();
        }
        /// <summary>
        /// 查找某个签到表的信息
        /// </summary>
        /// <param name="SigbID"></param>
        /// <returns></returns>
        public SignList signList(string SignID)
        {
            return gameClubEntities.SignList.Where(s => s.SignID == SignID).FirstOrDefault();
        }
        /// <summary>
        /// 更新签到信息记录
        /// </summary>
        /// <param name="signInfo"></param>
        /// <returns>true更新成功，false更新失败</returns>
        public bool UpdateSignInfo(SignInfo signInfo)
        {
            SignInfo signInfoResult = gameClubEntities.SignInfo.Where(s => (s.SignID == signInfo.SignID) && (s.GameID == signInfo.GameID)).FirstOrDefault();
            if (signInfoResult != null)
            {
                signInfoResult.SignCondition = signInfo.SignCondition;
                signInfoResult.SignDatetime = signInfo.SignDatetime;
                signInfoResult.VoiceCondition = signInfo.VoiceCondition;
                signInfoResult.IsLeave = signInfo.IsLeave;
                signInfoResult.Deal = signInfo.Deal;
                gameClubEntities.SaveChanges();
                EFUserRecord.AddUserOperateRecord("更新游戏ID为" + signInfo.GameID + "的签到信息");
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 更新签到表信息
        /// </summary>
        /// <param name="signList"></param>
        /// <returns>true更新成功，false更新失败，不存在该签到表</returns>
        public bool UpdateSignList(SignList signList)
        {
            if (signList == null)
            {
                return false;
            }
            SignList signListResult = gameClubEntities.SignList.Where(s => s.SignID == signList.SignID).FirstOrDefault();
            if (signListResult != null)
            {
                if (signListResult.ContributionID != signList.ContributionID)
                {
                    if (signListResult.ContributionID != null)
                    {
                        ContributionList contributionList = gameClubEntities.ContributionList.Where(c => c.ContributionID == signListResult.ContributionID).FirstOrDefault();
                        contributionList.SignID = null;
                    }
                    if (!string.IsNullOrEmpty(signList.ContributionID))
                    {
                        ContributionList contributionList = gameClubEntities.ContributionList.Where(c => c.ContributionID == signList.ContributionID).FirstOrDefault();
                        contributionList.SignID = signList.SignID;
                    }
                    gameClubEntities.SaveChanges();
                }
                if (signListResult.MemberGroupID != signList.MemberGroupID)
                {
                    if (signListResult.MemberGroupID != null)
                    {
                        MemberGroupList memberGroupList = gameClubEntities.MemberGroupList.Where(m => m.MemberGroupID == signListResult.MemberGroupID).FirstOrDefault();
                        memberGroupList.SignID = null;
                    }
                    if (!string.IsNullOrEmpty(signList.MemberGroupID))
                    {
                        MemberGroupList memberGroupList = gameClubEntities.MemberGroupList.Where(m => m.MemberGroupID == signList.MemberGroupID).FirstOrDefault();
                        memberGroupList.SignID = signList.SignID;
                    }
                    gameClubEntities.SaveChanges();
                }
                signListResult.Active = signList.Active;
                signListResult.Type = signList.Type;
                signListResult.StartDateTime = signList.StartDateTime;
                signListResult.EndDateTime = signList.EndDateTime;
                signListResult.ContributionID = signList.ContributionID;
                signListResult.MemberGroupID = signList.MemberGroupID;

                gameClubEntities.SaveChanges();
                EFUserRecord.AddUserOperateRecord("更新签到表ID为" + signList.SignID + "的签到表信息");
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 批量删除签到表信息
        /// </summary>
        /// <param name="signInfos"></param>
        /// <returns>true删除成功，false存在不能未能删除成功的数据</returns>
        public bool DelSignInfo(List<string> ListID)
        {
            bool f = true;
            foreach (var item in ListID)
            {
                string[] temp = item.Split('.');
                string SignID = temp[0];
                int GameID = Convert.ToInt32(temp[1]);
                SignInfo signInfo= gameClubEntities.SignInfo.Where(s => (s.SignID == SignID)&&(s.GameID==GameID)).FirstOrDefault();
                if (signInfo != null)
                {
                    if (!DelSignInfo(signInfo))
                    {
                        f = false;
                    }
                }
                else
                {
                    f = false;
                }
            }
            return f;
        }
    }
}