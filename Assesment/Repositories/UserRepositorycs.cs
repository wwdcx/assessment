using Assesment.Controllers;
using AssesmentProject.Controllers;
using AssesmentProject.Helpers;
using AssesmentProject.Models;
using Microsoft.Extensions.Caching.Memory;
using NLog;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Net.NetworkInformation;

namespace AssesmentProject.Repositories
{
    public class UserRepository
    {
        SqlExecution sqlexecution = new SqlExecution();
        BaseResponseHelper baseResponseHelper = new BaseResponseHelper();

        public bool Register(User user_details)
        {
            string sql = @"
                INSERT INTO dbo.users (username, mail, phone_number)
                VALUES(@username, @mail, @phone_number)
               
                DECLARE @user_id int 
                SET @user_id = (SELECT SCOPE_IDENTITY())
                SELECT @user_id

            ";

            SqlXParameter[] parameters = new SqlXParameter[]
            {
                new SqlXParameter("@username", user_details.UserName.Trim()),
                new SqlXParameter("@mail", user_details.Mail.Trim()),
                new SqlXParameter("@phone_number", user_details.PhoneNumber.Trim())
            };

            int id = (int)sqlexecution.ExecuteScalar(sql, parameters);

            if((int)id > 0)
            {
                if (user_details.Hobby.Count > 0)
                {
                    InsertUserHobbies(user_details.Hobby, (int)id);
                }

                if (user_details.SkillSets.Count > 0)
                {
                    InsertUserSkillsets(user_details.SkillSets, (int)id);
                }
                return true;
            }
            return false;
        }

        private void InsertUserHobbies(List<string> hobbyls, int userid)
        {
            string sql = @"
                INSERT INTO dbo.users_hobby (hobby, user_id)
                {0}
            ";

            string batch_insert_sql = "";
            foreach (var item in hobbyls)
            {
                if (string.IsNullOrEmpty(batch_insert_sql))
                {
                    batch_insert_sql = "SELECT '" + item.Trim() + "', " + userid;
                }
                else
                {
                    batch_insert_sql += " UNION ALL " + "SELECT '" + item.Trim() + "', " + userid;
                }
            }

            sql = string.Format(sql, batch_insert_sql);

            sqlexecution.ExecuteNonQuery(sql, null);
        }

        private void InsertUserSkillsets(List<string> skillsetls, int userid)
        {
            string sql = @"
                INSERT INTO dbo.users_skillsets (sillset, user_id)
                {0}
            ";

            string batch_insert_sql = "";
            foreach (var item in skillsetls)
            {
                if (string.IsNullOrEmpty(batch_insert_sql))
                {
                    batch_insert_sql = "SELECT '" + item.Trim() + "', " + userid;
                }
                else
                {
                    batch_insert_sql += " UNION ALL " + "SELECT '" + item.Trim() + "', " + userid;
                }
            }

            sql = string.Format(sql, batch_insert_sql);

            sqlexecution.ExecuteNonQuery(sql, null);
        }

        public List<ExpandoObject> SelectUserList(int page_num, int page_size)
        {
            List<ExpandoObject> userls = new List<ExpandoObject>();

            string sql = @"
                SELECT *, @pagenum as pageNum
                FROM(
                	SELECT u.user_id, u.username, u.mail, u.phone_number, (
                			SELECT STRING_AGG(hobby, ', ') 
                			FROM dbo.users_hobby
                			WHERE user_id = u.user_id	
                		)as hobby, (
                			SELECT STRING_AGG(sillset, ', ') 
                			FROM dbo.users_skillsets
                			WHERE user_id = u.user_id	
                		)as skillsets, ROW_NUMBER() OVER(ORDER BY u.username)as rowNum, COUNT(*) OVER(PARTITION BY NULL) as TotalCount
                	FROM dbo.users u
                )a
                WHERE rowNum > ((@pagenum - 1) * @pagesize) 
                AND rowNum <= (@pagenum * @pagesize)
            ";

            SqlXParameter[] parameters = new SqlXParameter[]
            {
            new SqlXParameter("@pagenum", page_num),
            new SqlXParameter("@pagesize", page_size),
            };

            DataSet ds = sqlexecution.ExecuteQuery(sql, parameters);

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                int TotalCount = dr.Field<int>("TotalCount");

                foreach (DataRow dritem in ds.Tables[0].Rows)
                {
                    dynamic objModel = new ExpandoObject();
                    objModel.UserID = dritem.Field<int?>("user_id");
                    objModel.UserName = dritem.Field<string>("username");
                    objModel.Mail = dritem.Field<string>("mail");
                    objModel.PhoneNumber = dritem.Field<string>("phone_number");
                    objModel.Hobby = dritem.Field<string?>("hobby") ?? "";
                    objModel.SkillSets = dritem.Field<string?>("skillsets") ?? "";
                    objModel.TotalCount = TotalCount;
                    userls.Add(objModel);
                }
            }
            return userls;
        }

        public bool UpdateUserDetails (User user_details, int user_id)
        {
            string sql = @"
                UPDATE dbo.users
                SET username = @username,
                    mail = @mail,
                    phone_number = @phone_number
                WHERE user_id = @user_id
            ";

            SqlXParameter[] parameters = new SqlXParameter[]
            {
                new SqlXParameter("@username", user_details.UserName.Trim()),
                new SqlXParameter("@mail", user_details.Mail.Trim()),
                new SqlXParameter("@phone_number", user_details.PhoneNumber.Trim()),
                new SqlXParameter("@user_id", user_id)
            };

            if(sqlexecution.ExecuteNonQuery(sql, parameters))
            {
                DeleteUserHobbiesAndSkillSets(user_id);
                if (user_details.Hobby.Count > 0)
                {
                    InsertUserHobbies(user_details.Hobby, user_id);
                }

                if (user_details.SkillSets.Count > 0)
                {
                    InsertUserSkillsets(user_details.SkillSets, user_id);
                }
                return true;
            }
            return false;
        }

        private void DeleteUserHobbiesAndSkillSets (int userid)
        {
            string sql = @"
                DELETE FROM dbo.users_hobby WHERE user_id = @user_id
                --JOIN dbo.users u ON (u.user_id = uh.user_id)
                --WHERE u.user_id = @user_id

                DELETE FROM dbo.users_skillsets WHERE user_id = @user_id
                --JOIN dbo.users u ON (u.user_id = us.user_id)
                --WHERE u.user_id = @user_id
            ";

            SqlXParameter[] parameters = new SqlXParameter[]
            {
                new SqlXParameter ("@user_id", userid),
            };

            sqlexecution.ExecuteNonQuery (sql, parameters);
        }

        public bool DeleteUser (int userid)
        {
            string sql = @"
                DELETE FROM dbo.users WHERE user_id = @user_id
            ";

            SqlXParameter[] parameters = new SqlXParameter[]
            {
                new SqlXParameter("@user_id", userid)
            };

            if (sqlexecution.ExecuteNonQuery(sql, parameters))
            {
                DeleteUserHobbiesAndSkillSets(userid);
                return true;
            }
            return false;
        }

        public List<User> SelectUserById(int userid)
        {
            List<User> userls = new List<User>();
            string sql = @"
                SELECT u.username, u.mail, u.phone_number, (
                    	SELECT STRING_AGG(hobby, ', ') 
                    	FROM dbo.users_hobby
                    	WHERE user_id = u.user_id	
                    )as hobby, (
                    	SELECT STRING_AGG(sillset, ', ') 
                    	FROM dbo.users_skillsets
                    	WHERE user_id = u.user_id	
                    )as skillsets
                FROM dbo.users u
	            WHERE u.user_id = @user_id
            ";

            SqlXParameter[] parameters = new SqlXParameter[]
            {
            new SqlXParameter("@user_id", userid),
            };

            DataSet ds = sqlexecution.ExecuteQuery(sql, parameters);

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                userls.Add(new User
                {
                    UserName = dr.Field<string>("username"),
                    Mail = dr.Field<string>("mail"),
                    PhoneNumber = dr.Field<string>("phone_number"),
                    Hobby = dr.Field<string?>("hobby") == null ? null : (dr.Field<string>("hobby")).Split(',').ToList(),
                    SkillSets = dr.Field<string>("skillsets") == null ? null : (dr.Field<string>("skillsets")).Split(',').ToList()
                });
            }
            return userls;
        }
    }
}
