using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GameClub.Abstract;
using GameClub.Models;

namespace GameClub.Concrete
{
    public class EFInformMessage:IInformMessage
    {
        GameClubEntities gameClubEntities = new GameClubEntities();
        EFUserRecord EFUserRecord = new EFUserRecord();

        public IEnumerable<InformMessage> InformMessages => gameClubEntities.InformMessage;

        public bool AddInformMessage(InformMessage informMessage)
        {
            if (informMessage == null)
            {
                return false;
            }
            else
            {
                informMessage.CreateTime = DateTime.Now;
                gameClubEntities.InformMessage.Add(informMessage);
                EFUserRecord.AddUserOperateRecord("添加了标题为" +informMessage.Title + "的通知消息");
                gameClubEntities.SaveChanges();
                return true;
            }
        }

        public bool DelInformMessage(InformMessage informMessage)
        {
            if (informMessage == null)
            {
                return false;
            }
            InformMessage informMessageResult = gameClubEntities.InformMessage.Where(i => i.ID == informMessage.ID).FirstOrDefault();
            if (informMessageResult != null)
            {
                gameClubEntities.InformMessage.Remove(informMessageResult);
                gameClubEntities.SaveChanges();
                EFUserRecord.AddUserOperateRecord("删除了标题为" + informMessage.Title + "的通知消息");
                return true;
            }
            return false;
        }

        public IEnumerable<InformMessage> SearchInformMessages(string Keyword)
        {
            if (Keyword != null)
            {
                return gameClubEntities.InformMessage.Where(i => (i.Title.ToString().Contains(Keyword)) || (i.Context.ToString().Contains(Keyword)));
            }
            return InformMessages;
        }

        public bool UpdateInformMessage(InformMessage informMessage)
        {
            if (informMessage == null)
            {
                return false;
            }
            InformMessage informMessageResult= gameClubEntities.InformMessage.Where(i => i.ID == informMessage.ID).FirstOrDefault();
            if (informMessageResult != null)
            {
                informMessageResult.Title = informMessage.Title;
                informMessageResult.Context = informMessage.Context;
                gameClubEntities.SaveChanges();
                EFUserRecord.AddUserOperateRecord("更新了标题为" + informMessage.Title + "的通知消息");
                return false;
            }
            return false;
        }
    }
}