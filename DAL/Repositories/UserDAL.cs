using DAL;
using DTO.DTOs;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muvi.DAL
{
    public class UserDAL
    {
        private DapperHelper db = new DapperHelper();

        public UserDTO Login(string username, string password)
        {
            string query = "SELECT * FROM Users WHERE Username = @user AND Password = @pass AND IsActive = 1";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@user", username),
                new SqlParameter("@pass", password) // Lưu ý: Thực tế nên hash password
            };

            DataTable dt = db.ExecuteQuery(query, parameters);

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                return new UserDTO
                {
                    UserID = (int)row["UserID"],
                    Username = row["Username"].ToString(),
                    FullName = row["FullName"].ToString(),
                    Role = row["Role"].ToString()
                };
            }
            return null;
        }
    }
}
