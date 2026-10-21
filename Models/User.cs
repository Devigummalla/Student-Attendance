namespace Student_Att.Models
{
    // A person who can log in (Admin or Faculty)
    public class User
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        public string PinHash { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
    }
}
