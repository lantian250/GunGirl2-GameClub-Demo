using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GameClub.Abstract;
using GameClub.Models;

namespace GameClub.Concrete
{
    public class EFArticle : IArticle
    {
        GameClubEntities gameClubEntities=new GameClubEntities();
        EFUserRecord EFUserRecord = new EFUserRecord();
        public IEnumerable<Article> Articles => gameClubEntities.Article;

        public bool AddArticle(Article article)
        {
            if (article == null)
            {
                return false;
            }
            else
            {
                article.CreateTime = DateTime.Now;
                gameClubEntities.Article.Add(article);
                gameClubEntities.SaveChanges();
                EFUserRecord.AddUserOperateRecord("添加了标题为" + article.Title+ "的文章");
                return true;
            }
            
        }

        public Article Article(int id)
        {
            return gameClubEntities.Article.Where(a => a.ArticleID == id).FirstOrDefault();
        }

        public bool DelArticle(int id)
        {
            if (id > 0)
            {
                Article article = gameClubEntities.Article.Where(a => a.ArticleID == id).FirstOrDefault();
                if (article != null)
                {
                    gameClubEntities.Article.Remove(article);
                    gameClubEntities.SaveChanges();
                    EFUserRecord.AddUserOperateRecord("删除了标题为" + article.Title + "的文章");
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool UpdateArticle(Article article)
        {
            Article articleResult = gameClubEntities.Article.Where(a => a.ArticleID == article.ArticleID).FirstOrDefault();
            if (articleResult != null)
            {
                articleResult.Context = article.Context;
                articleResult.Title = article.Title;
                gameClubEntities.SaveChanges();
                EFUserRecord.AddUserOperateRecord("更新了标题为" + article.Title + "的文章");
                return true;
            }
            return false;
        }
    }
}