using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameClub.Models;

namespace GameClub.Abstract
{
    public interface IInformMessage
    {
        IEnumerable<InformMessage> InformMessages { get; }
        bool DelInformMessage(InformMessage informMessage);
        bool AddInformMessage(InformMessage informMessage);
        bool UpdateInformMessage(InformMessage informMessage);
        IEnumerable<InformMessage> SearchInformMessages(string keyword);
    }
}
