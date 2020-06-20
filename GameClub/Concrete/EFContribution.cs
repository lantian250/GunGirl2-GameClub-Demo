using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GameClub.Abstract;
using GameClub.Models;

namespace GameClub.Concrete
{
    public class EFContribution : IContribution
    {
        GameClubEntities gameClubEntities = new GameClubEntities();
        EFUserRecord EFUserRecord = new EFUserRecord();
        public IEnumerable<Contribution> Contributions { get => gameClubEntities.Contribution; }
        public IEnumerable<ContributionList> ContributionLists { get => gameClubEntities.ContributionList; }
        /// <summary>
        /// 添加贡献表
        /// </summary>
        /// <param name="contribution"></param>
        /// <returns>true添加成功，false添加失败</returns>
        public bool AddContribution(Contribution contribution)
        {
            Contribution contributionResult = gameClubEntities.Contribution.Where(c => (c.ContributionID == contribution.ContributionID) && (c.GameID == contribution.GameID)).FirstOrDefault();
            if (contributionResult == null)
            {
                ContributionList contributionList = gameClubEntities.ContributionList.Where(c => c.ContributionID == contribution.ContributionID).FirstOrDefault();
                if (contributionList.Time != null && contributionList.Time > 0)
                {
                    contribution.MinSpeed = (contribution.AllContribution) / contributionList.Time * decimal.Parse("60");
                }
                gameClubEntities.Contribution.Add(contribution);
                gameClubEntities.SaveChanges();
                EFUserRecord.AddUserOperateRecord("为ID为" + contribution.GameID + "团员添加了贡献信息");
                return true;
            }
            return false;
        }
        /// <summary>
        /// 添加贡献表信息
        /// </summary>
        /// <param name="contributionList"></param>
        /// <returns></returns>
        public bool AddContributionList(ContributionList contributionList)
        {
            ContributionList contributionListResult = gameClubEntities.ContributionList.Where(c => c.ContributionID == contributionList.ContributionID).FirstOrDefault();
            if (contributionListResult == null)
            {
                contributionListResult = contributionList;
                contributionListResult.CreateDateTime = DateTime.Now;
                gameClubEntities.ContributionList.Add(contributionListResult);
                gameClubEntities.SaveChanges();
                foreach (var item in gameClubEntities.GameMember.Where(g => g.IsDel != true))
                {
                    Contribution contribution = new Contribution
                    {
                        ContributionID = contributionList.ContributionID,
                        GameID = item.GameID,
                    };
                    gameClubEntities.Contribution.Add(contribution);
                }
                EFUserRecord.AddUserOperateRecord("添加了ID为" + contributionList.ContributionID + "的贡献表");
                gameClubEntities.SaveChanges();
                return true;
            }
            return false;
        }
        /// <summary>
        /// 获取某个贡献表
        /// </summary>
        /// <param name="ContributionID"></param>
        /// <param name="GameID"></param>
        /// <returns>没有找到返回null</returns>
        public Contribution Contribution(string ContributionID, int GameID)
        {
            return gameClubEntities.Contribution.Where(c => (c.ContributionID == ContributionID) && (c.GameID == GameID)).FirstOrDefault();
        }
        /// <summary>
        /// 获取某个贡献表信息
        /// </summary>
        /// <param name="ContributionID"></param>
        /// <returns>没有找到返回null</returns>
        public ContributionList ContributionList(string ContributionID)
        {
            return gameClubEntities.ContributionList.Where(c => c.ContributionID == ContributionID).FirstOrDefault();
        }
        /// <summary>
        /// 批量处理贡献表信息
        /// </summary>
        /// <param name="ListID"></param>
        /// <param name="DealAction"></param>
        /// <returns></returns>
        public bool DealListContribution(List<string> ListID, string DealAction)
        {
            bool f = false;
            if ("删除".Equals(DealAction))
            {
                if (ListID != null)
                {
                    f = true;
                    foreach (var item in ListID)
                    {
                        string[] temp = item.Split('.');
                        string ContributionID = temp[0];
                        int GameID = Convert.ToInt32(temp[1]);
                        Contribution contributionResult = gameClubEntities.Contribution.Where(c => (c.ContributionID == ContributionID) & (c.GameID == GameID)).FirstOrDefault();
                        if (contributionResult != null)
                        {
                            if (DelContribution(contributionResult) == false)
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
        /// 批量处理贡献表
        /// </summary>
        /// <param name="ListID"></param>
        /// <param name="DealAction"></param>
        /// <returns></returns>
        public bool DealListContributionList(List<string> ListID, string DealAction)
        {
            bool f = true;
            if ("删除".Equals(DealAction))
            {
                if (ListID != null)
                {
                    f = true;
                    foreach (var item in ListID)
                    {
                        ContributionList contributionListResult = gameClubEntities.ContributionList.Where(c => c.ContributionID == item).FirstOrDefault();
                        if (contributionListResult != null)
                        {
                            if (DelContributionList(contributionListResult) == false)
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
        /// 删除贡献表信息
        /// </summary>
        /// <param name="contribution"></param>
        /// <returns></returns>
        public bool DelContribution(Contribution contribution)
        {
            Contribution contributionResult = gameClubEntities.Contribution.Where(c => (c.ContributionID == contribution.ContributionID) && (c.GameID == contribution.GameID)).FirstOrDefault();
            if (contributionResult != null)
            {
                gameClubEntities.Contribution.Remove(contributionResult);
                gameClubEntities.SaveChanges();
                EFUserRecord.AddUserOperateRecord("删除了ID为" + contribution.GameID + "团员的贡献信息");
                return true;
            }
            return false;

        }
        /// <summary>
        /// 删除贡献表
        /// </summary>
        /// <param name="contributionList"></param>
        /// <returns></returns>
        public bool DelContributionList(ContributionList contributionList)
        {
            ContributionList contributionListResult = gameClubEntities.ContributionList.Where(c => c.ContributionID == contributionList.ContributionID).FirstOrDefault();
            if (contributionListResult != null)
            {
                SignList signList = gameClubEntities.SignList.Where(s => s.SignID == contributionListResult.SignID).FirstOrDefault();
                MemberGroupList memberGroupList = gameClubEntities.MemberGroupList.Where(m => m.MemberGroupID == contributionListResult.MemberGroupID).FirstOrDefault();
                if (signList != null)
                {
                    signList.ContributionID = null;
                }
                if (memberGroupList != null)
                {
                    memberGroupList.ContributionID = null;
                }
                gameClubEntities.SaveChanges();
                IEnumerable<Contribution> contributions = gameClubEntities.Contribution;
                foreach (var item in contributions)
                {
                    gameClubEntities.Contribution.Remove(item);
                }
                gameClubEntities.ContributionList.Remove(contributionListResult);
                EFUserRecord.AddUserOperateRecord("删除了ID为" + contributionList.ContributionID + "的贡献表");
                gameClubEntities.SaveChanges();
                return true;
            }
            return false;
        }
        /// <summary>
        /// 按指定规则获取贡献表
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public IEnumerable<ContributionList> GetContributionLists(string keyword)
        {
            if (keyword != null)
            {
                return gameClubEntities.ContributionList.Where(c => c.ContributionID.ToString().Contains(keyword));
            }
            return ContributionLists;
        }
        /// <summary>
        /// 按指定规则获取贡献表信息
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public IEnumerable<Contribution> GetContributions(string keyword)
        {
            if (keyword != null)
            {
                return gameClubEntities.Contribution.Where(c => (c.ContributionID.ToString().Contains(keyword)) || (c.GameID.ToString().Contains(keyword)));
            }
            return Contributions;
        }
        /// <summary>
        /// 更新贡献表信息
        /// </summary>
        /// <param name="contribution"></param>
        /// <returns></returns>
        public bool UpdateContribution(Contribution contribution)
        {
            if (contribution == null) return false;
            Contribution contributionResult = gameClubEntities.Contribution.Where(c => (c.ContributionID == contribution.ContributionID) && (c.GameID == contribution.GameID)).FirstOrDefault();
            if (contributionResult != null)
            {
                ContributionList contributionList = gameClubEntities.ContributionList.Where(c => c.ContributionID == contributionResult.ContributionID).FirstOrDefault();
                contributionResult.AllContribution = contribution.AllContribution;
                if (contributionList.Time != null && contributionList.Time > 0)
                {
                    contributionResult.MinSpeed = (contribution.AllContribution) / contributionList.Time * decimal.Parse("60");
                }
                gameClubEntities.SaveChanges();
                EFUserRecord.AddUserOperateRecord("为ID为" + contribution.GameID + "团员更新了贡献信息");
                return true;
            }
            return false;
        }
        /// <summary>
        /// 更新贡献表
        /// </summary>
        /// <param name="contributionList"></param>
        /// <returns></returns>
        public bool UpdateContributionList(ContributionList contributionList)
        {
            if (contributionList == null) return false;
            ContributionList contributionListResult = gameClubEntities.ContributionList.Where(c => c.ContributionID == contributionList.ContributionID).FirstOrDefault();
            SignList signList;
            MemberGroupList memberGroupList;
            if (contributionListResult != null)
            {
                if (!string.IsNullOrEmpty(contributionListResult.SignID))
                {
                    signList = gameClubEntities.SignList.Where(s => s.SignID == contributionListResult.SignID).FirstOrDefault();
                    signList.ContributionID = null;
                    contributionListResult.SignID = null;
                }
                if (!string.IsNullOrEmpty(contributionListResult.MemberGroupID))
                {
                    memberGroupList = gameClubEntities.MemberGroupList.Where(m => m.MemberGroupID == contributionListResult.MemberGroupID).FirstOrDefault();
                    memberGroupList.ContributionID = null;
                    contributionListResult.MemberGroupID = null;
                }
                if (!string.IsNullOrEmpty(contributionList.SignID))
                {
                    signList = gameClubEntities.SignList.Where(s => s.SignID == contributionList.SignID).FirstOrDefault();
                    signList.ContributionID = contributionList.ContributionID;
                }
                if (!string.IsNullOrEmpty(contributionList.MemberGroupID))
                {
                    memberGroupList = gameClubEntities.MemberGroupList.Where(m => m.MemberGroupID == contributionList.MemberGroupID).FirstOrDefault();
                    memberGroupList.ContributionID = contributionList.ContributionID;
                }
                if (contributionListResult.Time != contributionList.Time && contributionList.Time > 0)
                {
                    contributionListResult.Time = contributionList.Time;
                    foreach (var item in gameClubEntities.Contribution.Where(c => c.ContributionID == contributionListResult.ContributionID))
                    {
                        UpdateContributionMinSpeed(item, contributionListResult.Time);
                    }
                }
                contributionListResult.Type = contributionList.Type;
                contributionListResult.MemberGroupID = contributionList.MemberGroupID;
                contributionListResult.SignID = contributionList.SignID;
                EFUserRecord.AddUserOperateRecord("更新了ID为" + contributionList.ContributionID + "的贡献表");
                gameClubEntities.SaveChanges();
                return true;
            }
            return false;
        }
        public bool UpdateContributionMinSpeed(Contribution contribution, int? time)
        {
            if (contribution == null) return false;
            Contribution contributionResult = gameClubEntities.Contribution.Where(c => (c.ContributionID == contribution.ContributionID) && (c.GameID == contribution.GameID)).FirstOrDefault();
            if (contributionResult != null)
            {
                contributionResult.MinSpeed = (contributionResult.AllContribution) / time * decimal.Parse("60");
                return true;
            }
            return false;
        }
    }
}