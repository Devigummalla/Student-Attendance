using System;
using System.Data;
using System.Data.SqlClient;
using Student_Att.Models;

namespace Student_Att.DataAccess
{
    // Database work for the Users table
    public class UserRepository
    {
        public RegisterResult RegisterUser(string fullName, string email, string username, string pinHash)
        {
            using (SqlConnection connection = DbHelper.GetConnection())
            using (SqlCommand command = new SqlCommand("usp_RegisterUser", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@FullName", SqlDbType.NVarChar, 100).Value = fullName;
                command.Parameters.Add("@Email", SqlDbType.NVarChar, 150).Value = email;
                command.Parameters.Add("@Username", SqlDbType.NVarChar, 50).Value = username;
                command.Parameters.Add("@PinHash", SqlDbType.VarChar, 200).Value = pinHash;

                connection.Open();
                int result = Convert.ToInt32(command.ExecuteScalar());
                return (RegisterResult)result;
            }
        }

        // Returns null when no user has this username or email
        public User GetUserForLogin(string login)
        {
            using (SqlConnection connection = DbHelper.GetConnection())
            using (SqlCommand command = new SqlCommand("usp_GetUserForLogin", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@Login", SqlDbType.NVarChar, 150).Value = login;

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    User user = new User();
                    user.UserId = Convert.ToInt32(reader["UserId"]);
                    user.FullName = reader["FullName"].ToString();
                    user.Username = reader["Username"].ToString();
                    user.PinHash = reader["PinHash"].ToString();
                    user.Role = reader["Role"].ToString();
                    user.IsActive = Convert.ToBoolean(reader["IsActive"]);
                    return user;
                }
            }
        }
    }
}
