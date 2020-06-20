using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameClub.Models;

namespace GameClub.Abstract
{
    public interface ISignInfo
    {
        IEnumerable<SignInfo> signInfos { get; }
        IEnumerable<SignList> signLists { get; }
        SignInfo signInfo(string SignID,int GameID);
        SignList signList(string SignID);
        bool AddSignInfo(SignInfo signInfo);
        bool DelSignInfo(SignInfo signInfo);
        bool UpdateSignInfo(SignInfo signInfo);
        bool AddSignList(SignList signList);
        bool DelSignList(SignList signList);
        bool UpdateSignList(SignList signList);
        IEnumerable<SignList> searchSignLists(string keyword);
        IEnumerable<SignInfo> searchSignInfos(string keyword);
        bool DelSignList(List<string> ListID);
        bool DelSignInfo(List<string> ListID);
        bool ActiveAbleList(List<string> ListID);
        bool ActiveDisableList(List<string> ListID);

    }
}
