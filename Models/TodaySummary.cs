namespace Student_Att.Models
{
    // Numbers shown on the dashboard
    public class TodaySummary
    {
        public int TotalStudents { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public int NotMarkedCount { get; set; }
    }
}
