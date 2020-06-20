using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GameClub.Models;
using GameClub.Abstract;

namespace GameClub.Concrete
{
    public class EFGameMember : IGameMember
    {
        GameClubEntities gameClubEntities = new GameClubEntities();
        EFUserRecord EFUserRecord = new EFUserRecord();
        public IEnumerable<GameMember> gameMembers => gameClubEntities.GameMember.OrderBy(g=>g.Authority).ThenBy(g=>g.GameID);//获取社团团员信息表
        /// <summary>
        /// 根据关键字查找团员信息
        /// </summary>
        /// <param name="keyword">关键字</param>
        /// <returns>返回查询结果集,如果传递进来是null，会返回完整信息数据</returns>
        public IEnumerable<GameMember> searchGameMembers(string keyword)
        {
            if (keyword == null)
            {
                return gameMembers;
            }
            else
            {
                var query = gameClubEntities.GameMember.AsQueryable();
                query = query.Where(g => g.GameID.ToString().Contains(keyword) || g.GameName.Contains(keyword)).Where(g => g.IsDel != true).OrderBy(g => g.Authority).ThenBy(g => g.GameID);
                return query;
            }
            
        }

        public List<GameAuthority> gameAuthorities => gameClubEntities.GameAuthority.ToList();//获取社团所有职位信息
        /// <summary>
        /// 添加职权信息
        /// </summary>
        /// <param name="gameAuthority"></param>
        /// <returns>返回0代表出现重复数据，返回1表示添加成功</returns>
        public int addGameAuthority(GameAuthority gameAuthority)
        {
            GameAuthority gameAuthorityResult = gameClubEntities.GameAuthority.Where(ga => ga.Number == gameAuthority.Number).FirstOrDefault();
            if (gameAuthorityResult == null)
            {
                gameClubEntities.GameAuthority.Add(gameAuthority);
                gameClubEntities.SaveChanges();
                EFUserRecord.AddUserOperateRecord("添加了" + gameAuthority.AuthorityString+ "的社团权限");
                return 1;
            }
            else
            {
                return 0;
            }
        }
        /// <summary>
        /// 添加社团团员信息
        /// </summary>
        /// <param name="gameMember"></param>
        /// <returns>“1”表示添加成功，“0”表示已存在相同GameID</returns>
        public int addGameMember(GameMember gameMember)
        {
            GameMember gameMemberResult = gameClubEntities.GameMember.Where(g => g.GameID == gameMember.GameID).FirstOrDefault();
            if (gameMemberResult == null)
            {
                gameMember.JoinDate = DateTime.Now;
                gameClubEntities.GameMember.Add(gameMember);
                gameClubEntities.SaveChanges();
                EFUserRecord.AddUserOperateRecord("添加了社团团员" +gameMember.GameID +" "+gameMember.GameName+ "的信息");
                return 1;
            }
            else
            {
                return 0;
            }
        }
        /// <summary>
        /// 删除职位信息
        /// </summary>
        /// <param name="number"></param>
        /// <returns>"0"表示未找到该职位信息，“1”表示删除成功</returns>
        public int deleteGameAuthority(int number)
        {
            GameAuthority gameAuthority = gameClubEntities.GameAuthority.Where(ga => ga.Number == number).FirstOrDefault();
            if (gameAuthority != null)
            {
                if (gameClubEntities.GameMember.Where(g => g.Authority == number) == null)
                {
                    gameClubEntities.GameAuthority.Remove(gameAuthority);
                    gameClubEntities.SaveChanges();
                    EFUserRecord.AddUserOperateRecord("删除了" + gameAuthority.AuthorityString + "的社团权限");
                    return 1;

                }
                else
                {
                    return 0;
                }
                
            }
            else
            {
                return 0;
            }
        }
        /// <summary>
        /// 删除社团团员
        /// </summary>
        /// <param name="gameID"></param>
        /// <returns>"0"不存在的社团团员，"1"删除成功</returns>
        public int deleteGameMember(int gameID)
        {
            GameMember gameMember = gameClubEntities.GameMember.Where(g => g.GameID == gameID).FirstOrDefault();
            if (gameMember != null)
            {
                gameMember.IsDel = true;
                gameMember.DelTime = DateTime.Now;
                gameClubEntities.SaveChanges();
                EFUserRecord.AddUserOperateRecord("删除了社团团员" + gameMember.GameID + " " + gameMember.GameName + "的信息");
                return 1;
            }
            else
            {
                return 0;
            }
        }
        /// <summary>
        /// 获取某个社团团员的信息
        /// </summary>
        /// <param name="gameID"></param>
        /// <returns>返回获取信息结果</returns>
        public GameMember gameMember(int gameID)
        {
            GameMember gameMember = gameClubEntities.GameMember.Where(g => g.GameID == gameID).FirstOrDefault();
            return gameMember;
        }
        /// <summary>
        /// 获取职位编号对应的信息
        /// </summary>
        /// <param name="number"></param>
        /// <returns>返回职位信息</returns>
        public string getAuthorityString(int number)
        {
            return gameClubEntities.GameAuthority.Where(ga => ga.Number == number).Select(ga => ga.AuthorityString).ToString();
        }
        /// <summary>
        /// 更新职位信息
        /// </summary>
        /// <param name="gameAuthority"></param>
        /// <returns>"0"更新失败,"1"更新成功</returns>
        public int updateGameAuthority(GameAuthority gameAuthority)
        {
            if (gameAuthority == null) return 0;
            GameAuthority gameAuthorityResult = gameClubEntities.GameAuthority.Where(ga => ga.Number == gameAuthority.Number).FirstOrDefault();
            if (gameAuthorityResult != null)
            {
                gameAuthorityResult.AuthorityString = gameAuthority.AuthorityString;
                gameClubEntities.SaveChanges();
                EFUserRecord.AddUserOperateRecord("更新了" + gameAuthority.AuthorityString + "的社团权限");
                return 1;
            }
            else
            {
                return 0;
            }
        }
        /// <summary>
        /// 更新社团团员信息
        /// </summary>
        /// <param name="gameMember"></param>
        /// <returns>"0"更新失败，不存在该团员,"1"更新成功</returns>
        public int updateGameMember(GameMember gameMember,int oldGameID)
        {
            if (gameMember == null) return 0;
            GameMember gameMemberResult = gameClubEntities.GameMember.Where(g => g.GameID == oldGameID).FirstOrDefault();
            if (gameMemberResult != null)
            {
                if (gameMember.GameID == oldGameID)
                {
                    gameMemberResult.GameName = gameMember.GameName;
                    gameMemberResult.Authority = gameMember.Authority;
                    gameMemberResult.JoinDate = gameMember.JoinDate;
                    gameClubEntities.SaveChanges();
                }
                else
                {
                    gameClubEntities.GameMember.Remove(gameMemberResult);
                    gameClubEntities.GameMember.Add(gameMember);
                    gameClubEntities.SaveChanges();

                }
                EFUserRecord.AddUserOperateRecord("更新了社团团员" + gameMember.GameID + " " + gameMember.GameName + "的信息");
                
                return 1;
            }
            else
            {
                return 0;
            }
            
        }
    }
}