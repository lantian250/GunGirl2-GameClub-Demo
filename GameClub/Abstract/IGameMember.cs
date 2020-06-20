using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameClub.Models;

namespace GameClub.Abstract
{
    public interface IGameMember
    {
        IEnumerable<GameMember> gameMembers { get; }
        GameMember gameMember(int gameID);
        IEnumerable<GameMember> searchGameMembers(string keyword);
        int updateGameMember(GameMember gameMember, int oldGameID);
        int deleteGameMember(int gameID);
        int addGameMember(GameMember gameMember);
        List<GameAuthority> gameAuthorities { get; }
        string getAuthorityString(int number);
        int addGameAuthority(GameAuthority gameAuthority);
        int deleteGameAuthority(int number);
        int updateGameAuthority(GameAuthority gameAuthority);
    }
}
