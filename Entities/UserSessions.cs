using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Student_Att.Entities
{
    public partial class UserSessions
    {
        public int SessionId { get; set; }
        public Guid TokenId { get; set; }
        public int UserId { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsLoggedOut { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual Users User { get; set; }
    }
}
