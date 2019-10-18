using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace DAL
{
    public class WorkInfo
    {
        private WorkInfo() { }
        private static WorkInfo _instance = new WorkInfo();
        public static WorkInfo Instance
        {
            get
            {
                return _instance;
            }
        }
        string cns = AppConfigurtaionServices.Configuration.GetConnectionString("cns");

        public Model.WorkInfoNo GetModel(int id)
        {
            using (IDbConnection cn = new MySqlConnection(cns))
            {
                string sql = "select workinfo.*,activity.activityName from workinfo join activity on workinfo.activityId=activity.activityId where workId=@id";
                return cn.QueryFirstOrDefault<Model.WorkInfoNo>(sql,new {id=id});
            }
        }
        public IEnumerable<Model.WorkInfo> GetNew()
        {
            using (IDbConnection cn = new MySqlConnection(cns))
            {
                string sql = "select * from workinfo where workVerify='审核通过' order by uploadTime desc limit 8";
                return cn.Query<Model.WorkInfo>(sql);
            }
        }

        public IEnumerable<Model.WorkInfo> GetRecommend()
        {
            using (IDbConnection cn = new MySqlConnection(cns))
            {
                string sql = "select * from userinfo where workVerify='审核通过' and recommend='是' order by recommendTime desc limit 6";
                return cn.Query<Model.WorkInfo>(sql);
            }
        }

        public int GetCount()
        {
            using (IDbConnection cn = new MySqlConnection(cns))
            {
                string sql = "select count(1) from Workinfo";
                return cn.ExecuteScalar<int>(sql);
            }
        }

        public int GetCount(int[]activityIds)
        {
            using (IDbConnection cn = new MySqlConnection(cns))
            {
                string sql = "select count(1) from workinfo where activityId in @activityIds";
                return cn.ExecuteScalar<int>(sql, new { activityIds=activityIds});
            }
        }

        public IEnumerable<Model.WorkInfoNo> GetPage(Model.WorkPage page)
        {
            using (IDbConnection cn = new MySqlConnection(cns))
            {
                string sql = "with a as(select row_number() over(order by uploadTime desc) as num, workinfo.* from workinfo where activityId in @activityId)";
                sql += "select* from a where num between (@pageIndex-1)*@pageSize+1 and @pageIndex*@pageSize;";
                return cn.Query<Model.WorkInfoNo>(sql,new { pageIndex=page.pageIndex,pageSize=page.pageSize,activityIds=page.activityIds});
            }
        }

        public int GetFindCount(string workName)
        {
            using (IDbConnection cn = new MySqlConnection(cns))
            {
                string sql = "select count(1) from workinfo where workverify='审核通过' and workName like concat('%',@workName,'%')";
                return cn.ExecuteScalar<int>(sql,new { workName = workName });
            }
        }

        public IEnumerable<Model.WorkInfoNo> GetFindPage(Model.WorkFindPage page)
        {
            using (IDbConnection cn = new MySqlConnection(cns))
            {
                string sql = "with a as(select row_number() over(order by uploadTime desc) as num, workinfo.*,activityName from workinfo join activity on  workinfo.activityId=activity.activityId where workverify='审核通过' and workName like concat('%',@workName,'%'))";
                sql += "select* from a where num between (@pageIndex-1)*@pageSize+1 and @pageIndex*@pageSize;";
                return cn.Query<Model.WorkInfoNo>(sql, page);
            }
        }

        public int GetMyCount(string userName)
        {
            using(IDbConnection cn=new MySqlConnection(cns))
            {
                string sql = "select count(1)from workinfo where userName=@userName";
                return cn.ExecuteScalar<int>(sql, new { userName = userName });
            }
        }

        public IEnumerable<Model.WorkInfoNo> GetMyPage(Model.WorkFindPage page)
        {
            using (IDbConnection cn = new MySqlConnection(cns))
            {
                string sql = "with a as(select row_number() over(order by uploadTime desc) as num, workinfo.*,activityName from workinfo join activity on  workinfo.activityId=activity.activityId where workinfo.userName=@userName)";
                sql += "select* from a where num between (@pageIndex-1)*@pageSize+1 and @pageIndex*@pageSize;";
                return cn.Query<Model.WorkInfoNo>(sql, page);
            }
        }

        public int Add(Model.WorkInfo workInfo)
        {
            using (IDbConnection cn = new MySqlConnection(cns))
            {
                string sql = "insert into workinfo(workId,workName,workPicture,uploadTime,workIntroduction,workVerify,userName,activityId,recommend,recommendTime)"+
                    "values(@workId,@workName,@workPicture,@uploadTime,@workIntroduction,@workVerify,@userName,@activityId,@recommend,@recommendTime);";
                sql += "SELECT @@IDENTITY";
                return cn.ExecuteScalar<int>(sql, workInfo);
            }
        }
        public int Update(Model.WorkInfo workinfo)
        {
            using (IDbConnection cn = new MySqlConnection(cns))
            {
                string sql = "update workinfo set workName=@workName,workPicture=@workPicture,uploadTime=@uploadTime,workIntroduction=@workIntroduction,workVerify=@workVerify,"+
                    "userName=@userName,recommend=@recommend,recommendTime=@recommendTime where workId=@workId";
                return cn.Execute(sql, workinfo);
            }
        }

        public int  UpdateImg(Model.WorkInfo workinfo)
        {
            using(IDbConnection cn=new MySqlConnection(cns))
            {
                string sql = "update workinfo set workPicture=@workPicture where workId=@workId";
                if (workinfo.workPicture == null)
                    sql = "update workinfo set workIntroduction=@workIntroduction where workId=@workId";
                return cn.Execute(sql, workinfo);
            }
        }

        public int UpdateVerify(Model.WorkInfo workinfo)
        {
            using (IDbConnection cn = new MySqlConnection(cns))
            {
                string sql = "update workinfo set workVerify=@workVerify where workId=@workId";
                return cn.Execute(sql, workinfo);
            }
        }

        public int Delete(int id)
        {
            using (IDbConnection cn = new MySqlConnection(cns))
            {
                string sql = "delete from userinfo where workid=@id";
                return cn.Execute(sql, new { id=id });
            }
        }

        public int UpdateRecommend(Model.WorkInfo workinfo)
        {
            using (IDbConnection cn = new MySqlConnection(cns))
            {
                string sql = "update workinfo set recommend=@recommend,recommendTime=@recommendTime where workId=@workId";
                return cn.Execute(sql, workinfo);
            }
        }
    }
}
