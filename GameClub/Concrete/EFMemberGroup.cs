using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GameClub.Models;
using GameClub.Abstract;

namespace GameClub.Concrete
{
    public class EFMemberGroup : IMemberGroup
    {
        GameClubEntities gameClubEntities = new GameClubEntities();
        EFUserRecord EFUserRecord = new EFUserRecord();
        public IEnumerable<MemberGroup> MemberGroups => gameClubEntities.MemberGroup;//获取分组表信息

        public IEnumerable<MemberGroupList> MemberGroupLists => gameClubEntities.MemberGroupList;//获取分组表
        /// <summary>
        /// 添加分组表信息
        /// </summary>
        /// <param name="memberGroup"></param>
        /// <returns></returns>
        public bool AddMemberGroup(MemberGroup memberGroup)
        {
            MemberGroup memberGroupResult = gameClubEntities.MemberGroup.Where(m => (m.MemberGroupID == memberGroup.MemberGroupID) && (m.GameID == memberGroup.GameID)).FirstOrDefault();
            if (memberGroupResult == null)
            {
                gameClubEntities.MemberGroup.Add(memberGroup);
                gameClubEntities.SaveChanges();
                EFUserRecord.AddUserOperateRecord("为ID为"+memberGroup.GroupName +"添加了分组信息");
                return true;
            }
            return false;
        }
        /// <summary>
        /// 添加分组表
        /// </summary>
        /// <param name="memberGroupList"></param>
        /// <returns></returns>
        public bool AddMemberGroupList(MemberGroupList memberGroupList)
        {
            MemberGroupList memberGroupListResult = gameClubEntities.MemberGroupList.Where(m => m.MemberGroupID == memberGroupList.MemberGroupID).FirstOrDefault();
            if (memberGroupListResult == null)
            {
                memberGroupListResult = memberGroupList;
                memberGroupListResult.CreateDateTime = DateTime.Now;
                gameClubEntities.MemberGroupList.Add(memberGroupListResult);
                gameClubEntities.SaveChanges();
                foreach (var item in gameClubEntities.GameMember.Where(g => g.IsDel != true))
                {
                    MemberGroup memberGroup = new MemberGroup
                    {
                        MemberGroupID = memberGroupList.MemberGroupID,
                        GameID = item.GameID,
                    };
                    gameClubEntities.MemberGroup.Add(memberGroup);
                }
                gameClubEntities.SaveChanges();
                EFUserRecord.AddUserOperateRecord("添加了ID为" + memberGroupList.MemberGroupID + "的分组表信息");
                return true;
            }
            return false;
        }
        /// <summary>
        /// 批量处理分组表信息
        /// </summary>
        /// <param name="ListID"></param>
        /// <param name="DealAction"></param>
        /// <returns></returns>
        public bool DealListMemberGroup(List<string> ListID, string DealAction)
        {
            bool f = true;
            if ("删除".Equals(DealAction))
            {
                if (ListID != null)
                {
                    f = true;
                    foreach (var item in ListID)
                    {
                        string[] temp = item.Split('.');
                        string MemberGroupID = temp[0];
                        int GameID = Convert.ToInt32(temp[1]);
                        MemberGroup memberGroupResult = gameClubEntities.MemberGroup.Where(m => (m.MemberGroupID == MemberGroupID) && (m.GameID == GameID)).FirstOrDefault();
                        if (memberGroupResult != null)
                        {
                            if (DelMemberGroup(memberGroupResult) == false)
                            {
                                f = false;
                            }
                        }
                        else
                        {
                            f = false;
                        }
                    }
                }
            }
            return f;
        }

        /// <summary>
        /// 批量处理分组表
        /// </summary>
        /// <param name="ListID"></param>
        /// <param name="DealAction"></param>
        /// <returns></returns>
        public bool DealListMemberGroupList(List<string> ListID, string DealAction)
        {
            bool f= true;
            if ("删除".Equals(DealAction))
            {
                if (ListID != null)
                {
                    f = true;
                    foreach (var item in ListID)
                    {
                        MemberGroupList memberGroupListResult = gameClubEntities.MemberGroupList.Where(m => m.MemberGroupID == item).FirstOrDefault();
                        if (memberGroupListResult != null)
                        {
                            if (DelMemberGroupList(memberGroupListResult) == false)
                            {
                                f = false;
                            }
                        }
                        else
                        {
                            f = false;
                        }
                    }
                }
            }
            return f;
        }
        /// <summary>
        /// 删除分组表信息
        /// </summary>
        /// <param name="memberGroup"></param>
        /// <returns></returns>
        public bool DelMemberGroup(MemberGroup memberGroup)
        {
            MemberGroup memberGroupResult = gameClubEntities.MemberGroup.Where(m => (m.MemberGroupID == memberGroup.MemberGroupID) && (m.GameID == memberGroup.GameID)).FirstOrDefault();
            if (memberGroupResult != null)
            {
                gameClubEntities.MemberGroup.Remove(memberGroupResult);
                gameClubEntities.SaveChanges();
                EFUserRecord.AddUserOperateRecord("删除了ID为" + memberGroup.GroupName + "的分组信息");
                return true;
            }
            return false;
        }
        /// <summary>
        /// 删除分组表
        /// </summary>
        /// <param name="memberGroupList"></param>
        /// <returns></returns>
        public bool DelMemberGroupList(MemberGroupList memberGroupList)
        {
            MemberGroupList memberGroupListResult = gameClubEntities.MemberGroupList.Where(m => m.MemberGroupID == memberGroupList.MemberGroupID).FirstOrDefault();
            if (memberGroupListResult != null)
            {
                SignList signList = gameClubEntities.SignList.Where(s => s.SignID == memberGroupList.SignID).FirstOrDefault();
                ContributionList contributionList = gameClubEntities.ContributionList.Where(c => c.ContributionID == memberGroupList.ContributionID).FirstOrDefault();
                IEnumerable<MemberGroup> memberGroups = gameClubEntities.MemberGroup.Where(m => m.MemberGroupID == memberGroupListResult.MemberGroupID);
                if (signList != null)
                {
                    signList.MemberGroupID = null;
                }
                if (contributionList != null)
                {
                    contributionList.MemberGroupID = null;
                }
                foreach (var item in memberGroups)
                {
                    gameClubEntities.MemberGroup.Remove(item);
                }
                gameClubEntities.MemberGroupList.Remove(memberGroupListResult);
                gameClubEntities.SaveChanges();
                EFUserRecord.AddUserOperateRecord("删除了ID为" + memberGroupList.MemberGroupID + "的分组表信息");
                return true;
            }
            return false;
        }
        /// <summary>
        /// 按指定规则获取分组表
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public IEnumerable<MemberGroupList> GetMemberGroupLists(string keyword)
        {
            if (keyword != null)
            {
                return gameClubEntities.MemberGroupList.Where(m => m.MemberGroupID.ToString().Contains(keyword));
            }
            return MemberGroupLists;
        }
        /// <summary>
        /// 按指定规则获取分组表信息
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public IEnumerable<MemberGroup> GetMemberGroups(string keyword)
        {
            if (keyword != null)
            {
                return gameClubEntities.MemberGroup.Where(m => m.MemberGroupID.ToString().Contains(keyword) || m.GameID.ToString().Contains(keyword));
            }
            return MemberGroups;
        }
        /// <summary>
        /// 获取某个分组表信息
        /// </summary>
        /// <param name="MemberGroupID"></param>
        /// <returns></returns>
        public MemberGroup MemberGroup(string MemberGroupID, int GameID)
        {
            return gameClubEntities.MemberGroup.Where(m => (m.MemberGroupID == MemberGroupID) && (m.GameID == GameID)).FirstOrDefault();
        }
        /// <summary>
        /// 获取某个分组表
        /// </summary>
        /// <param name="MemberGroupID"></param>
        /// <param name="GameID"></param>
        /// <returns></returns>
        public MemberGroupList MemberGroupList(string MemberGroupID)
        {
            return gameClubEntities.MemberGroupList.Where(m => m.MemberGroupID == MemberGroupID).FirstOrDefault();
        }
        /// <summary>
        /// 更新分组表信息
        /// </summary>
        /// <param name="memberGroup"></param>
        /// <returns></returns>
        public bool UpdateMemberGroup(MemberGroup memberGroup)
        {
            if (memberGroup == null) return false;
            MemberGroup memberGroupResult = gameClubEntities.MemberGroup.Where(m => (m.MemberGroupID == memberGroup.MemberGroupID) && (m.GameID == memberGroup.GameID)).FirstOrDefault();
            if (memberGroupResult != null)
            {
                memberGroupResult.GroupName = memberGroup.GroupName;
                gameClubEntities.SaveChanges();
                EFUserRecord.AddUserOperateRecord("更新了ID为" + memberGroup.GroupName + "的分组信息");
                return true;
            }
            return false;
        }
        /// <summary>
        /// 更新分组表
        /// </summary>
        /// <param name="memberGroupList"></param>
        /// <returns></returns>
        public bool UpdateMemberGroupList(MemberGroupList memberGroupList)
        {
            if (memberGroupList == null) return false;
            MemberGroupList memberGroupListResult = gameClubEntities.MemberGroupList.Where(m => m.MemberGroupID == memberGroupList.MemberGroupID).FirstOrDefault();
            SignList signList;
            ContributionList contributionList;
            if (memberGroupListResult != null)
            {
                if (!string.IsNullOrEmpty(memberGroupListResult.SignID))
                {
                    signList = gameClubEntities.SignList.Where(s => s.SignID == memberGroupListResult.SignID).FirstOrDefault();
                    signList.MemberGroupID = null;
                    memberGroupListResult.SignID = null;
                }
                if (!string.IsNullOrEmpty(memberGroupListResult.ContributionID))
                {
                    contributionList = gameClubEntities.ContributionList.Where(c => c.ContributionID == memberGroupListResult.ContributionID).FirstOrDefault();
                    contributionList.MemberGroupID = null;
                    memberGroupListResult.ContributionID = null;
                }
                if (!string.IsNullOrEmpty(memberGroupList.SignID))
                {
                    signList = gameClubEntities.SignList.Where(s => s.SignID == memberGroupList.SignID).FirstOrDefault();
                    signList.MemberGroupID = memberGroupList.MemberGroupID;
                }
                if (!string.IsNullOrEmpty(memberGroupList.ContributionID))
                {
                    contributionList = gameClubEntities.ContributionList.Where(c => c.ContributionID == memberGroupList.ContributionID).FirstOrDefault();
                    contributionList.MemberGroupID = memberGroupList.MemberGroupID;
                }
                memberGroupListResult.Type = memberGroupList.Type;
                memberGroupListResult.ContributionID = memberGroupList.ContributionID;
                memberGroupListResult.SignID = memberGroupList.SignID;
                EFUserRecord.AddUserOperateRecord("更新了ID为" + memberGroupList.MemberGroupID + "的分组表信息");
                gameClubEntities.SaveChanges();
                return true;
            }
            return false;
        }
    }
}