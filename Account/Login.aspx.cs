using System;
using System.Web.UI;
using Student_Att.Business;
using Student_Att.Models;
using Student_Att.Security;

namespace Student_Att
{
    public partial class LoginPage : Page
    {
        private readonly AuthService authService = new AuthService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }

            // Already logged in? Go straight to the dashboard
            if (CurrentUser.Get() != null)
            {
                Response.Redirect("~/Secure/Dashboard.aspx");
            }

            if (Request.QueryString["loggedout"] == "1")
            {
                ShowMessage("You have been logged out successfully.", "alert alert-info d-block");
            }
            else if (!string.IsNullOrEmpty(Request.QueryString["ReturnUrl"]))
            {
                ShowMessage("Please log in to continue.", "alert alert-warning d-block");
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                return;
            }

            string errorMessage;
            User user = authService.Login(txtLogin.Text, txtPin.Text, out errorMessage);

            if (user == null)
            {
                ShowMessage(errorMessage, "alert alert-danger d-block");
                return;
            }

            // Create the JWT and keep it in a secure cookie
            string token = authService.CreateLoginToken(user);
            AuthCookie.Save(Response, Request, token);

            Response.Redirect(GetSafeReturnUrl());
        }

        // After login we may send the user back to the page they asked for.
        // Only links inside THIS website are allowed; otherwise an attacker could
        // send people to a fake site with a link like Login.aspx?ReturnUrl=http://evil.com
        private string GetSafeReturnUrl()
        {
            string returnUrl = Request.QueryString["ReturnUrl"];

            bool isLocalUrl = !string.IsNullOrEmpty(returnUrl)
                              && returnUrl.StartsWith("/")
                              && !returnUrl.StartsWith("//")
                              && !returnUrl.StartsWith("/\\");

            if (isLocalUrl)
            {
                return returnUrl;
            }

            return "~/Secure/Dashboard.aspx";
        }

        private void ShowMessage(string message, string cssClass)
        {
            lblMessage.CssClass = cssClass;
            lblMessage.Text = Server.HtmlEncode(message);
        }
    }
}
