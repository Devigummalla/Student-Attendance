using System;
using System.Linq;
using Student_Att.Entities;

namespace Student_Att.DataAccess
{
    // Database work for the UserSessions table (one row per login token)
    public class SessionRepository
    {
        public void SaveSession(Guid tokenId, int userId, DateTime expiresAt)
        {
            using (AttendanceDbContext db = new AttendanceDbContext())
            {
                UserSessions session = new UserSessions();
                session.TokenId = tokenId;
                session.UserId = userId;
                session.ExpiresAt = expiresAt;
                session.IsLoggedOut = false;
                session.CreatedAt = DateTime.Now;

                db.UserSessions.Add(session);
                db.SaveChanges();
            }
        }

        // Valid = this token exists, the user has not logged out, and it has not expired
        public bool IsSessionValid(Guid tokenId)
        {
            DateTime now = DateTime.Now;

            using (AttendanceDbContext db = new AttendanceDbContext())
            {
                return db.UserSessions.Any(s => s.TokenId == tokenId
                                             && s.IsLoggedOut == false
                                             && s.ExpiresAt > now);
            }
        }

        public void EndSession(Guid tokenId)
        {
            using (AttendanceDbContext db = new AttendanceDbContext())
            {
                UserSessions session = db.UserSessions.FirstOrDefault(s => s.TokenId == tokenId);

                if (session != null)
                {
                    session.IsLoggedOut = true;   // EF Core notices the change...
                    db.SaveChanges();             // ...and sends an UPDATE
                }
            }
        }
    }
}
