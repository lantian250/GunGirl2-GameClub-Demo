using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameClub.Models;

namespace GameClub.Abstract
{
    public interface IContribution
    {
        IEnumerable<Contribution> Contributions { get;}//获取贡献表列表
        IEnumerable<ContributionList> ContributionLists { get;}//获取贡献表信息列表
        Contribution Contribution(string ContributionID, int GameID);//获取某个贡献表信息
        ContributionList ContributionList(string ContributionID);//获取某个贡献表
        IEnumerable<Contribution> GetContributions(string keyword);//按指定规则获取贡献表列表
        IEnumerable<ContributionList> GetContributionLists(string keyword);//按指定规则获取贡献表信息列表
        bool AddContribution(Contribution contribution);//添加贡献表
        bool DelContribution(Contribution contribution);//删除贡献表
        bool UpdateContribution(Contribution contribution);//更新贡献表
        bool AddContributionList(ContributionList contributionList);//添加贡献表信息
        bool DelContributionList(ContributionList contributionList);//删除贡献表
        bool UpdateContributionList(ContributionList contributionList);//更新贡献表
        bool DealListContribution(List<string> ListID,string DealAction);//批处理贡献表列表
        bool DealListContributionList(List<string> ListID, string DealAction);//批处理贡献表信息列表
}
}
