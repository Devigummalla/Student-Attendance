using System;

namespace Student_Att.Models
{
    // Today's attendance for one student (used to show "already submitted")
    public class AttendanceRecord
    {
        public string AttendanceStatus { get; set; }
        public string MarkedBy { get; set; }
        public DateTime MarkedAt { get; set; }
    }
}
