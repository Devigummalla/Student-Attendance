using System;
using System.Web.UI;
using Student_Att.Business;
using Student_Att.Security;

namespace Student_Att
{
    public partial class SignupPage : Page
    {
        private readonly AuthService authService = new AuthService();

        protected void Page_Load(object sender, EventArgs e)
        {
            // Already logged in? No need to sign up
            if (!IsPostBack && CurrentUser.Get() != null)
            {
                Response.Redirect("~/Secure/Dashboard.aspx");
            }
        }

        protected void btnSignup_Click(object sender, EventArgs e)
        {
            // Runs the validators again on the server
            // (JavaScript checks in the browser can be switched off)
            if (!Page.IsValid)
            {
                return;
            }

            ServiceResult result = authService.Register(
                txtFullName.Text,
                txtEmail.Text,
                txtUsername.Text,
                txtPin.Text,
                txtConfirmPin.Text);

            if (result.Success)
            {
                pnlForm.Visible = false;
                pnlSuccess.Visible = true;
                lblSuccess.Text = Server.HtmlEncode(result.Message);
            }
            else
            {
                ShowError(result.Message);
            }
        }

        private void ShowError(string message)
        {
            lblMessage.CssClass = "alert alert-danger d-block";
            lblMessage.Text = Server.HtmlEncode(message);
        }
    }
}
