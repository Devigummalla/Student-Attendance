using System;
using System.Data;
using System.Data.SqlClient;
using Student_Att.Models;

namespace Student_Att.DataAccess
{
    // Database work for the Attendance table
    public class AttendanceRepository
    {
        // Returns null when attendance has NOT been taken today
        public AttendanceRecord GetTodayAttendance(int studentId)
        {
            using (SqlConnection connection = DbHelper.GetConnection())
            using (SqlCommand command = new SqlCommand("usp_GetTodayAttendance", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@StudentId", SqlDbType.Int).Value = studentId;

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    AttendanceRecord record = new AttendanceRecord();
                    record.AttendanceStatus = reader["AttendanceStatus"].ToString();
                    record.MarkedBy = reader["MarkedBy"].ToString();
                    record.MarkedAt = Convert.ToDateTime(reader["MarkedAt"]);
                    return record;
                }
            }
        }

        public MarkAttendanceResult MarkAttendance(int studentId, string status, int markedByUserId)
        {
            using (SqlConnection connection = DbHelper.GetConnection())
            using (SqlCommand command = new SqlCommand("usp_MarkAttendance", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@StudentId", SqlDbType.Int).Value = studentId;
                command.Parameters.Add("@AttendanceStatus", SqlDbType.VarChar, 10).Value = status;
                command.Parameters.Add("@MarkedByUserId", SqlDbType.Int).Value = markedByUserId;

                connection.Open();
                int result = Convert.ToInt32(command.ExecuteScalar());
                return (MarkAttendanceResult)result;
            }
        }

        public TodaySummary GetTodaySummary()
        {
            using (SqlConnection connection = DbHelper.GetConnection())
            using (SqlCommand command = new SqlCommand("usp_GetTodaySummary", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    TodaySummary summary = new TodaySummary();

                    if (reader.Read())
                    {
                        summary.TotalStudents = ToInt(reader["TotalStudents"]);
                        summary.PresentCount = ToInt(reader["PresentCount"]);
                        summary.AbsentCount = ToInt(reader["AbsentCount"]);
                        summary.NotMarkedCount = ToInt(reader["NotMarkedCount"]);
                    }

                    return summary;
                }
            }
        }

        // SUM() returns NULL when there are no rows, so treat NULL as 0
        private int ToInt(object value)
        {
            if (value == DBNull.Value)
            {
                return 0;
            }
            return Convert.ToInt32(value);
        }
    }
}
