namespace Student_Att.Models
{
    // The database stores the year as a number (1-4). This turns it into text.
    public static class YearOfStudyHelper
    {
        public static string GetName(int year)
        {
            switch (year)
            {
                case 1: return "First Year";
                case 2: return "Second Year";
                case 3: return "Third Year";
                case 4: return "Fourth Year";
                default: return "Unknown";
            }
        }
    }
}
