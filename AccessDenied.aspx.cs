using System;
using System.Web.UI;

namespace Student_Att
{
    // Shown when a logged-in user opens a page their role cannot use
    // (for example a Faculty user opening Admin/ManageStudents.aspx)
    public partial class AccessDeniedPage : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }
    }
}
