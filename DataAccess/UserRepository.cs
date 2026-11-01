using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Student_Att.Entities;
using Student_Att.Models;

namespace Student_Att.DataAccess
{
    // Database work for the Users table (EF Core)
    public class UserRepository
    {
        public RegisterResult RegisterUser(string fullName, string email, string username, string pinHash)
        {
            using (AttendanceDbContext db = new AttendanceDbContext())
            {
                // Friendly checks first
                if (db.Users.Any(u => u.Username == username))
                {
                    return RegisterResult.UsernameTaken;
                }
                if (db.Users.Any(u => u.Email == email))
                {
                    return RegisterResult.EmailTaken;
                }

                Users newUser = new Users();
                newUser.FullName = fullName;
                newUser.Email = email;
                newUser.Username = username;
                newUser.PinHash = pinHash;
                newUser.Role = "Faculty";          // self sign-up is always Faculty
                newUser.IsActive = true;
                newUser.CreatedAt = DateTime.Now;

                db.Users.Add(newUser);

                try
                {
                    db.SaveChanges();              // INSERT happens here
                    return RegisterResult.Success;
                }
                catch (DbUpdateException ex)
                {
                    // Two people signed up with the same username/email at the same moment:
                    // the UNIQUE rule in the database stopped the second one.
                    if (!DbErrorHelper.IsDuplicateKey(ex))
                    {
                        throw;
                    }
                }
            }

            // Find out which value was taken, using a fresh context
            using (AttendanceDbContext db = new AttendanceDbContext())
            {
                if (db.Users.Any(u => u.Username == username))
                {
                    return RegisterResult.UsernameTaken;
                }
                return RegisterResult.EmailTaken;
            }
        }

        // Returns null when no user has this username or email
        public User GetUserForLogin(string login)
        {
            using (AttendanceDbContext db = new AttendanceDbContext())
            {
                Users found = db.Users.FirstOrDefault(u => u.Username == login || u.Email == login);

                if (found == null)
                {
                    return null;
                }

                // Copy the database entity into our own model class
                User user = new User();
                user.UserId = found.UserId;
                user.FullName = found.FullName;
                user.Username = found.Username;
                user.PinHash = found.PinHash;
                user.Role = found.Role;
                user.IsActive = found.IsActive == true;   // IsActive is bool? in the scaffolded entity
                return user;
            }
        }
    }
}
