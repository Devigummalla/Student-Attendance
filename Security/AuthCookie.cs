using System;
using System.Web;

namespace Student_Att.Security
{
    // The JWT is stored in a cookie, because a browser sends cookies
    // automatically with every page request and postback.
    public static class AuthCookie
    {
        public const string Name = "SAMS_AUTH";

        public static void Save(HttpResponse response, HttpRequest request, string token)
        {
            HttpCookie cookie = new HttpCookie(Name, token);
            cookie.HttpOnly = true;                       // JavaScript cannot read it (protects against XSS)
            cookie.Secure = request.IsSecureConnection;   // only sent over HTTPS when the site uses HTTPS
            cookie.SameSite = SameSiteMode.Strict;        // not sent from other websites (protects against CSRF)
            response.Cookies.Add(cookie);
        }

        public static void Remove(HttpResponse response)
        {
            // A cookie is deleted by sending it again with a date in the past
            HttpCookie cookie = new HttpCookie(Name, "");
            cookie.HttpOnly = true;
            cookie.SameSite = SameSiteMode.Strict;
            cookie.Expires = DateTime.Now.AddDays(-1);
            response.Cookies.Add(cookie);
        }
    }
}
