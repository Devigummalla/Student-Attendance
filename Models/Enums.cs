namespace Student_Att.Models
{
    // The numbers match the "Result" values returned by the stored procedures

    public enum RegisterResult
    {
        Success = 0,
        UsernameTaken = 1,
        EmailTaken = 2
    }

    public enum MarkAttendanceResult
    {
        Saved = 0,
        AlreadySubmitted = 1
    }
}
