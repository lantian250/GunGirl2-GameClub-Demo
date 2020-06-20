using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameClub.Infrastructure.Abstract
{
    public interface IAuthProvider
    {
        int Authenticate(string username, string password);
    }
}
