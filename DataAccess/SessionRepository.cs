using System;
using System.Data;
using System.Data.SqlClient;

namespace Student_Att.DataAccess
{
    // Database work for the UserSessions table (one row per login token)
    public class SessionRepository
    {
        public void SaveSession(Guid tokenId, int userId, DateTime expiresAt)
        {
            using (SqlConnection connection = DbHelper.GetConnection())
            using (SqlCommand command = new SqlCommand("usp_SaveSession", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@TokenId", SqlDbType.UniqueIdentifier).Value = tokenId;
                command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                command.Parameters.Add("@ExpiresAt", SqlDbType.DateTime).Value = expiresAt;

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public bool IsSessionValid(Guid tokenId)
        {
            using (SqlConnection connection = DbHelper.GetConnection())
            using (SqlCommand command = new SqlCommand("usp_IsSessionValid", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@TokenId", SqlDbType.UniqueIdentifier).Value = tokenId;

                connection.Open();
                int count = Convert.ToInt32(command.ExecuteScalar());
                return count > 0;
            }
        }

        public void EndSession(Guid tokenId)
        {
            using (SqlConnection connection = DbHelper.GetConnection())
            using (SqlCommand command = new SqlCommand("usp_EndSession", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@TokenId", SqlDbType.UniqueIdentifier).Value = tokenId;

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}
