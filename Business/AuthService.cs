using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using Student_Att.DataAccess;
using Student_Att.Models;
using Student_Att.Security;

namespace Student_Att.Business
{
    // Rules for signup, login and logout
    public class AuthService
    {
        // The same rules are used by the validators on the Signup page (client side)
        public const string EmailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        public const string UsernamePattern = @"^[A-Za-z0-9_.]{3,50}$";   // no '@' and no spaces
        public const string PinPattern = @"^[0-9]{4,6}$";                  // 4 to 6 digits

        public const int TokenLifetimeMinutes = 60;

        private readonly UserRepository userRepository = new UserRepository();
        private readonly SessionRepository sessionRepository = new SessionRepository();

        public ServiceResult Register(string fullName, string email, string username, string pin, string confirmPin)
        {
            // Clean the input
            fullName = (fullName ?? "").Trim();
            email = (email ?? "").Trim().ToLower();
            username = (username ?? "").Trim();
            pin = pin ?? "";

            // Server-side validation: never trust only the browser
            if (fullName.Length < 2 || fullName.Length > 100)
            {
                return ServiceResult.Fail("Please enter your full name (2 to 100 characters).");
            }
            if (!Regex.IsMatch(email, EmailPattern) || email.Length > 150)
            {
                return ServiceResult.Fail("Please enter a valid email address.");
            }
            if (!Regex.IsMatch(username, UsernamePattern))
            {
                return ServiceResult.Fail("Username must be 3 to 50 characters: letters, numbers, dot or underscore.");
            }
            if (!Regex.IsMatch(pin, PinPattern))
            {
                return ServiceResult.Fail("PIN must be 4 to 6 digits.");
            }
            if (pin != confirmPin)
            {
                return ServiceResult.Fail("PIN and Confirm PIN do not match.");
            }

            // Only the hash is saved, never the PIN itself
            string pinHash = PinHasher.HashPin(pin);

            try
            {
                RegisterResult result = userRepository.RegisterUser(fullName, email, username, pinHash);

                switch (result)
                {
                    case RegisterResult.Success:
                        return ServiceResult.Ok("Registration successful! You can now log in with your username or email and PIN.");
                    case RegisterResult.UsernameTaken:
                        return ServiceResult.Fail("This username is already taken. Please choose another one.");
                    case RegisterResult.EmailTaken:
                        return ServiceResult.Fail("This email address is already registered.");
                    default:
                        return ServiceResult.Fail("Registration failed. Please try again.");
                }
            }
            catch (SqlException ex)
            {
                // 2627 / 2601 = UNIQUE rule in the database (two people signing up at the same moment)
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    return ServiceResult.Fail("This username or email is already registered.");
                }
                throw;
            }
        }

        // Returns the user if the login is correct, otherwise null and an error message
        public User Login(string login, string pin, out string errorMessage)
        {
            errorMessage = "";
            login = (login ?? "").Trim();

            if (login == "" || string.IsNullOrEmpty(pin))
            {
                errorMessage = "Please enter your username or email and your PIN.";
                return null;
            }

            User user = userRepository.GetUserForLogin(login);

            // Same message for "user not found" and "wrong PIN",
            // so nobody can use the login page to find out which usernames exist
            if (user == null || !PinHasher.VerifyPin(pin, user.PinHash))
            {
                errorMessage = "Invalid username/email or PIN.";
                return null;
            }

            if (!user.IsActive)
            {
                errorMessage = "Your account is disabled. Please contact the administrator.";
                return null;
            }

            return user;
        }

        // Creates the JWT for a user who just logged in, and remembers it in the database
        public string CreateLoginToken(User user)
        {
            TokenInfo info = new TokenInfo();
            info.UserId = user.UserId;
            info.FullName = user.FullName;
            info.Role = user.Role;
            info.TokenId = Guid.NewGuid();
            info.ExpiresAtUtc = DateTime.UtcNow.AddMinutes(TokenLifetimeMinutes);

            // The database compares with GETDATE() (server local time), so save local time there
            sessionRepository.SaveSession(info.TokenId, user.UserId, DateTime.Now.AddMinutes(TokenLifetimeMinutes));

            return JwtHelper.CreateToken(info);
        }

        // Used on every request: is this token genuine, not expired and not logged out?
        public TokenInfo ValidateToken(string token)
        {
            TokenInfo info = JwtHelper.ReadToken(token);
            if (info == null)
            {
                return null;   // fake, changed or expired token
            }

            if (!sessionRepository.IsSessionValid(info.TokenId))
            {
                return null;   // user already logged out with this token
            }

            return info;
        }

        public void Logout(Guid tokenId)
        {
            sessionRepository.EndSession(tokenId);
        }
    }
}
