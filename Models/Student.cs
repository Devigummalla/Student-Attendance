namespace Student_Att.Models
{
    public class Student
    {
        public int StudentId { get; set; }
        public string RollNumber { get; set; }
        public string StudentName { get; set; }
        public string DepartmentName { get; set; }
        public int YearOfStudy { get; set; }
        public bool IsActive { get; set; }

        // "Present", "Absent" or null when today's attendance is not marked yet
        public string TodayStatus { get; set; }

        // Read-only property: 2 -> "Second Year"
        public string YearName
        {
            get { return YearOfStudyHelper.GetName(YearOfStudy); }
        }
    }
}
