using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameClub.Models;

namespace GameClub.Abstract
{
    public interface IFeedBack
    {
        IEnumerable<Feedback> Feedbacks { get;}
        Feedback Feedback(int feedbackID);
        bool AddFeedback(Feedback feedback);
        bool DelFeedback(int feedbackID);
        bool UpdateFeedback(Feedback feedback);

    }
}
