using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameClub.Models;
namespace GameClub.Abstract
{
    public interface IArticle
    {
        IEnumerable<Article> Articles { get; }
        Article Article(int id);
        bool AddArticle(Article article);
        bool UpdateArticle(Article article);
        bool DelArticle(int id);
    }
}
