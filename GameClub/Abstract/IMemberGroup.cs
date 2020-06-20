using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameClub.Models;

namespace GameClub.Abstract
{
    public interface IMemberGroup
    {
        IEnumerable<MemberGroup>  MemberGroups{ get;  }//获取所有分组表
        IEnumerable<MemberGroupList> MemberGroupLists { get; }//获取所有分组表信息
        MemberGroup MemberGroup(string MemberGroupID, int GameID);//获取某个分组表
        MemberGroupList MemberGroupList(string MemberGroupID);//获取某个分组表信息
        IEnumerable<MemberGroup> GetMemberGroups(string keyword);//按指定规则筛选排序获取分组表
        IEnumerable<MemberGroupList> GetMemberGroupLists(string keyword);//按指定指定规则筛选排序获取分组表信息
        bool AddMemberGroup(MemberGroup memberGroup);//添加分组表
        bool DelMemberGroup(MemberGroup memberGroup);//删除分组表
        bool UpdateMemberGroup(MemberGroup memberGroup);//更新分组表
        bool AddMemberGroupList(MemberGroupList memberGroupList);//添加分组表信息
        bool DelMemberGroupList(MemberGroupList memberGroupList);//删除分组表信息
        bool UpdateMemberGroupList(MemberGroupList memberGroupList);//更新分组表信息
        bool DealListMemberGroup(List<string> ListID, string DealAction);//批量处理签到表信息
        bool DealListMemberGroupList(List<string> ListID, string DealAction);//批量处理签到表
    }
}
