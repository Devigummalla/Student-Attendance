using System;
using Student_Att.DataAccess;
using Student_Att.Models;

namespace Student_Att.Business
{
    // Rules for marking attendance
    public class AttendanceService
    {
        // Shown in the popup - must match the assignment text exactly
        public const string AlreadySubmittedMessage = "Attendance Already Submitted for today";

        private readonly AttendanceRepository attendanceRepository = new AttendanceRepository();

        // Returns null when today's attendance is not marked yet
        public AttendanceRecord GetTodayAttendance(int studentId)
        {
            return attendanceRepository.GetTodayAttendance(studentId);
        }

        public MarkAttendanceResult MarkAttendance(int studentId, string status, int markedByUserId)
        {
            // Only these two values are allowed (the database checks this too)
            if (status != "Present" && status != "Absent")
            {
                throw new ArgumentException("Attendance status must be Present or Absent.");
            }
            if (studentId <= 0 || markedByUserId <= 0)
            {
                throw new ArgumentException("Invalid student or user.");
            }

            // The date is NOT passed: the database uses today's date itself
            return attendanceRepository.MarkAttendance(studentId, status, markedByUserId);
        }

        public TodaySummary GetTodaySummary()
        {
            return attendanceRepository.GetTodaySummary();
        }
    }
}
