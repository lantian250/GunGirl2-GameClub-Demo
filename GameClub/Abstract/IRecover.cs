using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameClub.Models;

namespace GameClub.Abstract
{
    public interface IRecover
    {
        IEnumerable<UserInfo> UserInfos { get; }
        IEnumerable<GameMember> GameMembers{ get; }
        bool DelUserInfo(int UserID);
        bool DelGameMember(int GameID);
        bool ResetUserInfo(int UserID);
        bool ResetGameMember(int GameID);
    }
}
