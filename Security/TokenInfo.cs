using System;

namespace Student_Att.Security
{
    // The information we put inside the JWT (and read back out of it)
    public class TokenInfo
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public Guid TokenId { get; set; }          // "jti" - also saved in UserSessions
        public DateTime ExpiresAtUtc { get; set; }
    }
}
