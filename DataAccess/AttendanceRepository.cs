using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Student_Att.Entities;
using Student_Att.Models;

namespace Student_Att.DataAccess
{
    // Database work for the Attendance table (EF Core + LINQ)
    public class AttendanceRepository
    {
        // Returns null when attendance has NOT been taken today
        public AttendanceRecord GetTodayAttendance(int studentId)
        {
            DateTime today = DateTime.Today;

            using (AttendanceDbContext db = new AttendanceDbContext())
            {
                var query = from a in db.Attendance
                            join u in db.Users on a.MarkedByUserId equals u.UserId
                            where a.StudentId == studentId && a.AttendanceDate == today
                            select new AttendanceRecord
                            {
                                AttendanceStatus = a.AttendanceStatus,
                                MarkedBy = u.FullName,
                                MarkedAt = a.CreatedAt
                            };

                return query.FirstOrDefault();
            }
        }

        public MarkAttendanceResult MarkAttendance(int studentId, string status, int markedByUserId)
        {
            // The date always comes from the server, never from the browser
            DateTime today = DateTime.Today;

            using (AttendanceDbContext db = new AttendanceDbContext())
            {
                // Check 1: already marked today?
                bool alreadyMarked = db.Attendance.Any(a => a.StudentId == studentId && a.AttendanceDate == today);
                if (alreadyMarked)
                {
                    return MarkAttendanceResult.AlreadySubmitted;
                }

                Attendance record = new Attendance();
                record.StudentId = studentId;
                record.AttendanceDate = today;
                record.AttendanceStatus = status;
                record.MarkedByUserId = markedByUserId;
                record.CreatedAt = DateTime.Now;

                db.Attendance.Add(record);

                try
                {
                    db.SaveChanges();
                    return MarkAttendanceResult.Saved;
                }
                catch (DbUpdateException ex)
                {
                    // Check 2: UNIQUE (StudentId, AttendanceDate) in the database.
                    // Happens if two requests arrive at the same moment.
                    if (DbErrorHelper.IsDuplicateKey(ex))
                    {
                        return MarkAttendanceResult.AlreadySubmitted;
                    }
                    throw;
                }
            }
        }

        // Dashboard numbers for today
        public TodaySummary GetTodaySummary()
        {
            DateTime today = DateTime.Today;

            using (AttendanceDbContext db = new AttendanceDbContext())
            {
                TodaySummary summary = new TodaySummary();
                summary.TotalStudents = db.Students.Count(s => s.IsActive == true);
                summary.PresentCount = db.Attendance.Count(a => a.AttendanceDate == today && a.AttendanceStatus == "Present");
                summary.AbsentCount = db.Attendance.Count(a => a.AttendanceDate == today && a.AttendanceStatus == "Absent");
                summary.NotMarkedCount = summary.TotalStudents - summary.PresentCount - summary.AbsentCount;
                return summary;
            }
        }
    }
}
