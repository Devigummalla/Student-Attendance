using System;
using System.Web.UI;
using Student_Att.Security;

namespace Student_Att
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (CurrentUser.Get() != null)
            {
                Response.Redirect("~/Secure/Dashboard.aspx");
            }
            else
            {
                Response.Redirect("~/Account/Login.aspx");
            }
        }
    }
}
