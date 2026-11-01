using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Student_Att.Entities
{
    public partial class Attendance
    {
        public int AttendanceId { get; set; }
        public int StudentId { get; set; }
        public DateTime AttendanceDate { get; set; }
        public string AttendanceStatus { get; set; }
        public int MarkedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual Users MarkedByUser { get; set; }
        public virtual Students Student { get; set; }
    }
}
