using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Student_Att.Entities
{
    public partial class Students
    {
        public Students()
        {
            Attendance = new HashSet<Attendance>();
        }

        public int StudentId { get; set; }
        public string RollNumber { get; set; }
        public string StudentName { get; set; }
        public int DepartmentId { get; set; }
        public int AdmissionYear { get; set; }
        public int YearOfStudy { get; set; }
        public bool? IsActive { get; set; }

        public virtual Departments Department { get; set; }
        public virtual ICollection<Attendance> Attendance { get; set; }
    }
}
