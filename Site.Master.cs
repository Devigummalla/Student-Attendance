using System;
using System.Web;
using System.Web.UI;
using Student_Att.Business;
using Student_Att.Security;

namespace Student_Att
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            TokenInfo user = CurrentUser.Get();
            if (user == null)
            {
                return;
            }

            // Ties every form on the page to this login.
            // A form posted from another website will fail (protects against CSRF).
            Page.ViewStateUserKey = user.TokenId.ToString();

            // Pages with personal data must not be cached by the browser,
            // otherwise the Back button could show them after logout.
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            TokenInfo user = CurrentUser.Get();
            bool isLoggedIn = user != null;

            // Logged in: show the left menu. Not logged in: show only the title.
            phSidebar.Visible = isLoggedIn;
            phGuestHeader.Visible = !isLoggedIn;

            if (isLoggedIn)
            {
                phAdminLink.Visible = CurrentUser.IsAdmin();
                lblUserName.Text = Server.HtmlEncode(user.FullName);   // encode: never trust text from the database
                lblRole.Text = Server.HtmlEncode(user.Role);
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            TokenInfo user = CurrentUser.Get();
            if (user != null)
            {
                // Mark the token as logged out in the database,
                // so it cannot be used again even if someone copied it
                AuthService authService = new AuthService();
                authService.Logout(user.TokenId);
            }

            AuthCookie.Remove(Response);
            Response.Redirect("~/Account/Login.aspx?loggedout=1");
        }

        // Returns "active" for the menu link of the page we are on, so it is highlighted
        protected string GetActiveClass(params string[] pages)
        {
            string currentPage = Request.AppRelativeCurrentExecutionFilePath;

            foreach (string page in pages)
            {
                if (string.Equals(currentPage, page, StringComparison.OrdinalIgnoreCase))
                {
                    return "active";
                }
            }

            return "";
        }
    }
}
