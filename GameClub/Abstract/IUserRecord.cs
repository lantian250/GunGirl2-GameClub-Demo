using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameClub.Models;

namespace GameClub.Abstract
{
    public interface IUserRecord
    {
        IEnumerable<UserLoginRecord> UserLoginRecords { get; }
        IEnumerable<UserOperateRecord> UserOperateRecords { get; }

        bool AddUserLoginRecord(UserLoginRecord userLoginRecord);
        bool AddUserOperateRecord(string OperateContext);
    }
}
