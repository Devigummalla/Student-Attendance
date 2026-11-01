using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Student_Att.Entities
{
    public partial class Users
    {
        public Users()
        {
            Attendance = new HashSet<Attendance>();
            UserSessions = new HashSet<UserSessions>();
        }

        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string PinHash { get; set; }
        public string Role { get; set; }
        public bool? IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<Attendance> Attendance { get; set; }
        public virtual ICollection<UserSessions> UserSessions { get; set; }
    }
}
