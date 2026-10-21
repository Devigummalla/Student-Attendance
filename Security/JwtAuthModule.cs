using System;
using System.Security.Principal;
using System.Web;
using Student_Att.Business;
using Student_Att.Logging;

namespace Student_Att.Security
{
    // An HttpModule runs for EVERY request, before the page.
    // 1. AuthenticateRequest: read the JWT cookie and decide who the user is.
    // 2. Web.config <authorization> rules then allow or block the page.
    // 3. EndRequest: if the page was blocked (401), send the user to Login or AccessDenied.
    public class JwtAuthModule : IHttpModule
    {
        public void Init(HttpApplication application)
        {
            application.AuthenticateRequest += OnAuthenticateRequest;
            application.EndRequest += OnEndRequest;
        }

        public void Dispose()
        {
        }

        private void OnAuthenticateRequest(object sender, EventArgs e)
        {
            HttpContext context = ((HttpApplication)sender).Context;
            TokenInfo user = null;

            HttpCookie cookie = context.Request.Cookies[AuthCookie.Name];
            if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
            {
                try
                {
                    AuthService authService = new AuthService();
                    user = authService.ValidateToken(cookie.Value);
                }
                catch (Exception ex)
                {
                    // e.g. database is down: log it and treat the user as not logged in
                    Logger.LogError(ex);
                    user = null;
                }

                if (user == null)
                {
                    AuthCookie.Remove(context.Response);   // expired or logged-out token
                }
            }

            if (user == null)
            {
                // Anonymous user: empty name means "not authenticated"
                context.User = new GenericPrincipal(new GenericIdentity(""), new string[0]);
                return;
            }

            // Logged-in user with a role, so web.config role rules work
            context.User = new GenericPrincipal(new GenericIdentity(user.FullName, "JWT"), new string[] { user.Role });
            CurrentUser.Set(context, user);
        }

        private void OnEndRequest(object sender, EventArgs e)
        {
            HttpContext context = ((HttpApplication)sender).Context;

            // 401 = the <authorization> rules in web.config blocked this page
            if (context.Response.StatusCode != 401)
            {
                return;
            }

            string redirectUrl;
            if (context.User != null && context.User.Identity.IsAuthenticated)
            {
                // Logged in, but wrong role (for example Faculty opening an Admin page)
                redirectUrl = VirtualPathUtility.ToAbsolute("~/AccessDenied.aspx");
            }
            else
            {
                // Not logged in: go to Login, then come back to the page they wanted
                redirectUrl = VirtualPathUtility.ToAbsolute("~/Account/Login.aspx")
                              + "?ReturnUrl=" + HttpUtility.UrlEncode(context.Request.RawUrl);
            }

            context.Response.ClearContent();
            context.Response.Redirect(redirectUrl, false);
        }
    }
}
